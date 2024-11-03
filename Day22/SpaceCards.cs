using System.Numerics;

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
            return [.. deck[c..], .. deck[..c]];
        }


        // Part 2

        public BigInteger ShufflePart2()
        {
            // To face part 2 we cannot replicate all the deck transformations through the steps.
            // We have to reverse the order of the steps and find out where the card of interest started.

            // While Part 1 used the modulus in one operation, in part 2 we have to see the whole problem from the modular arithmetic perspective
            // We have to think each shuffle operation as a modular arithmetic operation. For a card in position X
            // Deal into new stack   : f(x) = m-x-1 = -x-1 mod m
            // Cut N cards           : f(x) = x-n mod m
            // Deal with increment N : f(x) = n*x mod m

            // We can see each function as f(x) = a*x + b, and the result of a shuffle will be the combination of the functions, but for now
            // Deal into new stack   : f(x) = m-x-1 = -x-1 mod m ;; a = -1 , b = -1
            // Cut N cards           : f(x) = x-n mod m          ;; a = 1  , b = -n
            // Deal with increment N : f(x) = n*x mod m          ;; a = n  , b = 0
             
            // In modular arithmetic, g(f(x)) == f;g(x) .
            // f(x)    = ax + b mod m
            // g(x)    = cx + d mod m
            // g(f(x)) = c* (ax+b) + d mod m = acx + bc + d mod m
            // All operations can be composed : (a,b);(c,d) = (ac mod m, bc+d mod m)

            BigInteger numCards = 119315717514047;
            BigInteger numShuffle = 101741582076661;
            BigInteger position = 2020;
            BigInteger a = 1;
            BigInteger b = 0;

            steps.Reverse();    // Let's undo

            foreach (var step in steps)
            {
                if (step.Operation == ShuffleOperation.DealIntoNew)
                {
                    a = -a;
                    b = -b - 1;
                }
                if (step.Operation == ShuffleOperation.CutNCards)
                {
                    b += BigInteger.Parse(step.Amount.ToString());
                }
                if (step.Operation == ShuffleOperation.DealWithIncrement)
                {
                    var pow = ModInv(BigInteger.Parse(step.Amount.ToString()), numCards);
                    a *= pow;
                    b *= pow;
                }
            }

            // The result after having composed all the shuffles is calculated using exponentiation on module
            BigInteger result =  SolvePart2(numCards, numShuffle, position, a, b);

            // Must take into account that the result could be negative. In such case we apply modulus of negative:
            // ( x mod n ) = ((x mod n) + n) mod n ; when x is negative
            if (result < 0)
                result = (result % numCards + numCards) % numCards;

            return result;
        }

        private BigInteger SolvePart2(BigInteger n, BigInteger t, BigInteger p, BigInteger a, BigInteger b)
            => (p * BigInteger.ModPow(a, t, n) + (BigInteger.ModPow(a, t, n) - 1) * b * ModInv(a - 1, n)) % n;

        private BigInteger ModInv(BigInteger a, BigInteger m)
            => BigInteger.ModPow(a, m - 2, m);
    }

    internal class SpaceCards
    {
        List<string> instructions = new List<string>();

        public void ParseInput(List<string> lines)
            => instructions.AddRange(lines);

        string Deal(int part = 1)
        {
            CardDealer dealer = new(instructions);
            return part ==1 ? dealer.Shuffle(2019).ToString()
                            : dealer.ShufflePart2().ToString();
        }

        public string Solve(int part = 1)
            => Deal(part);
    }
}
