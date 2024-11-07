using AoC19.Common;

namespace AoC19.Day24
{
    public static class Tile
    {
        public static char Bug   = '#';
        public static char Space = '.';
    }

    class LifeSim
    {
        Dictionary<Coord2D, char> InitialState = new Dictionary<Coord2D, char>();

        void ParseLine(string line, int row)
            => Enumerable.Range(0, line.Length).ToList().ForEach(x => InitialState[(x, row)] = line[x]);

        public void LoadInitialState(List<string> input)
            => input.ForEach(x => ParseLine(x, input.IndexOf(x)));

        string GetState(Dictionary<Coord2D, char> current)
            => string.Concat(current.Keys.OrderBy(k => k.y).ThenBy(k => k.x).Select(k => current[k].ToString()));

        long CalcBio(Dictionary<Coord2D, char> state)
        {
            var maxX = state.Keys.Max(k => k.x) + 1;
            var powers = state.Keys.Where(k => state[k] == Tile.Bug).Select(k => k.y * maxX + k.x);
            return powers.Sum(x => (long)Math.Pow(2, x));
        }

        Dictionary<Coord2D, char> Evolve(Dictionary<Coord2D, char> current)
        {
            var nextState = new Dictionary<Coord2D, char>();

            foreach (var key in current.Keys)
            {
                var adjBugs = key.GetNeighbors().Count(n => current.ContainsKey(n) && current[n] == Tile.Bug);

                if (current[key] == Tile.Bug && adjBugs != 1)
                    nextState[key] = Tile.Space;
                else if (current[key] == Tile.Space && (adjBugs == 1 || adjBugs == 2))
                    nextState[key] = Tile.Bug;
                else
                    nextState[key] = current[key];
            }
            return nextState;
        }

        public long SolvePart1()
        {
            HashSet<string> seen = new();
            Dictionary<Coord2D, char> currentState = new();

            InitialState.Keys.ToList().ForEach(x => currentState[x] = InitialState[x]);

            while (seen.Add(GetState(currentState)))
            {
                var nextState = Evolve(currentState);
                nextState.Keys.ToList().ForEach(x => currentState[x] = nextState[x]);
            }

            return CalcBio(currentState);
        }

        // Part 2 
        // 0 1 2 3 4   - y=0
        // 0 1 2 3 4   - y=1
        // 0 1 ? 3 4   - y=2
        // 0 1 2 3 4   - y=3
        // 0 1 2 3 4   - y=4

        HashSet<Coord2D> outerLeftPositions = [(0, 0), (0, 1), (0, 2), (0, 3), (0, 4)];
        HashSet<Coord2D> outerRightPositions = [(4, 0), (4, 1), (4, 2), (4, 3), (4, 4)];
        HashSet<Coord2D> outerTopPositions = [(0, 0), (1, 0), (2, 0), (3, 0), (4, 0)];
        HashSet<Coord2D> outerBottomPositions = [(0, 4), (1, 4), (2, 4), (3, 4), (4, 4)];

        HashSet<Coord2D> innerLeftPositions = [(1, 2)];
        HashSet<Coord2D> innerRightPositions = [(3, 2)];
        HashSet<Coord2D> innerTopPositions = [(2, 1)];
        HashSet<Coord2D> innerBottomPositions = [(2, 3)];

        HashSet<Coord2D> innerPositions = [(1, 2), (3, 2), (2, 3), (2, 1)];
        HashSet<Coord2D> outerPositions = [(0, 0), (0, 1), (0, 2), (0, 3), (4, 0), (4, 1), (4, 2), (4, 3),
                                           (1, 0), (2, 0), (3, 0), (0, 4), (1, 4), (2, 4), (3, 4), (4, 4)];

        HashSet<Coord2D> validKeys = [(0,0), (1,0), (2,0), (3,0), (4,0),
                                      (0,1), (1,1), (2,1), (3,1), (4,1),
                                      (0,2), (1,2),        (3,2), (4,2),
                                      (0,3), (1,3), (2,3), (3,3), (4,3),
                                      (0,4), (1,4), (2,4), (3,4), (4,4)];

