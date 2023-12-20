using AoC19.Day01;
using System.Diagnostics;

namespace AoC19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int day = 3;
            int part = 1;
            bool test = false;

            string input = "./Input/day" + day.ToString("00");
            input += (test) ? "_test.txt" : ".txt";

            Console.WriteLine("AoC 2019 - Day {0} , Part {1} - Test Data {2}", day, part, test);
            Stopwatch st = new();
            st.Start();
            string result = day switch
            {
                1 => day1(input, part),
                2 => day2(input, part),
                3 => day3(input, part),
                _ => throw new ArgumentException("Wrong day number - unimplemented")
            };
            st.Stop();
            Console.WriteLine("Result : {0}", result);
            Console.WriteLine("Elapsed : {0}", st.Elapsed.TotalSeconds);
        }

        static string day1(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            RocketEquation eq = new();
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

    }
}
