using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Extension.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;

namespace Fanex.Athena.Common.Services
{
    public class ReportSettingService : IReportSettingService
    {
        private const string CacheKey = "LRF_ReportSettings";

        private readonly IMemoryCacheService _cache;
        private readonly IAppSettings _appSettings;


        public ReportSettingService(
            IMemoryCacheService cache,
            IAppSettings appSettings,
            IWebHostEnvironment hostEnviroment)           
        {
            _cache = cache;
            _appSettings = appSettings;
        }

        public ReportSetting GetSetting(string reportKey, string licenseeName)
        {
            var settings = _cache.Get<IEnumerable<ReportSetting>>(CacheKey);
            if (settings == null)
            {
                settings = GetSettingList();
                _cache.Set(CacheKey, settings);
            }

            var reportSetting = settings.FirstOrDefault(x => x.ReportKey.Equals(reportKey, StringComparison.OrdinalIgnoreCase));
            if (reportSetting != null)
            {
                AssignFakeLicenseeSetting(reportSetting, licenseeName);
                AssignDateRange(reportSetting);
            }

            return reportSetting;
        }

        private static DateTime GetNowByTimeZone(int? timeZone)
        {
            return timeZone.HasValue ? DateTimeHelper.UtcNow.AddHours(timeZone.Value) : DateTimeHelper.Now;
        }

        private static DateTime ConvertToDate(string datetime, int? timeZone, DateTime? defaultDate)
        {
            if (string.IsNullOrEmpty(datetime))
            {
                return defaultDate.HasValue ? defaultDate.Value : GetNowByTimeZone(timeZone);
            }

            if (datetime.Equals("today", StringComparison.OrdinalIgnoreCase))
            {
                return GetNowByTimeZone(timeZone).Date;
            }

            if (datetime.Equals("yesterday", StringComparison.OrdinalIgnoreCase))
            {
                return GetNowByTimeZone(timeZone).Date.AddDays(-1);
            }

            if (datetime.Equals("tomorrow", StringComparison.OrdinalIgnoreCase))
            {
                return GetNowByTimeZone(timeZone).Date.AddDays(1);
            }

            return Convert.ToDateTime(datetime);
        }

        private void AssignDateRange(ReportSetting reportSetting)
        {
            reportSetting.MinDate = ConvertToDate(reportSetting.MinDateString, reportSetting.TimeZone, _appSettings.HistoryDate);
            reportSetting.MaxDate = ConvertToDate(reportSetting.MaxDateString, reportSetting.TimeZone, _appSettings.MaxDate ?? DateTime.Now);

            if (reportSetting.LimitRecentMonths > 0)
            {
                var limitedMinDate = GetNowByTimeZone(reportSetting.TimeZone).Date.AddMonths(-reportSetting.LimitRecentMonths + 1);
                limitedMinDate = DateTimeHelper.CreateDateTime(limitedMinDate.Year, limitedMinDate.Month, 1);
                reportSetting.MinDate = DateTimeHelper.GetMaxDate(reportSetting.MinDate, limitedMinDate);
            }

            if (reportSetting.LimitRecentDays > 0)
            {
                var supportedMinDate = reportSetting.MaxDate.Date.AddDays(-reportSetting.LimitRecentDays);
                reportSetting.MinDate = DateTimeHelper.GetMaxDate(reportSetting.MinDate, supportedMinDate);
            }
        }

        private void AssignFakeLicenseeSetting(ReportSetting reportSetting, string licenseeName)
        {
            var fakeLicenseeSetting = _appSettings.FakeLicenseeSettings.FirstOrDefault(_ => _.SiteName.Equals(licenseeName, StringComparison.OrdinalIgnoreCase));
            if (fakeLicenseeSetting != null)
            {
                reportSetting.MinDateString = fakeLicenseeSetting.MinDate;
                reportSetting.MaxDateString = fakeLicenseeSetting.MaxDate;
                reportSetting.IsDisableBetList = fakeLicenseeSetting.DisableBetList;
                reportSetting.BaseCurrencies = fakeLicenseeSetting.BaseCurrencies;
                reportSetting.LimitRecentMonths = fakeLicenseeSetting.LimitRecentMonths;
                reportSetting.TimeZone = fakeLicenseeSetting.TimeZone;
            }
        }

        private IEnumerable<ReportSetting> GetSettingList()
            => _appSettings.ReportSettings;
    
    }
}
