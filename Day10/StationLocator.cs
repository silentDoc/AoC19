using AoC19.Common;
using System.Net.Security;

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
        public Coord2D Position        = new(0, 0);
        public Coord2D StationPosition = new(0, 0);
        Coord2D normalizedPosition = new(0, 0);

        double angle = -999999;         // Memoization
        double distance = -999999;

        public Asteroid(Coord2D pos, Coord2D stationPos)
        {
            Position = pos;
            StationPosition = stationPos;

            normalizedPosition = pos - stationPos;  // Make our station our 0,0
        }

        public double Angle
            => (angle == -999999) ? angle = normalizedPosition.GetAngle() : angle;

        public double Distance
            => (distance == -999999) ? distance = normalizedPosition.VectorModule : distance;

        public override string ToString()
            => "Position = (" + Position.ToString() + ") , Angle = " + Angle.ToString() + " , Distance = " + Distance.ToString();
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
            stationLocation = asteroids[index]; // For part 2
            
            return maxSeen;
        }

        int Vaporize()
        {
            FindBestLocation();
            // now stationLocation holds the location of the laser
            var others = asteroids.Where(x => x != stationLocation).ToList();

            // For part 2 I created an Asteroid class to help with normalization, angles and distances
            List<Asteroid> asteroidList = new List<Asteroid>();
            others.ForEach(x => asteroidList.Add(new Asteroid(x, stationLocation)));
            asteroidList = asteroidList.OrderBy(x => x.Angle)
                                       .ThenBy(x => x.Distance)
                                       .ToList();

            List<Asteroid> asteroidList_interest = new List<Asteroid>();
            asteroidList.ForEach(asteroidList_interest.Add);

            Asteroid vaporized = new(new(0, 0), stationLocation);

            bool goneAround = false;

            // We have station location become our center of axis (all points relative to it) so that we can find the angles easily
            for (int i = 0; i < 200; i++)
            {
                if (goneAround)
                {
                    // If we do a full turn, we reset the list of interest with the remaining asteroids
                    asteroidList.ForEach(asteroidList_interest.Add);
                    goneAround = false;
                }
                
                vaporized = asteroidList_interest[0];  // Destroy the first -closest
                asteroidList.Remove(vaporized);
                // Consider only the ones that do not have the same angle of the vaporized
                asteroidList_interest = asteroidList_interest.Where(x => x.Angle != vaporized.Angle).ToList();

                if (asteroidList_interest.Count == 0)
                    goneAround = true;
            }
            
            return vaporized.Position.x * 100 + vaporized.Position.y;
        }

        public int Solve(int part = 1)
            => part == 1 ? FindBestLocation() : Vaporize();
    }
}
