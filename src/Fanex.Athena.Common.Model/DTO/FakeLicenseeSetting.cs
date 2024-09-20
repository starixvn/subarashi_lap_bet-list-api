namespace Fanex.Athena.Model.Core.DTO.Configuration
{
    public class FakeLicenseeSetting
    {
        public string SiteName { get; set; }

        public bool DisableBetList { get; set; }

        public string MinDate { get; set; }

        public string MaxDate { get; set; }

        public string BaseCurrencies { get; set; }

        public byte LimitRecentMonths { get; set; }

        public int? TimeZone { get; set; }
    }
}