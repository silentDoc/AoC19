using AoC19.Common;

namespace AoC19.Day11
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

    internal class PaintTerm
    {
        Dictionary<long, long> IntCodes = new();
        Dictionary<Coord2D, long> PanelMap = new();

        Coord2D CurrentPosition = new(0, 0);

        List<long> OneParamInstructions = new() { Instructions.Input, Instructions.Output, Instructions.AdjustRelBase };
        List<long> TwoParamInstructions = new() { Instructions.JumpNonZero, Instructions.JumpZero };

        long Ptr = 0;    // Instruction pointer

        public long LastOutput = 0;
        const long EXIT_PROGRAM = long.MinValue;
        const long UNUSED_PARAM = long.MinValue + 2;

        long RelativeBase = 0;

        Coord2D UP = (0, -1);
        Coord2D DOWN = (0, 1);
        Coord2D RIGHT = (1, 0);
        Coord2D LEFT = (-1, 0);

        Coord2D CurrentDirection = new(0,-1);
        HashSet<Coord2D> PaintedPositions = new();

        bool paintOrTurn = true;

        void TurnLeft()
        {
            CurrentDirection = CurrentDirection switch
            {
                (0, -1) => LEFT,
                (-1, 0) => DOWN,
                (0, 1) => RIGHT,
                (1, 0) => UP,
                _ => throw new Exception("Invalid direction : " + CurrentDirection.ToString())
            };
        }

        void TurnRight()
        {
            CurrentDirection = CurrentDirection switch
            {
                (0, -1) => RIGHT,
                (-1, 0) => UP,
                (0, 1)  => LEFT,
                (1, 0)  => DOWN,
                _ => throw new Exception("Invalid direction : " + CurrentDirection.ToString())
            };
        }

        public void ParseInput(List<string> lines)
        {
            IntCodes.Clear();
            var nums = lines[0].Split(",").Select(long.Parse).ToList();
            for (int i = 0; i < nums.Count; i++)
                IntCodes[i] = nums[i];
        }

        long GetCurrentPanelColor()
            => PanelMap.Keys.Contains(CurrentPosition) ? PanelMap[CurrentPosition] : PanelMap[CurrentPosition] = 0;

        long PaintCurrentPanel(long color)
            => PanelMap[CurrentPosition] = color;

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
            long op1 = opCode == Instructions.Input ? GetAddress(v1, p1Mode) : GetOperand(v1, p1Mode);
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
                    IntCodes[op1] = GetCurrentPanelColor();
                    increment = 2;
                    break;
                case Instructions.Output:
                    LastOutput = op1;
                    if (paintOrTurn)
                    {
                        PaintCurrentPanel(op1);
                        PaintedPositions.Add(CurrentPosition);
                    }
                    else
                    {
                        if (op1 == 0)
                            TurnLeft();
                        else
                            TurnRight();

                        CurrentPosition += CurrentDirection;
                    }
                    paintOrTurn = !paintOrTurn;
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
            LastOutput = PaintedPositions.Count();
            return LastOutput;
        }
    }

    internal class Painter
    {
        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        long FindPaintedPanels(int part = 1)
        {
            PaintTerm term = new();
            term.ParseInput(sourceCode);
            term.RunProgram();
            return term.LastOutput;
        }

        public long Solve(int part = 1)
            => FindPaintedPanels(part);
    }
}
