namespace Fanex.Athena.Common.Utility.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Athena.Common.Model.DTO;

    public interface IAppSettings
    {
        bool IsProduction { get; }

        IEnumerable<int> FakeLicenseeList { get; }

        string TempFilePath { get; }

        IEnumerable<ReportSetting> ReportSettings { get; }

        IEnumerable<FakeLicenseeSetting> FakeLicenseeSettings { get; }

        string CustomerClassCategoryApiUrl { get; }

        int MaxRowPerExcelFile { get; }

        int MaxTicketLimit { get; }

        int MaxRowForMultipleExcelFiles { get; }

        DateTime HistoryDate { get; }

        DateTime? MaxDate { get; }
    }
}