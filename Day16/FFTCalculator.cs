namespace AoC19.Day16
{
    class FFTProcessor
    { 
        List<int> input = new List<int>();
        List<int> basePattern = new List<int>([0, 1, 0, -1]);

        public FFTProcessor(string inputString)
            => input = inputString.Select(x => int.Parse(x.ToString())).ToList();

        int calculatePosition(List<int> inputElements, int position)
        { 
            // Generate the base pattern
            List<int> factors = new List<int>();
            int iterations = 0;
            while(factors.Count < inputElements.Count+3) 
            {
                factors.AddRange(Enumerable.Repeat(basePattern[iterations % 4], position +1).ToList());
                iterations++;
            }

            factors = factors.Skip(1).Take(inputElements.Count).ToList();
            return Math.Abs(inputElements.Zip(factors).Sum(x => x.First * x.Second)) % 10;
        }

        public string Process(int numPhases)
        {
            List<int> localInput = [.. input];
            List<int> output = new List<int>();
            var numElements = input.Count();
            var indexs = Enumerable.Range(0, numElements).ToList();

            for (int i = 0; i < numPhases; i++)
            {
                indexs.ForEach(x => output.Add(calculatePosition(localInput, x)));
                localInput = [.. output];
                output.Clear();
            }

            return string.Concat(localInput.Select(x => x.ToString()));
        }
    }

    class FFTCalculator
    {
        string input = "";

        public void ParseInput(List<string> lines)
            => input = lines[0];

        string CalculatePhases(int numPhases)
        {
            FFTProcessor proc = new(input);
            return proc.Process(100);
        }

        public string Solve(int part = 1)
            => CalculatePhases(1);
    }
}
