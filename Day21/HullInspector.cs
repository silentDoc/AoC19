using AoC19.Common;

namespace AoC19.Day21
{
    class HullRobot
    {
        IntCodeProcessor droid = new();

        public HullRobot(List<string> sourceCode)
           => droid.ParseInput(sourceCode);

        public void LoadInstructions(List<string> asmInstructions)
            => asmInstructions.SelectMany(x => x).ToList().ForEach(x => droid.AddInputToQueue((int)x));

        public int GetDamage()
        {
            long output = -1;
            while (!droid.ProgramEnded())
                output = droid.RunProgram(0);

            return (int)output;
        }
    }
    internal class HullInspector
    {
        List<string> asm_p1 = [
            // Jump if (hole at B or C) AND (D is safe)
            "NOT B J\n",
            "NOT C T\n",
            "OR T J\n",
            "AND D J\n",
            // Jump if (A is hole)
            "NOT A T\n",
            "OR T J\n",
            // walk
            "WALK\n"];

        // Part 2 is the same, all the extra registers are not needed because when you jump
        // they will become ABCD. H is the only one, as 4+4 is the range of 2 jumps, so what we want to do
        // is wait until last moment to jump.
        List<string> asm_p2 = [
            // Jump if (hole at B or C) AND (D is safe)
            "NOT B J\n",
            "NOT C T\n",
            "OR T J\n",
            "AND D J\n",
            // Part 2 - (H has to be safe as well)
            "AND H J\n",
            // Jump if A is hole - always
            "NOT A T\n",
            "OR T J\n",
            // Part 2 - Run instead of walk
            "RUN\n"];

        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        public int GetHullDamage(int part = 1)
        {
            HullRobot hullRobot = new(sourceCode);
            
            hullRobot.LoadInstructions(part == 1? asm_p1 : asm_p2);
            return hullRobot.GetDamage();
        }

        public long Solve(int part = 1)
            => GetHullDamage(part);
    }
}
