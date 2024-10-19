using System.Diagnostics;

namespace AoC19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int day = 10;
            int part = 1;
            bool test = !false;

            string input = "./Input/day" + day.ToString("00");
            input += (test) ? "_test.txt" : ".txt";

            Console.WriteLine("AoC 2019 - Day {0} , Part {1} - Test Data {2}", day, part, test);
            Stopwatch st = new();
            st.Start();
            string result = day switch
            {
                1  => day1(input, part),
                2  => day2(input, part),
                3  => day3(input, part),
                4  => day4(input, part),
                5  => day5(input, part),
                6  => day6(input, part),
                7  => day7(input, part),
                8  => day8(input, part),
                9  => day9(input, part),
                10 => day10(input, part),
                _ => throw new ArgumentException("Wrong day number - unimplemented")
            };
            st.Stop();
            Console.WriteLine("Result : {0}", result);
            Console.WriteLine("Elapsed : {0}", st.Elapsed.TotalSeconds);
        }

        static string day1(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day01.RocketEquation eq = new();
            eq.ParseInput(lines);
            return eq.Solve(part).ToString();
        }

        static string day2(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day02.GravityAssist assist = new();
            assist.ParseInput(lines);
            return assist.Solve(part).ToString();
        }

        static string day3(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day03.WireMap wires = new();
            wires.ParseInput(lines);

            return wires.Solve(part).ToString();
        }

        static string day4(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day04.PassChecker checker = new();
            checker.ParseInput(lines);
            return checker.Solve(part).ToString();
        }

        static string day5(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day05.TestTerm term = new();
            term.ParseInput(lines);
            return term.Solve(part).ToString();
        }

        static string day6(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day06.OrbitCounter counter = new();
            counter.ParseInput(lines);
            return counter.Solve(part).ToString();
        }

        static string day7(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day07.Amplifiers amp = new();
            amp.ParseInput(lines);

            return amp.Solve(part).ToString();
        }

        static string day8(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day08.ImageFormatAnalyzer img = new();
            img.ParseInput(lines);

            return img.Solve(part).ToString();
        }

        static string day9(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day09.Booster booster = new();
            booster.ParseInput(lines);

            return booster.Solve(part).ToString();
        }

        static string day10(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();

            return "";
        }

    }
}
