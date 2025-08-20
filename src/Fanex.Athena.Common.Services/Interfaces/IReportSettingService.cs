namespace Fanex.Athena.Common.Services.Interfaces
{
    using Model.DTO;

    public interface IReportSettingService
    {
        ReportSetting GetSetting(string reportKey, string licenseeName);
    }
}
