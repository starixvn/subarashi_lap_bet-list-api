namespace Fanex.Athena.Common.Utility.Services.Interfaces
{
    using System.Collections.Generic;

    public interface IAppSettings
    {
        bool IsProduction { get; }

        IEnumerable<int> FakeLicenseeList { get; }

        string TempFilePath { get; }

        bool IsDisableExportLargeBetList { get; }

        IEnumerable<string> LicenseesWithLargeData { get; }

        string CustomerClassCategoryApiUrl { get; }

        int MaxRowSupportExportExcel { get; }

        int MaxRowPerExcelFile { get; }
    }
}