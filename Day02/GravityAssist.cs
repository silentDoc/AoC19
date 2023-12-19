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

        int RunProgram(int noun, int verb)
        {
            IntCodes[1] = noun;
            IntCodes[2] = verb;

            int Ptr = 0;
            while (IntCodes.Keys.Contains(Ptr))
                if (!RunOpCode(Ptr+=4))
                    break;
            
            return IntCodes[0];
        }

        int FindNounAndVerb()
        {
            int noun = 0;
            int verb = 0;
            List<int> backup = new List<int>(IntCodes.Values);

            for (noun = 0; noun <= 99; noun++)
                for (verb = 0; verb <= 99; verb++)
                {
                    // Idempotence
                    for (int i = 0; i < backup.Count; i++)
                        IntCodes[i] = backup[i];

                    if (RunProgram(noun, verb) == 19690720)
                        return noun * 100 + verb;
                }
            return -1;
        }

        public int Solve(int part)
            => part == 1 ? RunProgram(12, 2) : FindNounAndVerb();
    }
}
