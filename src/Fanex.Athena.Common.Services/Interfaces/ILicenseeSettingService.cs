using Fanex.Athena.Model.Core.DTO.Configuration;


namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface ILicenseeSettingService
    {
        bool IsFakeLicensee(string licenseeName);

        bool IsAuthorizedOnBetList(string licenseeName);

        FakeLicenseeSetting GetFakeLicenseeSetting(string licenseeName);
    }
}
