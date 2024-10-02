using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;


namespace Fanex.Athena.Common.Services
{
    public class LicenseeSettingService : ILicenseeSettingService
    {
        private readonly IAppSettings _appSettings;

        public LicenseeSettingService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public FakeLicenseeSetting GetFakeLicenseeSetting(string licenseeName)
        {
            return _appSettings.FakeLicenseeSettings.FirstOrDefault(x => x.SiteName.Equals(licenseeName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsAuthorizedOnBetList(string licenseeName)
        {
            var authorized = !string.IsNullOrEmpty(licenseeName);
            if (authorized)
            {
                var setting = GetFakeLicenseeSetting(licenseeName);
                if (setting != null)
                {
                    authorized = !setting.DisableBetList;
                }
            }

            return authorized;
        }

        public bool IsFakeLicensee(string licenseeName)
           => GetFakeLicenseeSetting(licenseeName) != null;
    }
}
