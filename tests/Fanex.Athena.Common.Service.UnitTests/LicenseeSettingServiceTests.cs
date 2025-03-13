using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using NSubstitute;

namespace Fanex.Athena.Common.Service.UnitTests
{
    public class LicenseeSettingServiceTests
    {
        private IAppSettings _appSettingsMock;
        private LicenseeSettingService _licenseeSettingService;

        [SetUp]
        public void Setup()
        {
            _appSettingsMock = Substitute.For<IAppSettings>();
            _licenseeSettingService = new LicenseeSettingService(_appSettingsMock);
        }

        [TearDown]
        public void TearDown()
        {
            _appSettingsMock = null;
            _licenseeSettingService = null;
        }

        [Test]
        public void GetFakeLicenseeSetting_ShouldReturnCorrectSetting_WhenLicenseeExists()
        {
            // Arrange
            var licenseeName = "Licensee1";
            var fakeLicenseeSettings = new List<FakeLicenseeSetting>
                {
                    new FakeLicenseeSetting { SiteName = "Licensee1", DisableBetList = false },
                    new FakeLicenseeSetting { SiteName = "Licensee2", DisableBetList = true }
                };
            _appSettingsMock.FakeLicenseeSettings.Returns(fakeLicenseeSettings);

            // Act
            var result = _licenseeSettingService.GetFakeLicenseeSetting(licenseeName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SiteName, Is.EqualTo(licenseeName));
        }

        [Test]
        public void GetFakeLicenseeSetting_ShouldReturnNull_WhenLicenseeDoesNotExist()
        {
            // Arrange
            var licenseeName = "Licensee3";
            var fakeLicenseeSettings = new List<FakeLicenseeSetting>
                {
                    new FakeLicenseeSetting { SiteName = "Licensee1", DisableBetList = false },
                    new FakeLicenseeSetting { SiteName = "Licensee2", DisableBetList = true }
                };
            _appSettingsMock.FakeLicenseeSettings.Returns(fakeLicenseeSettings);

            // Act
            var result = _licenseeSettingService.GetFakeLicenseeSetting(licenseeName);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void IsAuthorizedOnBetList_ShouldReturnTrue_WhenLicenseeIsAuthorized()
        {
            // Arrange
            var licenseeName = "Licensee1";
            var fakeLicenseeSettings = new List<FakeLicenseeSetting>
                {
                    new FakeLicenseeSetting { SiteName = "Licensee1", DisableBetList = false },
                    new FakeLicenseeSetting { SiteName = "Licensee2", DisableBetList = true }
                };
            _appSettingsMock.FakeLicenseeSettings.Returns(fakeLicenseeSettings);

            // Act
            var result = _licenseeSettingService.IsAuthorizedOnBetList(licenseeName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsAuthorizedOnBetList_ShouldReturnFalse_WhenLicenseeIsNotAuthorized()
        {
            // Arrange
            var licenseeName = "Licensee2";
            var fakeLicenseeSettings = new List<FakeLicenseeSetting>
                {
                    new FakeLicenseeSetting { SiteName = "Licensee1", DisableBetList = false },
                    new FakeLicenseeSetting { SiteName = "Licensee2", DisableBetList = true }
                };
            _appSettingsMock.FakeLicenseeSettings.Returns(fakeLicenseeSettings);

            // Act
            var result = _licenseeSettingService.IsAuthorizedOnBetList(licenseeName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsAuthorizedOnBetList_ShouldReturnFalse_WhenLicenseeNameIsEmpty()
        {
            // Arrange
            var licenseeName = "";
            var fakeLicenseeSettings = new List<FakeLicenseeSetting>
                {
                    new FakeLicenseeSetting { SiteName = "Licensee1", DisableBetList = false },
                    new FakeLicenseeSetting { SiteName = "Licensee2", DisableBetList = true }
                };
            _appSettingsMock.FakeLicenseeSettings.Returns(fakeLicenseeSettings);

            // Act
            var result = _licenseeSettingService.IsAuthorizedOnBetList(licenseeName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsFakeLicensee_ShouldReturnTrue_WhenLicenseeExists()
        {
            // Arrange
            var licenseeName = "Licensee1";
            var fakeLicenseeSettings = new List<FakeLicenseeSetting>
                {
                    new FakeLicenseeSetting { SiteName = "Licensee1", DisableBetList = false },
                    new FakeLicenseeSetting { SiteName = "Licensee2", DisableBetList = true }
                };
            _appSettingsMock.FakeLicenseeSettings.Returns(fakeLicenseeSettings);

            // Act
            var result = _licenseeSettingService.IsFakeLicensee(licenseeName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsFakeLicensee_ShouldReturnFalse_WhenLicenseeDoesNotExist()
        {
            // Arrange
            var licenseeName = "Licensee3";
            var fakeLicenseeSettings = new List<FakeLicenseeSetting>
                {
                    new FakeLicenseeSetting { SiteName = "Licensee1", DisableBetList = false },
                    new FakeLicenseeSetting { SiteName = "Licensee2", DisableBetList = true }
                };
            _appSettingsMock.FakeLicenseeSettings.Returns(fakeLicenseeSettings);

            // Act
            var result = _licenseeSettingService.IsFakeLicensee(licenseeName);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
