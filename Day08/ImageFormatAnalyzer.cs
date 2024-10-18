namespace AoC19.Day08
{
    internal class ImageFormatAnalyzer
    {
        int wide = 25;
        int tall = 6;
        List<int[]> layers = new();

        public void ParseInput(List<string> lines)
           => layers = lines[0].ToList()
                               .Select(x => int.Parse(x.ToString()))
                               .Chunk(wide * tall)
                               .ToList();

        int SolvePart1()
        { 
            var zeroes = layers.Select(x => x.Count(y => y==0)).ToList();
            var index = zeroes.IndexOf(zeroes.Min());
            return layers[index].Count(x => x == 1) * layers[index].Count(x => x == 2);
        }

        public int Solve(int part = 1)
            => SolvePart1();
    }
}
