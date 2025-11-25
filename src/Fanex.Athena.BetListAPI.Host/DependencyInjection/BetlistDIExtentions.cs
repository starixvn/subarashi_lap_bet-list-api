namespace Fanex.Athena.BetListAPI.Host.DependencyInjection
{
    using BetList.Controllers.Utility;
    using BetList.DataAccess;
    using BetList.DataAccess.Interfaces;
    using BetList.Excel;
    using BetList.Excel.Builders;
    using BetList.Service;
    using BetList.Service.Interfaces;
    using Common.DataAccess;
    using Common.DataAccess.Interfaces;
    using Common.Services;
    using Common.Services.Interfaces;
    using Common.Utility.Helpers;
    using Common.Utility.Services;
    using Common.Utility.Services.Interfaces;
    using Common.Utility.Services.Internal;
    using Core.Utils;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Builder.HybridParlayBuilder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.TicketHelper;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using RemotingRef;
    using Result.Controller.ResultService;
    using Result.Controller.ResultService.Interfaces;

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
            services.AddSingleton<ICustomMemoryCache, CustomMemoryCache>();
            services.AddSingleton<IMemoryCacheService, MemoryCacheService>();
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
            services.AddScoped<IThirdPartySportResultService, ThirdPartySportResultService>();

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
            services.AddScoped<IHybridParlayBuilder, HyBridParlayBuilder>();
            return services;
        }
    }
}