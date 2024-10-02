namespace Fanex.Athena.Common.Utility.Services.Interfaces
{
    using Fanex.Athena.Common.Model.DTO;
    using System.Collections.Generic;

    public interface IAppSettings
    {
        bool IsProduction { get; }

        IEnumerable<int> FakeLicenseeList { get; }

        string TempFilePath { get; }

        bool IsDisableExportLargeBetList { get; }

        IEnumerable<string> LicenseesWithLargeData { get; }

        IEnumerable<ReportSetting> ReportSettings { get; }

        IEnumerable<FakeLicenseeSetting> FakeLicenseeSettings { get; }

        string CustomerClassCategoryApiUrl { get; }

        int MaxRowSupportExportExcel { get; }

        int MaxRowPerExcelFile { get; }

        DateTime HistoryDate { get; }

        DateTime? MaxDate { get; }
    }
}