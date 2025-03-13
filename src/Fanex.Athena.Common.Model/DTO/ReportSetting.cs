namespace Fanex.Athena.Common.Model.DTO
{
    public class ReportSetting
    {
        public string ReportKey { get; set; }

        public string MinDateString { get; set; }

        public string MaxDateString { get; set; }

        public string CutOffDateString { get; set; }

        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public bool IsDisableBetList { get; set; }

        public string BaseCurrencies { get; set; }

        public int LimitRecentMonths { get; set; }

        public int LimitRecentDays { get; set; }

        public int? TimeZone { get; set; }
    }
}
