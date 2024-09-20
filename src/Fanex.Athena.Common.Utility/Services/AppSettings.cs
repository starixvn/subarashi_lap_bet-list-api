namespace Fanex.Athena.Common.Utility.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Athena.Common.Model.DTO;
    using Fanex.Athena.Model.Core.DTO.Configuration;
    using Interfaces;

    /// <summary>
    /// Setting class.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        public string Environment { get; set; }

        public bool IsProduction => Environment.Equals("PRO", StringComparison.OrdinalIgnoreCase);

        public IEnumerable<int> FakeLicenseeList { get; set; } = Enumerable.Empty<int>();

        public string TempFilePath { get; set; }

        public int MaxRowPerExcelFile { get; set; }

        public int MaxRowSupportExportExcel { get; set; }

        public bool IsDisableExportLargeBetList { get; set; }

        public IEnumerable<string> LicenseesWithLargeData { get; set; }

        public string CustomerClassCategoryApiUrl { get; set; }

        public IEnumerable<ReportSetting> ReportSettings { get; set; }

        public IEnumerable<FakeLicenseeSetting> FakeLicenseeSettings { get; set; }

        public DateTime HistoryDate { get; set; }

        public DateTime? MaxDate { get; set; }
    }
}