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
            var maxX = state.Keys.Max(k => k.x)+1;
            var powers = state.Keys.Where(k => state[k] == Tile.Bug).Select(k => k.y * maxX + k.x);
            return (long) powers.Sum(x => (long) Math.Pow(2, x) );
        }

        Dictionary<Coord2D, char> Evolve(Dictionary<Coord2D, char> current)
        {
            var nextState = new Dictionary<Coord2D, char>();

            foreach (var key in current.Keys) 
            {
                var adjBugs = key.GetNeighbors().Count(n => current.ContainsKey(n) && current[n] == Tile.Bug);

                if (current[key] == Tile.Bug && adjBugs != 1)
                    nextState[key] = Tile.Space;
                else if ( current[key] == Tile.Space && (adjBugs == 1 || adjBugs == 2))
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
    }

    internal class BugPlanet
    {
        LifeSim sim = new();

        public void ParseInput(List<string> lines)
            => sim.LoadInitialState(lines);

        public long Solve(int part = 1)
            => sim.SolvePart1();
    }
}
