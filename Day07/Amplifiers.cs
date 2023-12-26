namespace AoC19.Day07
{
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
    }

    internal class Amplifiers
    {
        TestTerm Program = new();
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => Program.ParseInput(sourceCode = lines);

        int FindSignals()
        {
            int maxSignal = -1;

            for (int i = 0; i < 5; i++)
             for (int j = 0; j < 5; j++)
              for (int k = 0; k < 5; k++)
               for (int l = 0; l < 5; l++)
                for (int m = 0; m < 5; m++)
                {
                    var vs = new List<int> { i, j, k, l, m };
                    if (vs.Distinct().Count()<5)
                        continue;

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

        public int Solve(int part = 1)
            => FindSignals();
    }
}
