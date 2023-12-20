using AoC19.Common;

namespace AoC19.Day03
{
    internal class WireMap
    {
        Dictionary<Coord2D, int> Cells = new();
        List<Coord2D> Wire1 = new();
        List<Coord2D> Wire2 = new();

        Coord2D UP = (0, -1);
        Coord2D DOWN = (0, 1);
        Coord2D RIGHT = (1, 0);
        Coord2D LEFT = (-1, 0);

        int wire = 1;
        
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

                    if (wire == 1)
                        Wire1.Add(current);
                    else
                        Wire2.Add(current);
                }
            }
            wire++;
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        int DistToOrigin()
           => Cells.Keys.Where(x => Cells[x] > 1).Min(x => new Coord2D(0, 0).Manhattan(x));

        int MinSteps()
            =>  Cells.Keys.Where(x => Cells[x] > 1)
                          .Intersect(Wire1)
                          .Intersect(Wire2)
                          .Min(x => Wire1.IndexOf(x) + Wire2.IndexOf(x) + 2);  // Index +1 (zero based) for both wires -- +2
        public int Solve(int part)
            => part ==1 ? DistToOrigin() : MinSteps();
    }
}
