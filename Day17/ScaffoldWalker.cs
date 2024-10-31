using AoC19.Common;
using System.Text;

namespace AoC19.Day17
{
    public static class Tile
    {
        public const int NewLine  = 10;
        public const int Scaffold = 35;  // #
        public const int Space    = 46;  // .
        public const int BotUp    = 94;  // ^
        public const int BotDown  = 118; // v
        public const int BotLeft  = 60;  // <
        public const int BotRight = 62;  // >
    }

    public static class Direction
    {
        public static Coord2D Up    = new( 0,-1);
        public static Coord2D Down  = new( 0, 1);
        public static Coord2D Left  = new(-1, 0);
        public static Coord2D Right = new( 1, 0);
    }

    class AsciiRobot
    {
        IntCodeProcessor droid = new();
        Dictionary<Coord2D, int> Map = new();

        public AsciiRobot(List<string> sourceCode)
           => droid.ParseInput(sourceCode);

        public void RetrieveMap()
        {
            int x = 0;
            int y = 0;
            while (!droid.ProgramEnded())
            {
                int mapValue = (int)droid.RunProgram(0);

                if (mapValue == Tile.NewLine)
                {
                    y++; x = 0;
                    continue;
                }

                Map[(x, y)] = mapValue;
                x++;
            }
        }

        public void PaintMap()
        {
            Console.WriteLine("");
            for (int y = 0; y <= Map.Keys.Max(k => k.y); y++)
            {
                var keys = Map.Keys.Where(k => k.y == y).OrderBy(k => k.x).ToList();
                string line = string.Concat(keys.Select(k => ((char)Map[k]).ToString()));
                Console.WriteLine(line);
            }
        }

        public int GetAlignmentValue()
        {
            RetrieveMap();
            PaintMap();
            // Find crossings - the positions where all neighbors are scaffold positions (4 connectivity)
            var scaffoldPositions = Map.Keys.Where(x => Map[x] != Tile.Space).ToList();
            var crossings = scaffoldPositions.Where(x => x.GetNeighbors().All(y => scaffoldPositions.Contains(y))).ToList();
            
            return crossings.Sum(k => k.x * k.y);
        }

        #region Methods to build route after map is retrieved
        Coord2D TurnLeft(Coord2D currentDirection)
            => currentDirection switch
            {
                (0, -1) => Direction.Left,
                (-1, 0) => Direction.Down,
                (0, 1) => Direction.Right,
                (1, 0) => Direction.Up,
                _ => throw new Exception("Invalid direction " + currentDirection.ToString())
            };

        Coord2D TurnRight(Coord2D currentDirection)
            => currentDirection switch
            {
                (0, -1) => Direction.Right,
                (-1, 0) => Direction.Up,
                (0, 1) => Direction.Left,
                (1, 0) => Direction.Down,
                _ => throw new Exception("Invalid direction " + currentDirection.ToString())
            };

        string BuildRoute()
        {
            Coord2D currentDirection = new(0, 0);
            Coord2D currentPosition = Map.Keys.Where(x => Map[x]>=Tile.BotLeft).First();

            currentDirection = Map[currentPosition] switch
            {
                Tile.BotLeft => Direction.Left,
                Tile.BotRight => Direction.Right,
                Tile.BotDown => Direction.Down,
                Tile.BotUp => Direction.Up,
                _ => throw new Exception("Invalid tile " + currentDirection.ToString())
            };

            HashSet<Coord2D> scaffoldPositions = Map.Keys.Where(x => Map[x] != Tile.Space).ToHashSet();
            StringBuilder route = new("");

            while (true)
            {
                int currentCount = 0;

                // Try to advance as much as possible in the current direction
                while (scaffoldPositions.Contains(currentPosition + currentDirection))
                {
                    currentCount++;
                    currentPosition += currentDirection;
                }

                if (currentCount > 0)
                    route.Append(currentCount.ToString() + ",");

                // Turn - Find the one that keeps us in scaffold
                var newDirL = TurnLeft(currentDirection);
                var newDirR = TurnRight(currentDirection);

                if (scaffoldPositions.Contains(currentPosition + newDirL))
                    route.Append("L,");
                else if (scaffoldPositions.Contains(currentPosition + newDirR))
                    route.Append("R,");
                else
                    break;

                currentDirection = scaffoldPositions.Contains(currentPosition + newDirL) ? newDirL : newDirR;
            }

            return route.ToString();
        }
        #endregion

        public int GetDustValue()
        {
            // This section is commented because it only needs to be run once to retrieve the full route. Once I have it, I manually build the chunks
            //RetrieveMap();
            //var route = BuildRoute();

            // L,6,R,12,L,6,L,8,L,8,L,6,R,12,L,6,L,8,L,8,L,6,R,12,R,8,L,8,L,4,L,4,L,6,L,6,R,12,R,8,L,8,L,6,R,12,L,6,L,8,L,8,L,4,L,4,L,6,L,6,R,12,R,8,L,8,L,4,L,4,L,6,L,6,R,12,L,6,L,8,L,8
            // A = L,6,R,12,L,6,L,8,L,8
            // B = L,6,R,12,R,8,L,8
            // C = L,4,L,4,L,6
            // Route = A,A,B,C,B,A,C,B,C,A
            
            droid.PatchMemory(0, 2);

            var segmentA = "L,6,R,12,L,6,L,8,L,8\n".ToArray();
            var segmentB = "L,6,R,12,R,8,L,8\n".ToArray();
            var segmentC = "L,4,L,4,L,6\n".ToArray();
            var allRoute = "A,A,B,C,B,A,C,B,C,A\n".ToArray();
            var vidFeed = "n\n".ToArray();

            var inputs = allRoute
                        .Concat(segmentA)
                        .Concat(segmentB)
                        .Concat(segmentC)
                        .Concat(vidFeed)
                        .Select(c => (int) c)
                        .ToArray();

            var output = 0;
            foreach (var input in inputs)
                droid.AddInputToQueue(input);

            // Make the program finish
            while(!droid.ProgramEnded())
                output = (int)droid.RunProgram(0); 

            return output;
        }
    }

    public class ScaffoldWalker
    {
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        public int GetValue(int part = 1)
        {
            AsciiRobot asciiBot = new(sourceCode);
            return part ==1 ? asciiBot.GetAlignmentValue() : asciiBot.GetDustValue();
        }

        public long Solve(int part = 1)
            => GetValue(part);
    }
}
