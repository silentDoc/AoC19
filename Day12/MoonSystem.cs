using AoC19.Common;

namespace AoC19.Day12
{
    class Moon
    {
        public Coord3D Position = new(0, 0, 0);
        public Coord3D Velocity = new(0, 0, 0);

        public Moon(int x, int y, int z)
            => Position = new Coord3D(x, y, z);

        public void UpdateVelocity(Moon otherMoon)
        { 
            var PositionDiff = otherMoon.Position - Position;

            Velocity.x += PositionDiff.x > 0 ? 1 : (PositionDiff.x < 0 ? -1 : 0);
            Velocity.y += PositionDiff.y > 0 ? 1 : (PositionDiff.y < 0 ? -1 : 0);
            Velocity.z += PositionDiff.z > 0 ? 1 : (PositionDiff.z < 0 ? -1 : 0);
        }

        public void UpdatePosition()
            => Position += Velocity;

        int PotEnergy 
            => Math.Abs(Position.x) + Math.Abs(Position.y) + Math.Abs(Position.z);
        int KinEnergy
            => Math.Abs(Velocity.x) + Math.Abs(Velocity.y) + Math.Abs(Velocity.z);

        public int Energy
            => PotEnergy * KinEnergy;
    }


    internal class MoonSystem
    {
        List<Moon> Moons = new();

        void ParseLine(string line)
        {
            var coords = line.Substring(1, line.Length - 2).Split(",", StringSplitOptions.TrimEntries);
            var values = coords.Select(x => x.Substring(2)).Select(int.Parse).ToList();
            Moons.Add(new Moon(values[0], values[1], values[2]));
        }

        public void ParseInput(List<string> input)
            => input.ForEach(ParseLine);

        public int FindEnergy()
        {
            int steps = 1000;
            
            for (int step = 0; step < steps; step++)
            {
                HashSet<Moon> used = new();
                foreach (var moon in Moons)
                {
                    var restOfMoons = Moons.Where(x => x != moon && !used.Contains(x)).ToList();
                    foreach (var otherMoon in restOfMoons)
                    {
                        moon.UpdateVelocity(otherMoon);
                        otherMoon.UpdateVelocity(moon);
                        used.Add(moon);
                    }
                }

                Moons.ForEach(x => x.UpdatePosition());
            }
            return Moons.Sum(x => x.Energy);
        }

        public int Solve(int part = 1)
            => FindEnergy();
    }
}
