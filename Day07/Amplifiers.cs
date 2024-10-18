namespace AoC19.Day07
{

    public enum TermState
    { 
        RUNNING =0, 
        WAITING =1, 
        HALTED =2
    }

    internal class TestTerm
    {
        Dictionary<int, int> IntCodes = new();
        int LastOutput = 0;

        const int EXIT_PROGRAM = -9999999;
        bool firstInputEntered = false;

        public void ParseInput(List<string> lines)
        {
            IntCodes.Clear();
            firstInputEntered = false;
            var nums = lines[0].Split(",").Select(int.Parse).ToList();
            for (int i = 0; i < nums.Count; i++)
                IntCodes[i] = nums[i];
        }

        int RunOpCode(int Ptr, int firstInput, int secondInput)
        {
            var opCodeSet = IntCodes[Ptr];
            var strOpCode = opCodeSet.ToString("0000");
            var opCode = int.Parse(strOpCode.Substring(2));
            var p1Mode = int.Parse(strOpCode.Substring(1, 1));
            var p2Mode = int.Parse(strOpCode.Substring(0, 1));

            if (opCode == 99)
                return EXIT_PROGRAM;

            int v1 = IntCodes[Ptr + 1];
            int v2 = opCode < 3 || opCode > 4 ? IntCodes[Ptr + 2] : EXIT_PROGRAM;  // Just in case, we don't want to step off bounds
            int v3 = opCode < 3 || opCode > 6 ? IntCodes[Ptr + 3] : EXIT_PROGRAM;  // EXIT_PROGRAM just to have an identifiable value

            var op1 = p1Mode == 0 ? IntCodes[v1] : v1;
            int op2 = v2 != EXIT_PROGRAM ? (p2Mode == 0 ? IntCodes[v2] : v2) : EXIT_PROGRAM;

            var increment = 4;
            switch (opCode)
            {
                case 1:     // Sum / mul
                case 2:
                    IntCodes[v3] = opCode == 1 ? op1 + op2 : op1 * op2;
                    break;
                case 3:     // Input
                    IntCodes[v1] = !firstInputEntered ? firstInput : secondInput;
                    firstInputEntered = true;
                    increment = 2;
                    break;
                case 4:     // Output
                    LastOutput = op1;
                    increment = 2;
                    break;
                case 5:
                case 6:    // Jumps
                    var cond_jz_jnz = opCode == 5 ? (op1 != 0) : (op1 == 0);
                    if (cond_jz_jnz)
                    {
                        increment = op2 - Ptr;
                    }
                    else
                        increment = 3;
                    break;
                case 7:
                case 8:    // Less than - Equals
                    var cond_lt_eq = opCode == 7 ? (op1 < op2) : (op1 == op2);
                    IntCodes[v3] = cond_lt_eq ? 1 : 0;
                    break;
            }
            return increment;
        }

        public int RunProgram(int firstInput, int SecondInput)
        {
            int Ptr = 0;
            while (IntCodes.Keys.Contains(Ptr))
            {
                var increment = RunOpCode(Ptr, firstInput, SecondInput);
                if (increment == EXIT_PROGRAM)
                    break;
                Ptr += increment;
            }

            return LastOutput;
        }

        public int RunProgramPart2(int firstInput, int SecondInput)
        {
            firstInputEntered = true;

            int Ptr = 0;
            while (IntCodes.Keys.Contains(Ptr))
            {
                var increment = RunOpCode(Ptr, firstInput, SecondInput);
                if (increment == EXIT_PROGRAM)
                    break;
                Ptr += increment;

            }
            return LastOutput;
        }
    }

    internal class TestTerm2
    {
        Dictionary<int, int> IntCodes = new();
        public int LastOutput = 0;
        const int EXIT_PROGRAM = -9999999;
        const int NO_INPUT = -8888888;
        bool firstInputEntered = false;

        // Part 2
        public Queue<int> inputBuffer = new Queue<int>();
        public TestTerm2 nextTerm = null;
        public TermState State = TermState.RUNNING;
        int Ptr = 0;    // Instruction pointer
        int phase = 0;
        bool phaseSet = false;


        public TestTerm2(int phaseNum)
            => phase = phaseNum;

        public void ReceiveInput(int input)
            => inputBuffer.Enqueue(input);

        public void ParseInput(List<string> lines)
        {
            IntCodes.Clear();
            firstInputEntered = false;
            var nums = lines[0].Split(",").Select(int.Parse).ToList();
            for (int i = 0; i < nums.Count; i++)
                IntCodes[i] = nums[i];
        }

        int RunOpCode(int Ptr)
        {
            var opCodeSet = IntCodes[Ptr];
            var strOpCode = opCodeSet.ToString("0000");
            var opCode = int.Parse(strOpCode.Substring(2));
            var p1Mode = int.Parse(strOpCode.Substring(1, 1));
            var p2Mode = int.Parse(strOpCode.Substring(0, 1));

            if (opCode == 99)
                return EXIT_PROGRAM;

            int v1 = IntCodes[Ptr + 1];
            int v2 = opCode < 3 || opCode > 4 ? IntCodes[Ptr + 2] : EXIT_PROGRAM;  // Just in case, we don't want to step off bounds
            int v3 = opCode < 3 || opCode > 6 ? IntCodes[Ptr + 3] : EXIT_PROGRAM;  // EXIT_PROGRAM just to have an identifiable value

            var op1 = p1Mode == 0 ? IntCodes[v1] : v1;
            int op2 = v2 != EXIT_PROGRAM ? (p2Mode == 0 ? IntCodes[v2] : v2) : EXIT_PROGRAM;

            var increment = 4;
            switch (opCode)
            {
                case 1:     // Sum / mul
                case 2:
                    IntCodes[v3] = opCode == 1 ? op1 + op2 : op1 * op2;
                    break;
                case 3:     // Input
                    if (!phaseSet)
                    {
                        IntCodes[v1] = phase;
                        phaseSet = true;
                    }
                    else
                    {
                        // Gotta check if we have inputs
                        if (inputBuffer.Count == 0)
                            return NO_INPUT;
                        IntCodes[v1] = inputBuffer.Dequeue();
                    }
                    increment = 2;
                    break;
                case 4:     // Output
                    LastOutput = op1;
                    nextTerm.ReceiveInput(LastOutput);
                    increment = 2;
                    break;
                case 5:
                case 6:    // Jumps
                    var cond_jz_jnz = opCode == 5 ? (op1 != 0) : (op1 == 0);
                    if (cond_jz_jnz)
                    {
                        increment = op2 - Ptr;
                    }
                    else
                        increment = 3;
                    break;
                case 7:
                case 8:    // Less than - Equals
                    var cond_lt_eq = opCode == 7 ? (op1 < op2) : (op1 == op2);
                    IntCodes[v3] = cond_lt_eq ? 1 : 0;
                    break;
            }
            return increment;
        }

        public int RunProgramPart2()
        {
            while (IntCodes.Keys.Contains(Ptr))
            {
                State = TermState.RUNNING;
                var increment = RunOpCode(Ptr);
                if (increment == EXIT_PROGRAM)
                {
                    State = TermState.HALTED;
                    break;
                }

                if (increment == NO_INPUT)
                {
                    State = TermState.WAITING;
                    break;
                }

                Ptr += increment;

            }
            return LastOutput;
        }
    }


    internal class Amplifiers
    {
        TestTerm Program = new();
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => Program.ParseInput(sourceCode = lines);

        List<List<int>> getAllPerms(int minValue, int maxValue)
        {
            List<List<int>> retVal = new();

            for (int i = minValue; i < maxValue + 1; i++)
                for (int j = minValue; j < maxValue + 1; j++)
                    for (int k = minValue; k < maxValue + 1; k++)
                        for (int l = minValue; l < maxValue + 1; l++)
                            for (int m = minValue; m < maxValue + 1; m++)
                            {
                                var vs = new List<int> { i, j, k, l, m };
                                if (vs.Distinct().Count() < 5)
                                    continue;
                                retVal.Add(vs);
                            }
            return retVal;
        }

        int FindSignals()   // Part 1
        {
            int maxSignal = -1;
            var perms = getAllPerms(0, 4);

            foreach (var vs in perms)
            { 
                var outPutA = Program.RunProgram(vs[0], 0);
                Program.ParseInput(sourceCode); // Reset for AmpB
                var outPutB = Program.RunProgram(vs[1], outPutA);
                Program.ParseInput(sourceCode); // Reset for AmpC
                var outPutC = Program.RunProgram(vs[2], outPutB);
                Program.ParseInput(sourceCode); // Reset for AmpD
                var outPutD = Program.RunProgram(vs[3], outPutC);
                Program.ParseInput(sourceCode); // Reset for AmpE
                var outPutE = Program.RunProgram(vs[4], outPutD);
                Program.ParseInput(sourceCode); // Reset for next iteration

                maxSignal = (outPutE > maxSignal) ? outPutE : maxSignal;
            }

            return maxSignal;
        }

        int FindSignalsPart2()  // Part 2
        {
            int maxSignal = -1;
            var perms = getAllPerms(5, 9);

            foreach (var vs in perms)
            {
                Console.WriteLine(string.Concat(vs.Select(x => x.ToString())));
                // Prepare the terms and connect them
                List<TestTerm2> amplifiers =
                [
                    new(vs[0]),
                    new(vs[1]),
                    new(vs[2]),
                    new(vs[3]),
                    new(vs[4])
                ];

                for(int i=0; i<5;i++)
                    amplifiers[i].ParseInput(sourceCode);

                for (int i=0; i < 4;i++)
                    amplifiers[i].nextTerm = amplifiers[i+1];
                amplifiers[4].nextTerm = amplifiers[0];

                amplifiers[0].ReceiveInput(0);

                while (amplifiers[4].State != TermState.HALTED)
                {
                    foreach (var a in amplifiers)
                        a.RunProgramPart2();
                }
                var outPutE = amplifiers[4].LastOutput;
                
                maxSignal = (outPutE > maxSignal) ? outPutE : maxSignal;
            }
            return maxSignal;
        }

        public int Solve(int part = 1)
            => part == 1 ? FindSignals() : FindSignalsPart2();
    }
}
