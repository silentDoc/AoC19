using AoC19.Common;
using System.Runtime.ConstrainedExecution;

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
           => Cells.Keys.Where(x => Cells[x] > 1).Select(x => new Coord2D(0, 0).Manhattan(x)).Min();

        int MinSteps()
        {
            var crossings = Cells.Keys.Where(x => Cells[x] > 1);
            var w1Steps = crossings.Select(x => Wire1.IndexOf(x) + 1).ToList();
            var w2Steps = crossings.Select(x => Wire2.IndexOf(x) + 1).ToList();
            // There are crossings done by only 1 wire, we need to ignore positions of multiple
            var dists = w1Steps.Zip(w2Steps, (f, s) => (f!=0 && s!=0) ? f + s : 999999).ToList();
            return dists.Min();
        }

        public int Solve(int part)
            => part ==1 ? DistToOrigin() : MinSteps();
    }
}
