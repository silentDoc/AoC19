using AoC19.Common;

namespace AoC19.Day19
{
    class TractorBeamDroid
    {
        IntCodeProcessor droid = new();

        public TractorBeamDroid(List<string> sourceCode)
            => droid.ParseInput(sourceCode);

        public int ActivePositions()
        {
            int activePositions = 0;

            for (int x = 0; x < 50; x++)
                for (int y = 0; y < 50; y++)
                {
                    droid.ResetProgram();
                    droid.AddInputToQueue(x);
                    droid.AddInputToQueue(y);
                    var output = droid.RunProgram(0);
                    if (output == 1)
                        activePositions++;
                }

            return activePositions;
        }
    }

    internal class BeamChecker
    {
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        public int GetValue(int part = 1)
        {
            TractorBeamDroid beamDroid = new(sourceCode);
            return beamDroid.ActivePositions();
        }

        public long Solve(int part = 1)
            => GetValue(part);
    }
}
