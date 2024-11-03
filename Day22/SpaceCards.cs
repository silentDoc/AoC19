namespace AoC19.Day22
{
    public static class ShuffleOperation
    {
        public const int DealIntoNew       = 0;
        public const int CutNCards         = 1;
        public const int DealWithIncrement = 2;
    }

    public record ShuffleStep
    {
        public int Operation;
        public int Amount;

        public ShuffleStep(int operation, int amount)
        { 
            Operation = operation;
            Amount = amount;
        }
    }

    class CardDealer
    {
        List<ShuffleStep> steps = new List<ShuffleStep>();
        int numberOfCards = 10007; // 10007 is prime num

        public CardDealer(List<string> instructions)
            => instructions.ForEach(ParseInstruction);

        void ParseInstruction(string instruction)
        {
            // For easier parsing
            var ins = instruction.Replace("deal into new stack", "new").Replace("deal with increment", "inc");
            if (ins.StartsWith("new"))
                steps.Add(new ShuffleStep(ShuffleOperation.DealIntoNew, -1));
            else
            {
                var parts = ins.Split(' ');
                var operation = parts[0] == "inc" ? ShuffleOperation.DealWithIncrement : ShuffleOperation.CutNCards;
                var amount = int.Parse(parts[1]);
                steps.Add(new ShuffleStep(operation, amount));
            }
        }

        public int Shuffle(int position)
        {
            int[] deck = new int[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
                deck[i] = i;

            foreach (var step in steps)
                deck = step.Operation switch
                {
                    ShuffleOperation.DealIntoNew => DealIntoNew(deck),
                    ShuffleOperation.DealWithIncrement => DealWithIncrement(deck, step.Amount),
                    ShuffleOperation.CutNCards => CutNCards(deck, step.Amount),
                    _ => throw new Exception("Invalid operation - " + step.Operation.ToString())
                };

            return Array.IndexOf(deck, position);
        }

        int[] DealIntoNew(int[] deck)
            => deck.Reverse().ToArray();

        int[] DealWithIncrement(int[] deck, int increment)
        {
            int[] newDeck = new int[deck.Length];

            for (int i = 0; i < deck.Length; i++)
            {
                var idx = i * increment % deck.Length;
                newDeck[idx] = deck[i];
            }
            return newDeck;
        }

        int[] CutNCards(int[] deck, int cut)
        {
            var c = cut > 0 ? cut : deck.Length + cut;
            return [..deck[c..], ..deck[..c]];
        }
    }

    internal class SpaceCards
    {
        List<string> instructions = new List<string>();

        public void ParseInput(List<string> lines)
            => instructions.AddRange(lines);

        int Deal(int part = 1)
        {
            CardDealer dealer = new(instructions);
            return dealer.Shuffle(2019);
        }

        public int Solve(int part = 1)
            => Deal(part);
    }
}
