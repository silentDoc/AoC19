﻿using AoC19.Common;
using System.Collections;
using System.Text;

namespace AoC19.Day25
{
    class ShipRobot
    {
        IntCodeProcessorv2 droid;

        public ShipRobot()
            => droid = new IntCodeProcessorv2();

        public void StartUp(List<string> sourceCode)
            => droid.ParseInput(sourceCode);

        List<List<string>> GetCombinations(List<string> objects, int num)
        {
            int count = 1 << objects.Count; // 2^n
            List<List<string>> retVal = new();

            for (int i = 0; i < count; i++)
            {
                BitArray b = new BitArray(BitConverter.GetBytes(i));
                var bitCount = 0;

                for (int bitc = 0; bitc < objects.Count; bitc++)
                    if (b[bitc])
                        bitCount++;

                if (bitCount != num)
                    continue;
                
                List<string> combination = new();

                for (int bit = 0; bit < objects.Count; bit++)
                {
                    if(b[bit])
                        combination.Add(objects[bit]);
                }
                retVal.Add(combination);
            }
            return retVal;
        }

        private void Play()
        {
            // Automation commands to navigate the map and take the elements - manually explored
            List<string> commands = ["west", "take fixed point", "north", "take sand", "south", "east", "east", "take asterisk", "north", "north", "take hypercube", "north",
                                     "take coin", "north", "take easter egg", "south", "south","south","west", "north", "take spool of cat6", "north", "take shell", "west" ];
            int cachedCommand = 0;
            var foundAll = false;

            while (!foundAll)
            {
                droid.RunProgram();
                StringBuilder sb = new("");
                while (droid.outputBuffer.Count != 0)
                {
                    var outChar = (char)((int) droid.outputBuffer.Dequeue());
                    sb.Append(outChar.ToString());
                }

                
                Console.WriteLine(sb.ToString());

                string input = (cachedCommand < commands.Count) ? commands[cachedCommand] : Console.ReadLine();
                cachedCommand++;

                if (cachedCommand == commands.Count)
                    foundAll = true;

                var listInputs = input.Select(Convert.ToInt64).Append('\n').ToArray();

                foreach(var inp in listInputs)
                    droid.AddInputToQueue(inp);
            }

            // Second part, find combinations
            List<string> objects = ["easter egg", "sand", "fixed point", "coin", "spool of cat6", "shell", "hypercube", "asterisk"];

            // Before bruteforcing, we drop everything
            for (int i = 0; i < objects.Count; i++)
            {
                var input = "drop " + objects[i];
                var listInputs = input.Select(Convert.ToInt64).Append('\n').ToArray();
                foreach (var inp in listInputs)
                    droid.AddInputToQueue(inp);
                droid.RunProgram();
                StringBuilder sb = new("");
                while (droid.outputBuffer.Count != 0)
                {
                    var outChar = (char)((int)droid.outputBuffer.Dequeue());
                    sb.Append(outChar.ToString());
                }
                Console.WriteLine(sb.ToString());
            }

            var numElements = 0;
            var entered = false;

            while (!entered)
            {
                numElements++;
                var combs = GetCombinations(objects, numElements);
                foreach (var combination in combs)
                {
                    // Take the items
                    List<string> takeItemsCommand = new();
                    List<string> dropItemsCommand = new();

                    foreach (var item in combination)
                    {
                        takeItemsCommand.Add("take " + item);
                        dropItemsCommand.Add("drop " + item);
                    }

                    for (int i = 0; i < takeItemsCommand.Count; i++)
                    {
                        var input2 = takeItemsCommand[i];
                        var listInputs2 = input2.Select(Convert.ToInt64).Append('\n').ToArray();
                        foreach (var inp in listInputs2)
                            droid.AddInputToQueue(inp);
                        droid.RunProgram();
                        StringBuilder sb2 = new("");
                        while (droid.outputBuffer.Count != 0)
                        {
                            var outChar = (char)((int)droid.outputBuffer.Dequeue());
                            sb2.Append(outChar.ToString());
                        }
                        Console.WriteLine(sb2.ToString());
                    }

                    // Attempt to go north
                    var input = "north";
                    var listInputs = input.Select(Convert.ToInt64).Append('\n').ToArray();
                    foreach (var inp in listInputs)
                        droid.AddInputToQueue(inp);
                    droid.RunProgram();
                    StringBuilder sb = new("");
                    while (droid.outputBuffer.Count != 0)
                    {
                        var outChar = (char)((int)droid.outputBuffer.Dequeue());
                        sb.Append(outChar.ToString());
                    }
                    var response = sb.ToString();
                    Console.WriteLine(response);

                    entered = response.IndexOf("heavier") == -1 && response.IndexOf("lighter") == -1;
                    if (entered)
                        break;

                    // Drop items
                    for (int i = 0; i < dropItemsCommand.Count; i++)
                    {
                        var input3 = dropItemsCommand[i];
                        var listInputs3 = input3.Select(Convert.ToInt64).Append('\n').ToArray();
                        foreach (var inp in listInputs3)
                            droid.AddInputToQueue(inp);
                        droid.RunProgram();
                        StringBuilder sb3 = new("");
                        while (droid.outputBuffer.Count != 0)
                        {
                            var outChar = (char)((int)droid.outputBuffer.Dequeue());
                            sb3.Append(outChar.ToString());
                        }
                        Console.WriteLine(sb3.ToString());
                    }
                }
            }
        }

        public long SolvePart1()
        {
            Play();
            return 1;
        }
        

        public long SolvePart2()
            => 2;
    }
    internal class ShipExplorer
    {
        ShipRobot robot = new();

        public void ParseInput(List<string> lines)
            => robot.StartUp(lines);

        public long Solve(int part = 1)
            => part == 1 ? robot.SolvePart1() : robot.SolvePart2();
    }
}
