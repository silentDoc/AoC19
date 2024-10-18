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

        int SolvePart2()
        {
            List<int> image = new();
            for (int pos = 0; pos < wide * tall; pos++)
                image.Add(layers.Select(layer => layer[pos]).First(x => x != 2));

            var lines = image.Chunk(wide).ToList();
            Console.WriteLine("");
            lines.ForEach( line => Console.WriteLine( string.Concat( line.Select( x => (x==1 ? "*" : " ")))));
            Console.WriteLine("");
            return 0;
        }

        public int Solve(int part = 1)
            => part ==1 ? SolvePart1() : SolvePart2();
    }
}
