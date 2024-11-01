using AoC19.Common;

namespace AoC19.Day20
{
    class DonutMazeSolver
    {
        Dictionary<Coord2D, char> Map = new();
        HashSet<Coord2D> OpenPositions = new();
        HashSet<Coord2D> LetterPositions = new();
        HashSet<Coord2D> WallPositions = new();
        Dictionary<Coord2D, string> warpPositions = new();

        Coord2D startPosition = (0, 0);
        Coord2D endPosition = (0, 0);

        // Transform open space to walls
        void ParseLine(string line, int row)
            => Enumerable.Range(0, line.Length).ToList().ForEach(x => Map[(x, row)] = line[x] == ' ' ? '#' : line[x]);

        public void LoadMap(List<string> input)
        {
            Enumerable.Range(0, input.Count).ToList().ForEach(x => ParseLine(input[x], x));
            LetterPositions = Map.Keys.Where(x => char.IsLetter(Map[x]) && char.IsUpper(Map[x])).ToHashSet();
            OpenPositions = Map.Keys.Where(x => Map[x] == '.').ToHashSet();
            WallPositions = Map.Keys.Where(x => Map[x] == '#').ToHashSet();

            // Parse warp positions - horizontal
            var horizontalWarpLabels = LetterPositions.Where(x => LetterPositions.Contains(x + (1, 0))).ToList();
            var horizontalWarpPositionsRight = OpenPositions.Where(x => horizontalWarpLabels.Contains(x - (2, 0))).ToList();
            var horizontalWarpPositionsLeft = OpenPositions.Where(x => horizontalWarpLabels.Contains(x + (1, 0))).ToList();

            // Parse warp positions - vertical
            var verticaWarpLabels = LetterPositions.Where(x => LetterPositions.Contains(x + (0, 1))).ToList();
            var verticaWarpPositionsUp = OpenPositions.Where(x => verticaWarpLabels.Contains(x + (0, 1))).ToList();
            var verticaWarpPositionsDown = OpenPositions.Where(x => verticaWarpLabels.Contains(x - (0, 2))).ToList();

            // Retrieve the warp cells into a dictionary
            foreach (var position in horizontalWarpPositionsRight)
            {
                var label = string.Concat(Map[position - (2, 0)], Map[position - (1, 0)]);
                warpPositions[position] = label;
            }

            foreach (var position in horizontalWarpPositionsLeft)
            {
                var label = string.Concat(Map[position + (1, 0)], Map[position + (2, 0)]);
                warpPositions[position] = label;
            }

            foreach (var position in verticaWarpPositionsUp)
            {
                var label = string.Concat(Map[position + (0, 1)], Map[position + (0, 2)]);
                warpPositions[position] = label;
            }

            foreach (var position in verticaWarpPositionsDown)
            {
                var label = string.Concat(Map[position - (0, 2)], Map[position - (0, 1)]);
                warpPositions[position] = label;
            }

            startPosition = warpPositions.Keys.Where(x => warpPositions[x] == "AA").First();
            endPosition = warpPositions.Keys.Where(x => warpPositions[x] == "ZZ").First();
        }

        List<Coord2D> GetNeighbors(Coord2D position)
        {
            var retValue = position.GetNeighbors().Where(x => Map.ContainsKey(x) && !WallPositions.Contains(x)).ToList();
            if (warpPositions.ContainsKey(position))
                retValue.AddRange(warpPositions.Keys.Where(x => warpPositions[x] == warpPositions[position] && x != position));
            return retValue;
        }

        public int FindShortestPath()
        {
            var visited = new Dictionary<Coord2D, int>();
            var activeQueue = new Queue<Coord2D>();
            var startPos = startPosition;

            activeQueue.Enqueue(startPos);
            visited[startPos] = 0;

            while (activeQueue.Count > 0)
            {
                var currentPos = activeQueue.Dequeue();
                var cost = visited[currentPos];

                if (currentPos == endPosition)
                    return cost;
                
                var neighbors = GetNeighbors(currentPos).Where(x => !visited.ContainsKey(x)).ToList();

                foreach (var neighbor in neighbors)
                {
                    if (visited.ContainsKey(neighbor))
                        continue;

                    activeQueue.Enqueue(neighbor);
                    visited[neighbor] = cost + 1;
                }
            }
            return -1;
        }
    }

    internal class DonutMazeRunner
    {
        List<string> inputMap = new();
        DonutMazeSolver mazeSolver = new();

        public void ParseInput(List<string> lines)
            => inputMap = lines;

        public int FindShortestPath(int part = 1)
        {
            mazeSolver.LoadMap(inputMap);
            return mazeSolver.FindShortestPath();
        }

        public long Solve(int part = 1)
            => FindShortestPath(part);
    }
}
