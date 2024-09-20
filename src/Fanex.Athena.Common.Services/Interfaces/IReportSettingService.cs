

using Fanex.Athena.Common.Model.DTO;

namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface IReportSettingService
    {
        ReportSetting GetSetting(string reportKey, string licenseeName);
    }
}
