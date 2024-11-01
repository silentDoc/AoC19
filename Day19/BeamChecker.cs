using AoC19.Common;

namespace AoC19.Day19
{
    class TractorBeamDroid
    {
        IntCodeProcessor droid = new();

        public TractorBeamDroid(List<string> sourceCode)
            => droid.ParseInput(sourceCode);

        int CheckPosition(int x, int y)
        {
            droid.ResetProgram();
            droid.AddInputToQueue(x);
            droid.AddInputToQueue(y);
            return (int) droid.RunProgram(0);
        }

        public int ActivePositions()
        {
            int activePositions = 0;

            for (int x = 0; x < 50; x++)
                for (int y = 0; y < 50; y++)
                    if (CheckPosition(x, y) == 1)
                        activePositions++;
            return activePositions;
        }

        bool CheckSquare(int x, int y, int w)
            => CheckPosition(x, y) == 1 && CheckPosition(x + w, y) == 1 &&
               CheckPosition(x, y + w) == 1 && CheckPosition(x + w, y + w) == 1;

        // For a given row, we find x0 and x1 being the start and end x coords
        // of the tractor beam activity zone. Also doing binary search
        (int x0, int x1) GetBeamInRow(int y)
        {
            var xLow = 0;
            var xHigh = y/2;

            while (xHigh - xLow > 1)
            { 
                var avg = (xHigh + xLow) / 2;
                var value = CheckPosition(avg, y);
                xLow = (value ==1) ? xLow : avg;
                xHigh = (value == 1) ? avg : xHigh;
            }

            var x0 = xHigh;

            xLow = y / 2;
            xHigh = y;

            while (xHigh - xLow > 1)
            {
                var avg = (xHigh + xLow) / 2;
                var value = CheckPosition(avg, y);
                xLow = (value == 1) ? avg : xLow;
                xHigh = (value == 1) ? xHigh : avg;
            }

            var x1 = xLow;

            return (x0, x1);
        }

        public int FindSantaShip()
        {
            // Implement some sort of binary search. 
            var y_low = 100;
            var y_high = 10000;
            var x1 = 0;
            while (y_high - y_low > 1)
            {
                
                var avg = (y_high + y_low) / 2;
                (_, x1) = GetBeamInRow(avg);
                var squareFits = CheckSquare(x1 - 99, avg, 99);
                y_high = squareFits ? avg : y_high;
                y_low = squareFits ? y_low : avg;
            }

            return ((x1-99) * 10000) + y_high;
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
            return part == 1 ? beamDroid.ActivePositions() : beamDroid.FindSantaShip();
        }

        public long Solve(int part = 1)
            => GetValue(part);
    }
}
