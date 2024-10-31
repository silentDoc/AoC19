using AoC19.Common;

namespace AoC19.Day18
{
    class MazeSolver
    {
        Dictionary<Coord2D, char> Map = new();
        HashSet<Coord2D> DoorPositions = new();
        HashSet<Coord2D> KeyPositions = new();
        HashSet<Coord2D> WallPositions = new();
        Coord2D startPosition = (0, 0);

        void ParseLine(string line, int row)
            => Enumerable.Range(0, line.Length).ToList().ForEach(x => Map[(x, row)] = line[x]);

        public void LoadMap(List<string> input)
        {
            Enumerable.Range(0, input.Count).ToList().ForEach(x => ParseLine(input[x], x));
            DoorPositions = Map.Keys.Where(x => char.IsLetter(Map[x]) && char.IsUpper(Map[x])).ToHashSet();
            KeyPositions  = Map.Keys.Where(x => char.IsLetter(Map[x]) && char.IsLower(Map[x])).ToHashSet();
            WallPositions = Map.Keys.Where(x => Map[x] == '#').ToHashSet();
            startPosition = Map.Keys.First(x => Map[x] == '@');
        }

        public void PaintMap()
        {
            Console.WriteLine("");
            for (int y = 0; y <= Map.Keys.Max(k => k.y); y++)
            {
                var keys = Map.Keys.Where(k => k.y == y).OrderBy(k => k.x).ToList();
                string line = string.Concat(keys.Select(k => Map[k]));
                Console.WriteLine(line);
            }
        }
       
        
        // state has to be sorted to avoid ambiguity
        string AddKey(char key, string keyState)
            => keyState.Contains(key) ? keyState : String.Concat((keyState + key.ToString()).OrderBy(c => c));  

        public int FindShortestPath()
        {
            var visited = new Dictionary<(Coord2D pos, string keyState), int>();
            var activeQueue = new Queue<(Coord2D pos, string keyState)>();
            var initialState = (startPosition, "");

            activeQueue.Enqueue(initialState);
            visited[initialState] = 0;

            while (activeQueue.Count > 0)
            {
                
                var state = activeQueue.Dequeue();
                var cost = visited[state];

                if (KeyPositions.Contains(state.pos))
                    state.keyState = AddKey(Map[state.pos], state.keyState);

                if (state.keyState.Length == KeyPositions.Count)
                    return cost;

                var neighbors = state.pos.GetNeighbors().Where(x => !WallPositions.Contains(x)).ToList();

                foreach (var neigh in neighbors)
                {
                    var tile = Map[neigh];
                    
                    if (DoorPositions.Contains(neigh))
                        if (!state.keyState.Contains(Char.ToLower(tile)))
                            continue;

                    var neighState = (neigh, state.keyState);

                    if (visited.ContainsKey(neighState))
                        continue;

                    activeQueue.Enqueue(neighState);
                    visited[neighState] = cost + 1;
                }
            }
            return -1;
        }

        // Part 2

        void UpdateMapPart2()
        {
            Map[(40, 40)] = Map[(39, 40)] = Map[(41, 40)] = Map[(40, 41)] = Map[(40, 39)] = '#';
            Map[(39, 39)] = Map[(39, 41)] = Map[(41, 41)] = Map[(41, 39)] = '@';
            WallPositions = Map.Keys.Where(x => Map[x] == '#').ToHashSet();
        }

        (Coord2D keyLocation, int distance) GetNearestReachableKey(Coord2D currentPosition, string keyState) 
        {
            var visited = new Dictionary<(Coord2D pos, string keyState), int>();
            var activeQueue = new Queue<(Coord2D pos, string keyState)>();
            var initialState = (currentPosition, keyState);

            activeQueue.Enqueue(initialState);
            visited[initialState] = 0;

            while (activeQueue.Count > 0)
            {

                var state = activeQueue.Dequeue();
                var cost = visited[state];

                if (KeyPositions.Contains(state.pos) && !state.keyState.Contains(Map[state.pos]))
                    return (state.pos, cost);

                var neighbors = state.pos.GetNeighbors().Where(x => !WallPositions.Contains(x)).ToList();

                foreach (var neigh in neighbors)
                {
                    var tile = Map[neigh];

                    if (DoorPositions.Contains(neigh))
                        if (!state.keyState.Contains(Char.ToLower(tile)))
                            continue;

                    var neighState = (neigh, state.keyState);

                    if (visited.ContainsKey(neighState))
                        continue;

                    activeQueue.Enqueue(neighState);
                    visited[neighState] = cost + 1;
                }
            }
            return ((-1,-1), 99999);    // No reachable key
        }

        public int FindShortestPath4Robots()
        {
            UpdateMapPart2();

            Dictionary<int, Coord2D> droidPositions = new();
            var robotPositions = Map.Keys.Where(x => Map[x] == '@').ToList();
            for (int i = 0; i < robotPositions.Count; i++)
                droidPositions[i] = robotPositions[i];

            var keyState = "";
            int totalSteps = 0;

            // The heuristic is simple. Find the nearest reachable key to any of the robots, 
            // get the closest one, and repeat until we have all the keys. We have to update positions though
            while (keyState.Length < KeyPositions.Count)
            {
                var nearestKeys = droidPositions.Values.Select(x => GetNearestReachableKey(x, keyState)).ToList();
                var shortestDistance = nearestKeys.Min(x => x.distance);
                var keyOfInterest = nearestKeys.First(x => x.distance == shortestDistance);
                var robotIndex = nearestKeys.IndexOf(keyOfInterest);

                // Update position of the droid that moved and also the keys we have
                droidPositions[robotIndex] = keyOfInterest.keyLocation;
                keyState = AddKey(Map[keyOfInterest.keyLocation], keyState);
                totalSteps += keyOfInterest.distance;
            }
            return totalSteps;
        }
    }

    internal class MazeRunner
    {
        List<string> inputMap = new();
        MazeSolver mazeSolver = new();

        public void ParseInput(List<string> lines)
            => inputMap = lines;

        public int FindShortestPath(int part = 1)
        {
            mazeSolver.LoadMap(inputMap);
            return part==1? mazeSolver.FindShortestPath() : mazeSolver.FindShortestPath4Robots();
        }

        public long Solve(int part = 1)
            => FindShortestPath(part);
    }
}
