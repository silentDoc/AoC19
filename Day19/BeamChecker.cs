using AoC19.Common;

namespace AoC19.Day19
{
     
    class TractorBeamDroid
    {
        IntCodeProcessor droid = new();

        public TractorBeamDroid(List<string> sourceCode)
            => droid.ParseInput(sourceCode);

        int ActivePositions()
        { 
            
        }



    }

    internal class BeamChecker
    {
        List<string> sourceCode = new();
        TractorBeamDroid droid;

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        public int GetValue(int part = 1)
        {
            TractorBeamDroid beamDroid = new(sourceCode);
            return beamDroid.ActivePositions()
        }

        public long Solve(int part = 1)
            => GetValue(part);
    }
}