        int GetAdjacentBugs((int level, Coord2D pos) key, Dictionary<(int level, Coord2D pos), char> current)
        {
            var lowerLevel = key.level - 1;
            var upperLevel = key.level + 1;

            // Find neighbors
            var neighs = key.pos.GetNeighbors().Where(x => validKeys.Contains(x)).Select(x => (key.level, x)).ToList();
            neighs = neighs.Where(x => current.ContainsKey(x)).ToList();
            
            if (innerPositions.Contains(key.pos) && current.Keys.Any(x => x.level ==lowerLevel))
            {
                if (innerLeftPositions.Contains(key.pos))
                    neighs.AddRange(outerLeftPositions.Select(x => (lowerLevel, x)).ToList());
                if (innerTopPositions.Contains(key.pos))
                    neighs.AddRange(outerTopPositions.Select(x => (lowerLevel, x)).ToList());
                if (innerRightPositions.Contains(key.pos))
                    neighs.AddRange(outerRightPositions.Select(x => (lowerLevel, x)).ToList());
                if (innerBottomPositions.Contains(key.pos))
                    neighs.AddRange(outerBottomPositions.Select(x => (lowerLevel, x)).ToList());
            }

            if (outerPositions.Contains(key.pos) && current.Keys.Any(x => x.level == upperLevel))
            {
                if (outerLeftPositions.Contains(key.pos))
                    neighs.AddRange(innerLeftPositions.Select(x => (upperLevel, x)).ToList());
                if (outerTopPositions.Contains(key.pos))
                    neighs.AddRange(innerTopPositions.Select(x => (upperLevel, x)).ToList());
                if (outerRightPositions.Contains(key.pos))
                    neighs.AddRange(innerRightPositions.Select(x => (upperLevel, x)).ToList());
                if (outerBottomPositions.Contains(key.pos))
                    neighs.AddRange(innerBottomPositions.Select(x => (upperLevel, x)).ToList());
            }

            return neighs.Count(x => current[x] == Tile.Bug);
        }

        Dictionary<(int level, Coord2D pos), char> Evolve_part2(Dictionary<(int level, Coord2D pos), char> current)
        {
            var nextState = new Dictionary<(int level, Coord2D pos), char>();

            foreach (var key in current.Keys)
            {
                var adjBugs = GetAdjacentBugs(key, current);

                if (current[key] == Tile.Bug && adjBugs != 1)
                    nextState[key] = Tile.Space;
                else if (current[key] == Tile.Space && (adjBugs == 1 || adjBugs == 2))
                    nextState[key] = Tile.Bug;
                else
                    nextState[key] = current[key];
            }

            // Add the +1 and -1 levels
            var maxLevel = current.Keys.Max(x => x.level) +1;
            var minLevel = current.Keys.Min(x => x.level) -1;

            foreach (var pos in validKeys)
            {
                var key = (maxLevel , pos);
                var adjBugs = GetAdjacentBugs(key, current);
                nextState[key] = (adjBugs == 1 || adjBugs == 2) ? Tile.Bug : Tile.Space;

                key = (minLevel , pos);
                adjBugs = GetAdjacentBugs(key, current);
                nextState[key] = (adjBugs == 1 || adjBugs == 2) ? Tile.Bug : Tile.Space;
            }

            return nextState;
        }

        public long SolvePart2()
        {
            Dictionary<(int level, Coord2D pos), char> currentState = new();

            InitialState.Keys.Where(k => validKeys.Contains(k)).ToList().ForEach(x => currentState[(0,x)] = InitialState[x]);

            for (int i = 0; i < 200; i++)
            {
                var nextState = Evolve_part2(currentState);
                nextState.Keys.ToList().ForEach(x => currentState[x] = nextState[x]);
            }
            return currentState.Values.Count(x => x== Tile.Bug);
        }
    }

    internal class BugPlanet
    {
        LifeSim sim = new();

        public void ParseInput(List<string> lines)
            => sim.LoadInitialState(lines);

        public long Solve(int part = 1)
            => part == 1 ? sim.SolvePart1() : sim.SolvePart2();
    }
}
