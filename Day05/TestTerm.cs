namespace AoC19.Day05
{
    internal class TestTerm
    {
        Dictionary<int, int> IntCodes = new();
        int LastOutput = 0;
        const int EXIT_PROGRAM = -9999999;

        public void ParseInput(List<string> lines)
        {
            var nums = lines[0].Split(",").Select(int.Parse).ToList();
            for (int i = 0; i < nums.Count; i++)
                IntCodes[i] = nums[i];
        }

        int RunOpCode(int Ptr, int part =1)
        {
            var opCodeSet = IntCodes[Ptr];
            var strOpCode = opCodeSet.ToString("0000");
            var opCode  = int.Parse(strOpCode.Substring(2));
            var p1Mode = int.Parse(strOpCode.Substring(1, 1));
            var p2Mode = int.Parse(strOpCode.Substring(0, 1));

            if (opCode == 99)
                return EXIT_PROGRAM;

            int v1 = IntCodes[Ptr + 1];
            int v2 = opCode < 3 || opCode > 4 ? IntCodes[Ptr + 2] : 0;  // Just in case, we don't want to step off bounds
            int v3 = opCode < 3 || opCode > 6 ? IntCodes[Ptr + 3] : 0;

            var op1 = p1Mode == 0 ? IntCodes[v1] : v1;
            int op2;

            var increment = 4;
            switch (opCode)
            {
                case 1:     // Sum / mul
                case 2:
                    op2 = p2Mode == 0 ? IntCodes[v2] : v2;
                    IntCodes[v3] = opCode == 1 ? op1 + op2 : op1 * op2;
                    break;
                case 3:     // Input
                    IntCodes[v1] = part == 1 ? 1 : 5;
                    increment = 2;
                    break;
                case 4:     // Output
                    LastOutput = op1;
                    increment = 2;
                    break;
                case 5:     // Jump if true - first param is non zero - we set the IntPtr to second param
                    if (op1 != 0)
                    {
                        op2 = p2Mode == 0 ? IntCodes[v2] : v2;
                        increment = op2 - Ptr;
                    }
                    else
                        increment = 3;
                    break;
                case 6:     // Jump if false - first param is zero - we set the IntPtr to second param
                    if (op1 == 0)
                    {
                        op2 = p2Mode == 0 ? IntCodes[v2] : v2;
                        increment = op2 - Ptr;
                    }
                    else
                        increment = 3;
                    break;
                case 7:     // Less than - first < second -- code[third] = 1 else 0
                    op2 = p2Mode == 0 ? IntCodes[v2] : v2;
                    IntCodes[v3] = (op1 < op2) ? 1 : 0;
                    break;
                case 8:     // Equals - - first == second -- code[third] = 1 else 0
                    op2 = p2Mode == 0 ? IntCodes[v2] : v2;
                    IntCodes[v3] = (op1 == op2) ? 1 : 0;
                    break;
            }
            return increment;
        }

        int RunProgram(int part)
        {
            int Ptr = 0;
            while (IntCodes.Keys.Contains(Ptr))
            {
                var increment = RunOpCode(Ptr, part);
                if (increment == EXIT_PROGRAM)
                    break;
                Ptr += increment;

            }
            return LastOutput;
        }

        public int Solve(int part)
            => RunProgram(part);
    }
}