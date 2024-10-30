using AoC19.Common;

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

        public int GetAlignmentValue()
        {
            RetrieveMap();
            var ScaffoldPositions = Map.Keys.Where(x => Map[x] != Tile.Space).ToList();
            int alignmentValue = 0;

            // Find crossings
            foreach (var pos in ScaffoldPositions)
            { 
                var neighbors = pos.GetNeighbors().Where(x => Map.ContainsKey(x)).ToList();
                if (neighbors.All(x => Map[x] == Tile.Scaffold))
                    alignmentValue += pos.x * pos.y;
            }
            return alignmentValue;
        }
    }

    public class ScaffoldWalker
    {
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        public int GetAlignmentValue(int part = 1)
        {
            AsciiRobot asciiBot = new(sourceCode);
            return asciiBot.GetAlignmentValue();
        }

        public long Solve(int part = 1)
            => GetAlignmentValue(part);
    }
}
