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

    internal class Asteroid
    {
        Coord2D Position        = new(0, 0);
        Coord2D StationPosition = new(0, 0);

        Coord2D normalizedPosition = new(0, 0);

        public Asteroid(Coord2D pos, Coord2D stationPos)
        {
            Position = pos;
            StationPosition = stationPos;

            normalizedPosition = pos - stationPos;
        }

        public double Angle
            => normalizedPosition.GetAngle();

        public double Dist
            => normalizedPosition.VectorModule;
    }

    internal class StationLocator
    {
        List<Coord2D> asteroids = new();
        Coord2D stationLocation = new(0,0);
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
            var maxSeen = seen.Max();
            var index = seen.IndexOf(maxSeen);
            stationLocation = asteroids[index];
            
            return maxSeen;
        }

        int Vaporize()
        {
            FindBestLocation();
            // now stationLocation holds the location of the laser
            var others = asteroids.Where(x => x != stationLocation).ToList();
            others.ForEach(x => x -= stationLocation);
            
            var remaining = new List<Coord2D>();
            others.ForEach(x => remaining.Add(x));
            Coord2D vaporized = new Coord2D(-1, -1);

            bool mustSort = true;
            
            Console.WriteLine("Station Location = " + stationLocation.ToString());

            // We have station location become our center of axis (all points relative to it) so that we can find the angles easily
            for (int i = 0; i < 200; i++)
            {

                if (mustSort)
                {
                    others = others.OrderBy(x => x.GetAngle())
                                   .ThenBy(x => x.VectorModule).ToList();
                    mustSort = false;
                }
                
                vaporized = others[0];
                var currentAngle = vaporized.GetAngle();

                Console.WriteLine("Order " + i.ToString() + " - Loc : " + (vaporized + stationLocation).ToString());

                remaining.Remove(vaporized);
                others = others.Where(x => x.GetAngle() != currentAngle).ToList();

                if (others.Count == 0)
                {
                    remaining.ForEach(x => others.Add(x));
                    mustSort = true;
                }
            }
            

            vaporized += stationLocation;   // Restore the original coordinates
            return vaporized.x * 100 + vaporized.y;
        }

        public int Solve(int part = 1)
            => part == 1 ? FindBestLocation() : Vaporize();
    }
}
