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
        
        // Part 2
        HashSet<Coord2D> outerWarpPositions = new();
        HashSet<Coord2D> innerWarpPositions = new();

        Coord2D startPosition = (0, 0);
        Coord2D endPosition = (0, 0);

        HashSet<Coord2D> Lvl0Warp = new();
        HashSet<Coord2D> Lvl1Warp = new();

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
                warpPositions[position] = string.Concat(Map[position - (2, 0)], Map[position - (1, 0)]);

            foreach (var position in horizontalWarpPositionsLeft)
                warpPositions[position] = string.Concat(Map[position + (1, 0)], Map[position + (2, 0)]);

            foreach (var position in verticaWarpPositionsUp)
                warpPositions[position] = string.Concat(Map[position + (0, 1)], Map[position + (0, 2)]);

            foreach (var position in verticaWarpPositionsDown)
                warpPositions[position] = string.Concat(Map[position - (0, 2)], Map[position - (0, 1)]);

            startPosition = warpPositions.Keys.Where(x => warpPositions[x] == "AA").First();
            endPosition = warpPositions.Keys.Where(x => warpPositions[x] == "ZZ").First();

            // Part 2
            Coord2D center = new Coord2D(Map.Keys.Max(p => p.x) / 2, Map.Keys.Max(p => p.y) / 2);
            var listOfWarpPoints = warpPositions.Values.Where(x => x!="AA" && x!="ZZ").Distinct();
            foreach (var label in listOfWarpPoints)
            {
                var positions = warpPositions.Keys.Where(x => warpPositions[x] == label).ToList();
                positions = positions.OrderBy(x => (x - center).VectorModule).ToList();
                innerWarpPositions.Add(positions[0]);
                outerWarpPositions.Add(positions[1]);
            }

            LetterPositions.ToList().ForEach(x => WallPositions.Add(x));

            foreach (var pos in outerWarpPositions)
                if (pos != startPosition && pos != endPosition)
                    Lvl1Warp.Add(pos);

            Lvl0Warp.Add(startPosition);
            Lvl0Warp.Add(endPosition);
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

        // Part 2
        List<Coord2D> GetNeighborsPart2(Coord2D position, int level)
        {
            var retValue = position.GetNeighbors().Where(x => Map.ContainsKey(x) && !WallPositions.Contains(x)).ToList();
            if (warpPositions.ContainsKey(position))
                retValue.AddRange(warpPositions.Keys.Where(x => warpPositions[x] == warpPositions[position] && x != position));

            if (level == 0 && retValue.Any(Lvl1Warp.Contains) && retValue.All(p => (position - p).VectorModule == 1))
                retValue = retValue.Where(x => !Lvl1Warp.Contains(x)).ToList();
            else if (level > 0 && retValue.Any(Lvl0Warp.Contains) && retValue.All(p => (position - p).VectorModule == 1))
                retValue = retValue.Where(x => !Lvl0Warp.Contains(x)).ToList();

            return retValue;
        }

        public int FindShortestPathPart2()
        {
            var visited = new Dictionary<(Coord2D, int), int>();
            var activeQueue = new Queue<(Coord2D,  int)>();
            var startPos = startPosition;

            activeQueue.Enqueue((startPos, 0));
            visited[(startPos,0)] = 0;

            while (activeQueue.Count > 0)
            {
                var (currentPos, currentLevel) = activeQueue.Dequeue();
                var cost = visited[(currentPos, currentLevel)];

                if (currentPos == endPosition && currentLevel == 0)
                    return cost;

                var neighbors = GetNeighborsPart2(currentPos, currentLevel).ToList();
                
                foreach (var neighbor in neighbors)
                {
                    var neighborLevel = currentLevel;
                    if ((neighbor - currentPos).VectorModule > 1)    // We have a warp
                        neighborLevel += innerWarpPositions.Contains(neighbor) ? -1 : 1;

                    if (visited.ContainsKey((neighbor, neighborLevel)))
                        continue;

                    activeQueue.Enqueue((neighbor, neighborLevel));
                    visited[(neighbor, neighborLevel)] = cost + 1;
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
            return part==1 ? mazeSolver.FindShortestPath() : mazeSolver.FindShortestPathPart2();
        }

        public long Solve(int part = 1)
            => FindShortestPath(part);
    }
}
