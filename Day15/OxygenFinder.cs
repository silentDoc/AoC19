using AoC19.Common;

namespace AoC19.Day15
{
    public static class DroidDir
    {
        public const int North = 1;
        public const int South = 2;
        public const int East  = 3;
        public const int West  = 4;
    }

    public static class Tile
    {
        public const int Wall = 0;
        public const int Floor = 1;
        public const int Oxygen = 2;
    }

    internal class FinderOperator
    {
        IntCodeProcessor droid = new();
        Dictionary<Coord2D, int> Map = new();
        Dictionary<Coord2D, List<int>> DirectionsFromCenter = new();
        Coord2D StartPos = new(0,0);

        public FinderOperator(List<string> sourceCode) 
            => droid.ParseInput(sourceCode);

        int GetDirection(Coord2D pos, Coord2D nextPos)
            => (nextPos - pos) switch
            {
                (1, 0) => DroidDir.East,
                (-1, 0) => DroidDir.West,
                (0, -1) => DroidDir.North,
                (0, 1) => DroidDir.South,
                _ => throw new Exception("Invalid positions")
            };

        int GetOppositeDirection(int direction)
            => direction switch
            {
                DroidDir.West => DroidDir.East,
                DroidDir.East => DroidDir.West,
                DroidDir.South => DroidDir.North,
                DroidDir.North => DroidDir.South,
                _ => throw new Exception("Invalid direction")
            };

        public void TraverseMap(List<Coord2D> boundaryPositions, int part = 1)
        {
            // Discovers the map from center until it finds oxygen
            List<Coord2D> newBoundaries = new();
            foreach (var position in boundaryPositions)
            {
                // First, we go to the boundary positions
                var pathToGetThere = DirectionsFromCenter[position];
                foreach (var step in pathToGetThere)
                    droid.RunProgram(step);
                // Get the unexplored neighbors
                var neighbors = position.GetNeighbors().Where(x => !Map.ContainsKey(x)).ToList();

                foreach (var neighbor in neighbors)
                {
                    // Discover the new tiles and set tile and path to get there
                    var newStepDirection = GetDirection(position, neighbor);
                    var newTile = droid.RunProgram(newStepDirection);
                    Map[neighbor] = (int) newTile;
                    DirectionsFromCenter[neighbor] = [.. pathToGetThere, newStepDirection];

                    if (newTile != Tile.Wall)
                    {
                        // move back to the boundary position
                        var backtrack = GetOppositeDirection(newStepDirection);
                        droid.RunProgram(backtrack);
                        newBoundaries.Add(neighbor);
                    }
                }

                // Get back to the center to be ready to go to a new boundary position in the next iteration
                var reversePath = new List<int>();
                reversePath.AddRange(pathToGetThere);
                reversePath.Reverse();

                foreach (var step in reversePath)
                    droid.RunProgram(GetOppositeDirection(step));
            }

            newBoundaries = newBoundaries.Distinct().ToList();

            var condition = part == 1 ? !Map.ContainsValue(Tile.Oxygen) : newBoundaries.Count > 0;
            if (condition)
                TraverseMap(newBoundaries, part);
        }

        int ExpandOxygen()
        {
            int count = 0;
            while (true)
            {
                var oxyPositions = Map.Keys.Where(x => Map[x] == Tile.Oxygen);
                var expansion = oxyPositions.SelectMany(x => x.GetNeighbors().Where(y => Map[y] == Tile.Floor)).ToList();

                if (expansion.Count == 0)   // No more space to fill with oxygen
                    break;

                expansion.ForEach(x => Map[x] = Tile.Oxygen); 
                count++;
            }
            return count;
        }

        public void DiscoverMap(int part =1)
        {
            Map[StartPos] = Tile.Floor;
            DirectionsFromCenter[StartPos] = new List<int>();
            TraverseMap([StartPos], part);
        }

        public int FindOxygen(int part =1)
        {
            DiscoverMap(part);
            var posOxygen = Map.Keys.Where(x => Map[x] == Tile.Oxygen).First();
            return part ==1 ? DirectionsFromCenter[posOxygen].Count() : ExpandOxygen();
        }
    }

    internal class OxygenFinder
    {
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        long FindOxygen(int part = 1)
        {
            FinderOperator OxygenOp = new(sourceCode);
            int steps = OxygenOp.FindOxygen(part);
            return steps;
        }

        public long Solve(int part = 1)
            => FindOxygen(part);
    }
}
