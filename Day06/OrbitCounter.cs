namespace AoC19.Day06
{
    class Satellite
    {
        public string Parent = "";
        public int level = -1;

        public int Level(Dictionary<string, Satellite> allSats)
        {
            if (level == -1)
                level = allSats[Parent].Level(allSats) + 1;

            return level;
        }
    }
    internal class OrbitCounter
    {
        Dictionary<string, Satellite> allSatellites = new();

        void ParseLine(string line)
            => allSatellites[line.Split(')')[1]] = new Satellite() { Parent = line.Split(')')[0] };

        public void ParseInput(List<string> lines)
        {
            lines.ForEach(ParseLine);
            allSatellites["COM"] = new Satellite() { Parent = "", level =0};
        }

        public int Solve(int part = 1)
            => part == 1 ? allSatellites.Values.Sum(x => x.Level(allSatellites)) : 0;
    }
}
