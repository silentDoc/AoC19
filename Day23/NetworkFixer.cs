using AoC19.Common;

namespace AoC19.Day23
{
    class RobotNetwork
    {
        IntCodeProcessorv2[] droids;
        int numDroids = 50;

        public RobotNetwork()
            => droids = new IntCodeProcessorv2[numDroids];

        public void StartUp(List<string> sourceCode)
        {
            for (int i = 0; i < droids.Length; i++)
            {
                droids[i] = new IntCodeProcessorv2();
                droids[i].ParseInput(sourceCode);
                droids[i].AddInputToQueue(i);
            }
        }

        public long SolvePart1()
        {
            bool addressFound = false;
            long result = -1;
            long outPutAddress, x, y;

            while (!addressFound)
            {
                for (int i = 0; i < droids.Length; i++)
                {
                    if (droids[i].inputBuffer.Count ==0)
                        droids[i].AddInputToQueue(-1);

                    droids[i].RunProgram();

                    while (droids[i].outputBuffer.Count > 2)
                    {
                        outPutAddress = droids[i].outputBuffer.Dequeue();
                        x = droids[i].outputBuffer.Dequeue();
                        y = droids[i].outputBuffer.Dequeue();

                        if (outPutAddress == 255)
                        {
                            addressFound = true;
                            result = y;
                            break;
                        }
                        
                        droids[outPutAddress].AddInputToQueue(x);
                        droids[outPutAddress].AddInputToQueue(y);
                    }

                    if (addressFound)
                        break;
                }
            }
            return result;
        }
    }

    internal class NetworkFixer
    {
        RobotNetwork robots = new();

        public void ParseInput(List<string> lines)
            => robots.StartUp(lines);

        public long Solve(int part = 1)
            => robots.SolvePart1();
    }
}
