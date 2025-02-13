namespace Fanex.Athena.Common.Utility.Services
{
    using Fanex.Athena.Common.Model.DTO;
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AppSettings : IAppSettings
    {
        public string Environment { get; set; }

        public bool IsProduction => Environment.Equals("PROD", StringComparison.OrdinalIgnoreCase) || Environment.Equals("PRO", StringComparison.OrdinalIgnoreCase);

        public IEnumerable<int> FakeLicenseeList { get; set; } = Enumerable.Empty<int>();

        public string TempFilePath { get; set; }

        public int MaxRowPerExcelFile { get; set; }

        public bool IsDisableExportLargeBetList { get; set; }

        public IEnumerable<string> LicenseesWithLargeData { get; set; }

        public int MaxRowForMultipleExcelFiles { get; set; }

        public int MaxTicketLimit { get; set; }

        public int MaxRowSupportSingleExportExcel { get; set; }

        public string CustomerClassCategoryApiUrl { get; set; }

        public IEnumerable<ReportSetting> ReportSettings { get; set; }

        public IEnumerable<FakeLicenseeSetting> FakeLicenseeSettings { get; set; }

        public DateTime HistoryDate { get; set; }

        public DateTime? MaxDate { get; set; }
    }
}