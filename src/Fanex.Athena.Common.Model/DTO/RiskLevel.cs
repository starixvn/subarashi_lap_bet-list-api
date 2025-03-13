namespace Fanex.Athena.Common.Model.DTO
{
    public class RiskLevel
    {
        public const string Uncategorized = "Uncategorized";

        private static readonly IDictionary<string, int> RiskLevelMapper = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "VIP", 1 },
            { "Healthy", 2 },
            { "New", 3 },
            { "Moderate", 4 },
            { "Suspicious", 5 },
            { "Dangerous", 6 },
            { "Problems", 7 },
            { "Robot", 8 },
            { Uncategorized, 999 }
        };

        public string Id { get; set; }

        public String Key { get; set; }

        public String Name { get; set; }

        public IEnumerable<string> CustomerClassList { get; set; } = Enumerable.Empty<string>();

        public static int GetComparerValue(RiskLevel level)
        {
            const int defaultValue = 99;

            return RiskLevelMapper.TryGetValue(level.Key, out int value) ? value : defaultValue;
        }
    }

    public class RiskLevelComparer : IComparer<RiskLevel>
    {
        public int Compare(RiskLevel x, RiskLevel y)
        {
            var xValue = RiskLevel.GetComparerValue(x);
            var yValue = RiskLevel.GetComparerValue(y);

            if (xValue > yValue)
            {
                return 1;
            }

            if (xValue < yValue)
            {
                return -1;
            }

            return 0;
        }
    }
}