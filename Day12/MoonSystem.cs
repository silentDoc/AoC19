using AoC19.Common;
using System.Text;

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

        public string State(int coordNum)   // Part 2
            => coordNum switch
            {
                0 => Position.x.ToString() + "," + Velocity.x.ToString() + ";;",
                1 => Position.y.ToString() + "," + Velocity.y.ToString() + ";;",
                2 => Position.z.ToString() + "," + Velocity.z.ToString() + ";;",
                _ => throw new Exception("Invalid coordNum passed : " + coordNum.ToString())
            };
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

        string GetState(int coordNum)   // Generates a string that identifies a state
        {
            StringBuilder sb = new();
            Moons.ForEach(x => sb.Append(x.State(coordNum)));
            return sb.ToString();
        }

        public long FindRepeatedState()
        {
            // It takes forever, the trick here is to see that the X, Y, Z Coordinates are completely unrelated
            // We can see their independent repeat cycle and work out the system's using the LCM of them

            HashSet<string> statesX = new();
            HashSet<string> statesY = new();
            HashSet<string> statesZ = new();
            long steps = 0;
            
            long stepsX = -1;
            long stepsY = -1;
            long stepsZ = -1;

            bool found = false;

            statesX.Add(GetState(0));
            statesY.Add(GetState(1));
            statesZ.Add(GetState(2));

            while (!found)
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
                steps++;

                if (stepsX == -1 && !statesX.Add(GetState(0)))
                    stepsX = steps;
                if (stepsY == -1 && !statesY.Add(GetState(1)))
                    stepsY = steps;
                if (stepsZ == -1 && !statesZ.Add(GetState(2)))
                    stepsZ = steps;

                found = stepsX != -1 && stepsY != -1 && stepsZ != -1;
            }
            return MathHelper.LCM( [stepsX, stepsY, stepsZ] );
        }

        public long Solve(int part = 1)
            => part == 1 ? FindEnergy() : FindRepeatedState();
    }
}
