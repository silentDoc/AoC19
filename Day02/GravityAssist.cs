namespace AoC19.Day02
{
    
    internal class GravityAssist
    {
        Dictionary<int, int> IntCodes = new();

        public void ParseInput(List<string> lines)
        {
            var nums = lines[0].Split(",").Select(int.Parse).ToList();
            for (int i = 0; i < nums.Count; i++)
                IntCodes[i] = nums[i];
        }

        bool RunOpCode(int Ptr)
        {
            if (IntCodes[Ptr] == 99)
                return false;

            var store = IntCodes[Ptr + 3];
            var op1 = IntCodes[Ptr + 1];
            var op2 = IntCodes[Ptr + 2];

            IntCodes[store] = IntCodes[Ptr] == 1 ? IntCodes[op1] + IntCodes[op2]
                                                 : IntCodes[op1] * IntCodes[op2];
            return true;
        }

        int RunProgram()
        {
            IntCodes[1] = 12;
            IntCodes[2] = 2;

            int Ptr = 0;
            while (IntCodes.Keys.Contains(Ptr))
                if (!RunOpCode(Ptr+=4))
                    break;
            
            return IntCodes[0];
        }

        public int Solve(int part)
            => RunProgram();
    }
}
