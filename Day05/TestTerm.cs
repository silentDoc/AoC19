namespace AoC19.Day05
{
    internal class TestTerm
    {
        Dictionary<int, int> IntCodes = new();
        int LastOutput = 0;

        public void ParseInput(List<string> lines)
        {
            var nums = lines[0].Split(",").Select(int.Parse).ToList();
            for (int i = 0; i < nums.Count; i++)
                IntCodes[i] = nums[i];
        }

        int RunOpCode(int Ptr)
        {
            var opCodeSet = IntCodes[Ptr];
            var strOpCode = opCodeSet.ToString("0000");
            var opCode  = int.Parse(strOpCode.Substring(2));
            var p1Mode = int.Parse(strOpCode.Substring(1, 1));
            var p2Mode = int.Parse(strOpCode.Substring(0, 1));

            int v1 = IntCodes[Ptr + 1];
            int v2 = IntCodes[Ptr + 2];
            int v3 = IntCodes[Ptr + 3];

            if (opCode == 99)
                return -1;

            var op1 = p1Mode == 0 ? IntCodes[v1] : v1;

            var increment = -1;
            switch (opCode)
            {
                case 1:
                case 2:
                    var op2 = p2Mode == 0 ? IntCodes[v2] : v2;
                    IntCodes[v3] = opCode == 1 ? op1 + op2
                                               : op1 * op2;
                    increment = 4;
                    break;
                case 3:
                    IntCodes[v1] = 1;
                    increment = 2;
                    break;
                case 4:
                    LastOutput = op1;
                    increment = 2;
                    break;
            }
            return increment;
        }

        int RunProgram()
        {
            int Ptr = 0;
            while (IntCodes.Keys.Contains(Ptr))
            {
                var increment = RunOpCode(Ptr);
                if (increment == -1)
                    break;
                Ptr += increment;

            }
            return LastOutput;
        }

        public int Solve(int part)
            => part == 1 ? RunProgram() : 0;
    }
}
