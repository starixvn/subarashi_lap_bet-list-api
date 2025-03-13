using AutoFixture;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using NSubstitute;

namespace Fanex.Athena.Common.Service.UnitTests
{
    public class ReportSettingServiceTests
    {
        private IMemoryCacheService _cacheMock;
        private IAppSettings _appSettingsMock;
        private IReportSettingService _reportSettingService;

        [SetUp]
        public void SetUp()
        {
            _cacheMock = Substitute.For<IMemoryCacheService>();
            _appSettingsMock = Substitute.For<IAppSettings>();
            _reportSettingService = new ReportSettingService(_cacheMock, _appSettingsMock);
        }

        [TearDown]
        public void TearDown()
        {
            // Code for tear down
            _cacheMock = null;
            _appSettingsMock = null;
            _reportSettingService = null;
        }

        [Test]
        public void GetSetting_WhenSettingsInCache_ReturnsReportSetting()
        {
            // Arrange
            var reportKey = "ReportKey";
            var licenseeName = "LicenseeName";
            var settings = new Fixture().CreateMany<ReportSetting>(1).ToList();
            settings[0].ReportKey = reportKey;
            settings[0].MaxDateString = "tomorrow";
            settings[0].MinDateString = "2022-01-01";
            _cacheMock.Get<IEnumerable<ReportSetting>>("LRF_ReportSettings").Returns(settings);

            // Act
            var result = _reportSettingService.GetSetting(reportKey, licenseeName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReportKey, Is.EqualTo(reportKey));
            _cacheMock.Received(1).Get<IEnumerable<ReportSetting>>("LRF_ReportSettings");
            _cacheMock.DidNotReceive().Set("LRF_ReportSettings", Arg.Any<IEnumerable<ReportSetting>>());
        }

        [Test]
        public void GetSetting_WhenSettingsNotInCache_ReturnsReportSetting()
        {
            // Arrange
            var reportKey = "ReportKey";
            var licenseeName = "LicenseeName";
            var settings = new Fixture().CreateMany<ReportSetting>(1).ToList();
            settings[0].ReportKey = reportKey;
            settings[0].MaxDateString = "today";
            settings[0].MinDateString = "yesterday";
            _cacheMock.Get<IEnumerable<ReportSetting>>("LRF_ReportSettings").Returns((IEnumerable<ReportSetting>)null);
            _appSettingsMock.ReportSettings.Returns(settings);

            // Act
            var result = _reportSettingService.GetSetting(reportKey, licenseeName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReportKey, Is.EqualTo(reportKey));
            _cacheMock.Received(1).Get<IEnumerable<ReportSetting>>("LRF_ReportSettings");
            _cacheMock.Received(1).Set("LRF_ReportSettings", settings);
        }

        [Test]
        public void GetSetting_WhenReportSettingFound_AssignsFakeLicenseeSetting()
        {
            // Arrange
            var reportKey = "ReportKey";
            var licenseeName = "LicenseeName";
            var settings = new List<ReportSetting>
                    {
                        new ReportSetting { ReportKey = reportKey }
                    };
            var fakeLicenseeSetting = new FakeLicenseeSetting
            {
                SiteName = licenseeName,
                MinDate = "",
                MaxDate = "",
                DisableBetList = true,
                BaseCurrencies = "USD,VND",
                LimitRecentMonths = 6,
                TimeZone = 3
            };
            _cacheMock.Get<IEnumerable<ReportSetting>>("LRF_ReportSettings").Returns(settings);
            _appSettingsMock.FakeLicenseeSettings.Returns(new List<FakeLicenseeSetting> { fakeLicenseeSetting });

            // Act
            var result = _reportSettingService.GetSetting(reportKey, licenseeName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReportKey, Is.EqualTo(reportKey));
            Assert.That(result.MinDateString, Is.EqualTo(fakeLicenseeSetting.MinDate));
            Assert.That(result.MaxDateString, Is.EqualTo(fakeLicenseeSetting.MaxDate));
            Assert.That(result.IsDisableBetList, Is.EqualTo(fakeLicenseeSetting.DisableBetList));
            Assert.That(result.BaseCurrencies, Is.EqualTo(fakeLicenseeSetting.BaseCurrencies));
            Assert.That(result.LimitRecentMonths, Is.EqualTo(fakeLicenseeSetting.LimitRecentMonths));
            Assert.That(result.TimeZone, Is.EqualTo(fakeLicenseeSetting.TimeZone));
            _cacheMock.Received(1).Get<IEnumerable<ReportSetting>>("LRF_ReportSettings");
            _cacheMock.DidNotReceive().Set("LRF_ReportSettings", Arg.Any<IEnumerable<ReportSetting>>());
        }

        [Test]
        public void GetSetting_WhenReportSettingNotFound_ReturnsNull()
        {
            // Arrange
            var reportKey = "ReportKey";
            var licenseeName = "LicenseeName";
            var settings = new List<ReportSetting>
                    {
                        new ReportSetting { ReportKey = "OtherReportKey" }
                    };
            _cacheMock.Get<IEnumerable<ReportSetting>>("LRF_ReportSettings").Returns(settings);

            // Act
            var result = _reportSettingService.GetSetting(reportKey, licenseeName);

            // Assert
            Assert.That(result, Is.Null);
            _cacheMock.Received(1).Get<IEnumerable<ReportSetting>>("LRF_ReportSettings");
            _cacheMock.DidNotReceive().Set("LRF_ReportSettings", Arg.Any<IEnumerable<ReportSetting>>());
        }
    }
}
