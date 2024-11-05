using AoC19.Common;
using System.Diagnostics;

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

        public long SolvePart2()
        {
            bool addressFound = false;
            long result = -1;
            long outPutAddress, x, y;
            long nat_x = 0, nat_y = 0;
            long last_nat_y = -1;

            while (!addressFound)
            {
                if (droids.All(x => x.inputBuffer.Count == 0))
                {
                    if (last_nat_y == nat_y)
                    {
                        addressFound = true;
                        result = nat_y;
                        break;
                    }
                    droids[0].AddInputToQueue(nat_x);
                    droids[0].AddInputToQueue(nat_y);
                    last_nat_y = nat_y;
                }


                for (int i = 0; i < droids.Length; i++)
                {
                    if (droids[i].inputBuffer.Count == 0)
                        droids[i].AddInputToQueue(-1);

                    droids[i].RunProgram();

                    while (droids[i].outputBuffer.Count > 2)
                    {
                        outPutAddress = droids[i].outputBuffer.Dequeue();
                        x = droids[i].outputBuffer.Dequeue();
                        y = droids[i].outputBuffer.Dequeue();

                        if (outPutAddress == 255)
                        {
                            nat_x = x;
                            nat_y = y;
                        }
                        else
                        {
                            droids[outPutAddress].AddInputToQueue(x);
                            droids[outPutAddress].AddInputToQueue(y);
                        }
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
            => part == 1 ? robots.SolvePart1(): robots.SolvePart2();
    }
}
