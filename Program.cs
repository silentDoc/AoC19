using System.Diagnostics;

namespace AoC19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int day = 16;
            int part = 1;
            bool test = false;
            int testNum = 0;

            string input = "./Input/day" + day.ToString("00");
            
            input += (test) ? "_test" + (testNum>0 ? testNum.ToString() : "") + ".txt" : ".txt";

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
                11 => day11(input, part),
                12 => day12(input, part),
                13 => day13(input, part),
                14 => day14(input, part),
                15 => day15(input, part),
                16 => day16(input, part),
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
            Day10.StationLocator locator = new();
            locator.ParseInput(lines);

            return locator.Solve(part).ToString();
        }

        static string day11(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day11.Painter painter = new();
            painter.ParseInput(lines);
            return painter.Solve(part).ToString();
        }

        static string day12(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day12.MoonSystem moonsys = new();
            moonsys.ParseInput(lines);
            return moonsys.Solve(part).ToString();
        }

        static string day13(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day13.Cabinet cabinet = new();
            cabinet.ParseInput(lines);
            return cabinet.Solve(part).ToString();
        }

        static string day14(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day14.FuelFactory factory = new();
            factory.ParseInput(lines);
            return factory.Solve(part).ToString();
        }

        static string day15(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day15.OxygenFinder oxygenFinder = new();
            oxygenFinder.ParseInput(lines);
            return oxygenFinder.Solve(part).ToString();
        }

        static string day16(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day16.FFTCalculator fftCalculator = new();
            fftCalculator.ParseInput(lines);
            return fftCalculator.Solve(part).ToString();
        }
    }
}
