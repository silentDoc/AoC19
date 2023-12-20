using AoC19.Common;

namespace AoC19.Day03
{
    internal class WireMap
    {
        Dictionary<Coord2D, int> Cells = new();
        Coord2D UP = (0, -1);
        Coord2D DOWN = (0, 1);
        Coord2D RIGHT = (1, 0);
        Coord2D LEFT = (-1, 0);
        
        void ParseLine(string line)
        {
            var elements = line.Split(',', StringSplitOptions.TrimEntries).ToList();

            Coord2D current = (0, 0);
            foreach (var element in elements)
            {
                var dir = element[0] switch
                {
                    'U' => UP,
                    'D' => DOWN,
                    'L' => LEFT,
                    'R' => RIGHT,
                    _ => throw new Exception("Invalid direction")
                };
                var steps = int.Parse(element.Substring(1));

                for (int i = 0; i < steps; i++)
                {
                    current += dir;
                    if (!Cells.ContainsKey(current))
                        Cells[current] = 0;
                    Cells[current]++;
                }
            }
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        int DistToOrigin()
           => Cells.Keys.Where(x => Cells[x] > 1).Select(x => new Coord2D(0, 0).Manhattan(x)).Min();

        public int Solve(int part)
            => DistToOrigin();
    }
}
