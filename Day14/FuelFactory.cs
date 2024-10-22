namespace AoC19.Day14
{
    public record Chemical
    {
        public string Name;
        public int Amount;

        public Chemical(string name, int amount)
        {   
            Name = name;
            Amount = amount;
        }
    }

    public record Reaction
    {
        public List<Chemical> Inputs = new();
        public Chemical Output;

        public Reaction(List<Chemical> inputs, Chemical output)
        { 
            Inputs.AddRange(inputs);
            Output = output;
        }
    }

    internal class FuelFactory
    {
        List<Reaction> Reactions = new();
        Dictionary<string, long> AvailableElements = new();

        void ParseLine(string line) 
        {
            var parts = line.Split("=>", StringSplitOptions.TrimEntries);
            var inputs = parts[0].Split(",", StringSplitOptions.TrimEntries).Select(x => x.Split(" "))
                                                                            .Select(x => new Chemical(x[1], int.Parse(x[0])))
                                                                            .ToList();
            var output_parts = parts[1].Split(" ");
            var output = new Chemical(output_parts[1], int.Parse(output_parts[0]));

            Reactions.Add(new Reaction(inputs, output));
        }

        long FindOreToProduce(string chemicalName, long amount)
        {
            long ore = 0;
            var targetReaction = Reactions.First(x => x.Output.Name == chemicalName);

            var alreadyAvailable = AvailableElements.ContainsKey(chemicalName) ? AvailableElements[chemicalName] : 0;
            var amountNeeded = amount - alreadyAvailable;
            var amountNeededOrZero = amountNeeded < 0 ? 0 : amountNeeded;   // We may have all what we need already
            
            var numBatches = amountNeededOrZero / targetReaction.Output.Amount;
            if (amountNeededOrZero % targetReaction.Output.Amount > 0)
                numBatches++;

            var amountProduced = targetReaction.Output.Amount * numBatches;
            var leftAvailable = amountProduced - amountNeeded;

            foreach (var input in targetReaction.Inputs)
            {
                var inputAmount = input.Amount * numBatches;
                ore += input.Name == "ORE" ? inputAmount : FindOreToProduce(input.Name, inputAmount);
            }

            AvailableElements[chemicalName] = leftAvailable;
            return ore;
        }

        long BurnInventory()
        {
            // This is a guess game. A blunt loop would be very inefficient, we have to ask how much ore is required to
            // produce X fuel, and keep looking for the X that is just below the ore in deck. Binary search ftw

            long oreInCargo = 1000000000000;
            long minBound = 1;
            long maxBound = oreInCargo/1000;

            while (maxBound - minBound > 1)
            {
                var average = (minBound + maxBound) / 2;
                var oreNeeded = FindOreToProduce("FUEL", average);

                if (oreNeeded < oreInCargo)
                    minBound = average;
                else
                    maxBound = average;
            }
            
            return minBound;
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        public double Solve(int part = 1)
            => part == 1 ? FindOreToProduce("FUEL", 1) : BurnInventory();
    }
}
