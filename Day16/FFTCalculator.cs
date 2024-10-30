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

        public string ProcessPart2(int numPhases) 
        {
            // We go for arrays instead of lists - faster
            var newLength = input.Count * 10000;
            var nums = new int[newLength];
            for (var i = 0; i < newLength; i++)
                nums[i] = input[i % input.Count];

            var offsetStr = string.Concat(nums[..7].Select(x => x.ToString()));
            var offset = int.Parse(offsetStr);

            // Let's analyze what we're asked. 
            // First, our new signal length will be 650*10000 = 6.500.000 positions
            // Our offset is the first 7 positions --> 59766299, well beyond half the 
            // This means that our digits of interest factors will be all 1s
            
            // We can work out only the digits that are after the offset, and if we reverse the loop, we can use
            // the previous operation to decide the current value. 

            for (var phase =0; phase<numPhases; phase++)
                for (var i = nums.Length - 2; i >= offset; i--)
                    nums[i] = (nums[i] + nums[i + 1]) % 10;

            return string.Join("", nums.Skip(offset).Take(8));
        }
    }

    class FFTCalculator
    {
        string input = "";

        public void ParseInput(List<string> lines)
            => input = lines[0];

        string CalculatePhases(int part = 1)
        {
            FFTProcessor proc = new(input);
            return part == 1 ? proc.Process(100) : proc.ProcessPart2(100); 
        }

        public string Solve(int part = 1)
            => CalculatePhases(part);
    }
}
