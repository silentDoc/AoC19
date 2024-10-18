namespace AoC19.Day07
{
    public enum TermState
    { 
        RUNNING =0, 
        WAITING =1, 
        HALTED =2
    }

    public class Instructions
    {
        public const int Sum = 1;
        public const int Mul = 2;
        public const int Input = 3;
        public const int Output = 4;
        public const int JumpNonZero = 5;
        public const int JumpZero = 6;
        public const int LessThan = 7;
        public const int Equal = 8;
    }

    internal class TestTerm
    {
        Dictionary<int, int> IntCodes = new();
        int Ptr = 0;    // Instruction pointer
        int phase = 0;
        bool phaseSet = false;
        int Part = 1;

        public int LastOutput = 0;
        const int EXIT_PROGRAM = -9999999;
        const int NO_INPUT = -8888888;

        // Part 2
        public Queue<int> inputBuffer = new Queue<int>();
        public TestTerm nextTerm = null;
        public TermState State = TermState.RUNNING;

        public TestTerm(int phaseNum)
            => phase = phaseNum;

        public void ReceiveInput(int input)
            => inputBuffer.Enqueue(input);

        public void SetPart(int part)
            => Part = part;

        public void ParseInput(List<string> lines)
        {
            IntCodes.Clear();
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
                case Instructions.Sum:     
                case Instructions.Mul:
                    IntCodes[v3] = opCode == Instructions.Sum ? op1 + op2 : op1 * op2;
                    break;
                case Instructions.Input:
                    if (Part == 2 && phaseSet && inputBuffer.Count == 0)    
                        return NO_INPUT;
                
                    IntCodes[v1] = (!phaseSet) ? phase : inputBuffer.Dequeue();
                    phaseSet = true;
                    increment = 2;
                    break;
                case Instructions.Output:  
                    LastOutput = op1;
                    
                    if(Part==2)
                        nextTerm.ReceiveInput(LastOutput);
                    
                    increment = 2;
                    break;
                case Instructions.JumpNonZero:
                case Instructions.JumpZero:
                    bool cond_jmp = opCode == Instructions.JumpNonZero ? (op1 != 0) : (op1 == 0);
                    increment = cond_jmp ? op2 - Ptr : 3;
                    break;
                case Instructions.LessThan:
                case Instructions.Equal:   
                    bool cond_comp = opCode == Instructions.LessThan ? (op1 < op2) : (op1 == op2);
                    IntCodes[v3] = cond_comp ? 1 : 0;
                    break;
            }
            return increment;
        }

        public int RunProgram()
        {
            while (IntCodes.Keys.Contains(Ptr))
            {
                State = TermState.RUNNING;
                var increment = RunOpCode(Ptr);
                if (increment == EXIT_PROGRAM || increment == NO_INPUT)
                {
                    State = (increment == NO_INPUT) ? TermState.WAITING : TermState.HALTED;
                    break;
                }
                Ptr += increment;
            }
            return LastOutput;
        }
    }

    internal class Amplifiers
    {
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

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

        int FindSignal(int part = 1)
        {
            int maxSignal = -1;
            var perms = part == 1 ? getAllPerms(0, 4) : getAllPerms(5, 9);

            foreach (var vs in perms)
            {
                List<TestTerm> amplifiers = new();
                for (int i = 0; i < 5; i++)
                {
                    amplifiers.Add(new(vs[i]));
                    amplifiers[i].SetPart(part);
                    amplifiers[i].ParseInput(sourceCode);
                }

                for (int i = 0; i < 4; i++)     
                    amplifiers[i].nextTerm = amplifiers[i + 1];
                amplifiers[4].nextTerm = amplifiers[0];

                amplifiers[0].ReceiveInput(0);

                var input = 0;
                if (part == 1)
                    for (int i = 0; i < 5; i++)
                    {
                        amplifiers[i].ReceiveInput(input);
                        amplifiers[i].RunProgram();
                        input = amplifiers[i].LastOutput;
                    }
                else
                    while (amplifiers[4].State != TermState.HALTED)
                    {
                        foreach (var a in amplifiers)
                            a.RunProgram();
                    }
                
                var signal = amplifiers[4].LastOutput;
                maxSignal = (signal > maxSignal) ? signal : maxSignal;
            }
            return maxSignal;
        }

        public int Solve(int part = 1)
            => FindSignal(part);
    }
}
