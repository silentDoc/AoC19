﻿namespace AoC19.Day09
{
    public static class Instructions
    {
        public const long Sum = 1;
        public const long Mul = 2;
        public const long Input = 3;
        public const long Output = 4;
        public const long JumpNonZero = 5;
        public const long JumpZero = 6;
        public const long LessThan = 7;
        public const long Equal = 8;
        public const long AdjustRelBase = 9;
    }

    internal class BoostTerm
    {
        Dictionary<long, long> IntCodes = new();

        List<long> OneParamInstructions = new() { Instructions.Input, Instructions.Output, Instructions.AdjustRelBase};
        List<long> TwoParamInstructions = new() { Instructions.JumpNonZero, Instructions.JumpZero };

        long Ptr = 0;    // Instruction pointer
        long phase = 0;

        public long LastOutput  = 0;
        const long EXIT_PROGRAM = long.MinValue;
        const long UNUSED_PARAM = long.MinValue + 2;

        long RelativeBase = 0;

        public BoostTerm(int phaseNum)
            => phase = phaseNum;

        public void ParseInput(List<string> lines)
        {
            IntCodes.Clear();
            var nums = lines[0].Split(",").Select(long.Parse).ToList();
            for (int i = 0; i < nums.Count; i++)
                IntCodes[i] = nums[i];
        }

        long GetOperand(long value, long mode)
            => mode switch
            {
                0 => IntCodes.Keys.Contains(value) ? IntCodes[value] : 0,                                // Position
                1 => value,                                                                              // Immediate
                2 => IntCodes.Keys.Contains(RelativeBase + value) ? IntCodes[RelativeBase + value] : 0,  // Relative
                _ => throw new Exception("Unkown parameter mode received")
            };

        long GetAddress(long value, long mode)
           => mode switch
           {
               0 => value,
               1 => value,
               2 => RelativeBase + value,  
               _ => throw new Exception("Unkown parameter mode received")
           };

        long RunOpCode(long Ptr)
        {
            var opCodeSet = IntCodes[Ptr];
            var strOpCode = opCodeSet.ToString("00000");            // ABCDE

            // Get instruction and parameter modes
            var opCode = long.Parse(strOpCode.Substring(3));        // DE
            var p1Mode = long.Parse(strOpCode.Substring(2, 1));     // C  - 0 -> Position, 1 -> immediate, 2 - relative
            var p2Mode = long.Parse(strOpCode.Substring(1, 1));     // B
            var p3Mode = long.Parse(strOpCode.Substring(0, 1));     // A

            if (opCode == 99)
                return EXIT_PROGRAM;

            // Retrieve the values in the source code
            long v1 = IntCodes[Ptr + 1];
            long v2 = OneParamInstructions.Contains(opCode) ? UNUSED_PARAM : IntCodes[Ptr + 2];  
            long v3 = OneParamInstructions.Contains(opCode) || TwoParamInstructions.Contains(opCode) ? UNUSED_PARAM : IntCodes[Ptr + 3];  

            // Transform the values into operands depending on the parameter modes
            long op1 = opCode == Instructions.Input ? GetAddress(v1,p1Mode) : GetOperand(v1, p1Mode);
            long op2 = v2 != UNUSED_PARAM ? GetOperand(v2, p2Mode) : UNUSED_PARAM;
            long op3 = v3 != UNUSED_PARAM ? GetAddress(v3, p3Mode) : UNUSED_PARAM;  // 3rd param is only for writing, has special treatment

            long increment = 4;
            switch (opCode)
            {
                case Instructions.Sum:
                case Instructions.Mul:
                    IntCodes[op3] = opCode == Instructions.Sum ? op1 + op2 : op1 * op2;
                    break;
                case Instructions.Input:
                    IntCodes[op1] = phase;
                    increment = 2;
                    break;
                case Instructions.Output:
                    LastOutput = op1;
                    Console.WriteLine(op1.ToString());
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
                    IntCodes[op3] = cond_comp ? 1 : 0;
                    break;
                case Instructions.AdjustRelBase:
                    RelativeBase += op1;
                    increment = 2;
                    break;
            }
            return increment;
        }

        public long RunProgram()
        {
            while (IntCodes.Keys.Contains(Ptr))
            {
                var increment = RunOpCode(Ptr);
                if (increment == EXIT_PROGRAM)
                    break;
                Ptr += increment;
            }
            return LastOutput;
        }
    }

    internal class Booster
    {
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        long FindBoostByteCode(int part = 1)
        {
            BoostTerm term = new(part == 1 ? 1 : 2); 
            term.ParseInput(sourceCode);
            term.RunProgram();
            return term.LastOutput;
        }

        public long Solve(int part = 1)
            => FindBoostByteCode(part);
    }
}
