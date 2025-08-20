namespace Fanex.Athena.Common.Services.Interfaces
{
    using Model.DTO;

    public interface ILicenseeSettingService
    {
        bool IsFakeLicensee(string licenseeName);

        bool IsAuthorizedOnBetList(string licenseeName);

        FakeLicenseeSetting GetFakeLicenseeSetting(string licenseeName);
    }
}
