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
        List<string> asm = [
            // Jump if D is safe and hole at B or C
            "NOT B J\n",
            "NOT C T\n",
            "OR T J\n",
            "AND D J\n",
            // Jump if A is hole
            "NOT A T\n",
            "OR T J\n",
            // walk
            "WALK\n"];

        List<string> sourceCode = new();

        public void ParseInput(List<string> lines)
            => sourceCode = lines;

        public int GetHullDamage(int part = 1)
        {
            HullRobot hullRobot = new(sourceCode);
            
            hullRobot.LoadInstructions(asm);
            return hullRobot.GetDamage();
        }

        public long Solve(int part = 1)
            => GetHullDamage(part);
    }
}
