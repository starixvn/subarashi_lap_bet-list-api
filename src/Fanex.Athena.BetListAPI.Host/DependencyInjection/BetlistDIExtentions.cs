using Fanex.Athena.BetList.Controllers.Utility;
using Fanex.Athena.BetList.DataAccess;
using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Builders;
using Fanex.Athena.BetList.Service;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.DataAccess;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Internal;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.TicketHelper;
using Fanex.Core.Utils;
using Fanex.RemotingRef;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Fanex.Athena.BetListAPI.Host.DependencyInjection
{
    public static class BetlistDIExtentions
    {
        public static IServiceCollection AddLapBetList(this IServiceCollection services)
        {
            services.AddCommonDataAccess()
                .AddCommonServices()
                .AddBetListDataAccess()
                .AddBetListServices();
            return services;
        }

        private static IServiceCollection AddCommonDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IBalanceUpDataAccess, BalanceUpDataAccess>();
            services.AddScoped<ICurrencyDataAccess, CurrencyDataAccess>();
            services.AddScoped<IAuthorizationDataAccess, AuthorizationDataAccess>();
            return services;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IAppSettings>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                var settings = config.GetSection("AppSettings").Get<AppSettings>();
                settings.MaxDate ??= DateTimeHelper.Now;
                return settings;
            });
            services.AddScoped<IMetaModelBindingService, BaseMetaModelBindingService>();
            services.AddScoped<ICurrencyValidation, CurrencyValidation>();
            services.AddScoped<ICustomMemoryCache, CustomMemoryCache>();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<ITempFileService, TempFileService>();
            services.AddSingleton<IShardInfoAccessor, ShardInfoAccessor>();
            services.AddSingleton<ISiteDBAccessor, SiteDBAccessor>();
            services.AddScoped<IContextService, ContextService>();
            services.AddScoped<ICacheRefAdapter, CacheRefAdapter>();
            services.AddScoped<ITemplateBuilder, TemplateBuilder>();
            services.AddScoped<ICustomerClassService, CustomerClassService>();
            services.AddScoped<ISportService, SportService>();
            services.AddScoped<ISubsidiaryService, SubsidiaryService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<ILicenseeSettingService, LicenseeSettingService>();
            services.AddScoped<IReportSettingService, ReportSettingService>();

            return services;
        }

        private static IServiceCollection AddBetListDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IBetListByProductDataAccess, BetListByProductDataAccess>();
            services.AddScoped<IBetListDataAccess, BetListDataAccess>();
            services.AddScoped<IBettingTicketCenterAdvancedSearchDataAccess, BettingTicketCenterAdvancedSearchDataAccess>();
            services.AddScoped<IBettingTicketCenterDataAccess, BettingTicketCenterDataAccess>();
            services.AddScoped<ICashOutBetListDataAccess, CashOutBetListDataAccess>();
            services.AddScoped<IExchangeBetListDataAccess, ExchangeBetListDataAccess>();
            services.AddScoped<IIPDataAccess, IPDataAccess>();
            services.AddScoped<ILastBetsMonitoringBetListDataAccess, LastBetsMonitoringBetListDataAccess>();
            services.AddScoped<IPlayerAdminBetListDataAccess, PlayerAdminBetListDataAccess>();
            services.AddScoped<ISingleMatchParlayDetailDataAccess, SingleMatchParlayDetailDataAccess>();
            services.AddScoped<ISystemParlayDetailDataAccess, SystemParlayDetailDataAccess>();
            return services;
        }

        private static IServiceCollection AddBetListServices(this IServiceCollection services)
        {
            services.AddScoped<IBetListByProductService, BetListByProductService>();
            services.AddScoped<IBetListService, BetListService>();
            services.AddScoped<IExchangeBetListService, ExchangeBetListService>();
            services.AddScoped<IIpInfoService, IpInfoService>();
            services.AddScoped<IIPService, IPService>();
            services.AddScoped<ILastBetsMonitoringBetListService, LastBetsMonitoringBetListService>();
            services.AddScoped<ISystemParlayDetailService, SystemParlayDetailService>();
            services.AddScoped<ITicketBuilderService, TicketBuilderService>();
            services.AddScoped<IExcelBuilder, ExcelBuilder>();
            services.AddScoped<IBetListHTMLBuilder, BetListHTMLBuilder>();
            services.AddScoped<IBetListExcelBuilder, BetListExcelBuilder>();
            services.AddScoped<IBettingTicketCenterExcelBuilder, BettingTicketCenterExcelBuilder>();
            services.AddScoped<IBettingTicketCenterExcel, BettingTicketCenterExcel>();
            services.AddScoped<IBettingTicketCenterZipBuilder, BettingTicketCenterZipBuilder>();
            services.AddScoped<ITicketHelper, TicketHelper>();
            services.AddSingleton<IRemotingRefService, RemotingRefService>();
            services.AddScoped<IExchangeRunningExcel, ExchangeRunningExcel>();
            services.AddScoped<IExchangeWinLossExcel, ExchangeWinLossExcel>();
            services.AddScoped<IBettingTicketCenterSportService, BettingTicketCenterSportService>();
            return services;
        }
    }
}
