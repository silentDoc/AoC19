namespace AoC19.Day01
{
    internal class RocketEquation
    {
        List<int> masses = new List<int>();

        public void ParseInput(List<string> lines)
            => lines.ForEach(line => masses.Add(int.Parse(line)));

        int Module(int mass)
            => (mass / 3) - 2;

        public int Solve(int part = 1)
            => part == 1 ? masses.Select(x => Module(x)).Sum() : 0;
    }
}
