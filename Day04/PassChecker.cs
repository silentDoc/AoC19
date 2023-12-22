namespace AoC19.Day04
{
    internal class PassChecker
    {
        int start = 0;
        int end = 0;

        public void ParseInput(List<string> lines)
        {
            var vs = lines[0].Split('-').Select(int.Parse).ToList();
            start = vs[0];
            end = vs[1];
        }

        int FindCombinations(int part = 1)
        {
            int count = 0;
            for(int v = start; v<= end; v++) 
            {
                var str = v.ToString();
                var tuples = str.Substring(1).Zip(str.Substring(0, str.Length - 1), (f, s) => (f, s)).ToList();
                var repeated = tuples.Any(x => x.f == x.s);
                var incOrEqual = tuples.All(x => x.f >= x.s);

                if (!(repeated && incOrEqual))
                    continue;

                if (part == 1)
                    count++;
                else
                {
                    var repeats = str.ToCharArray().Distinct().Select(x => str.Count(c => c == x)).ToList();
                    if (repeats.Any(x => x == 2))
                        count++;
                }
            }
            return count;
        }

        public int Solve(int part = 1)
            => FindCombinations(part);
    }
}
