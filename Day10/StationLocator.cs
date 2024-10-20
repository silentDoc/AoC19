using AoC19.Common;

namespace AoC19.Day10
{
    internal class StraightLine
    {
        // ax + by + c = 0
        double a,b,c;

        public StraightLine(Coord2D point1, Coord2D point2)
        { 
            a = point2.y - point1.y;
            b = point1.x - point2.x;
            c = point1.y * (point2.x - point1.x) - point1.x * (point2.y - point1.y);
        }

        public bool CheckPoint(Coord2D point)
            => a * point.x + b * point.y + c == 0; 
    }

    internal class StationLocator
    {
        List<Coord2D> asteroids = new();

        public void ParseInput(List<string> lines)
        {
            for (int row = 0; row < lines.Count; row++)
                for (int col = 0; col < lines[row].Length; col++)
                    if (lines[row][col] == '#')
                        asteroids.Add(new Coord2D(col, row));
        }
        
        int HowManyInLine(Coord2D stationCandidate, Coord2D asteroid)
        {
            // Only consider the ones in the quadrant defined by the two asteroids
            var line = new StraightLine(stationCandidate, asteroid);
            var rest = asteroids.Where(x => x!=stationCandidate && x!= asteroid && x.IsInside(stationCandidate, asteroid));
            return rest.Count(x => line.CheckPoint(x));
        }

        int FindBestLocation()
        {
            List<int> seen = new();

            foreach (var candidate in asteroids)
            { 
                var others = asteroids.Where(x => x != candidate);
                var clearlySeen = others.Count(x => HowManyInLine(candidate, x) == 0);
                seen.Add(clearlySeen);
            }

            return seen.Max();
        }

        public int Solve(int part = 1)
            => FindBestLocation();
    }
}
