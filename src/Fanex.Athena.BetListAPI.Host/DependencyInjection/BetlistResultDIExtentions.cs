using Fanex.Athena.Result.DataAccess;
using Fanex.Athena.Result.Service;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.AllBetCasino;
using Fanex.Result.Core.Base.Utilities;
using Fanex.Result.Core.LinkedGames;
using Fanex.Result.Core.SabaClub;
using Fanex.Result.Core.SAGaming;
using Fanex.Result.Core.SportsLottery;
using Fanex.Result.Core.WM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fanex.Athena.BetListAPI.Host.DependencyInjection
{
    public static class BetlistResultDIExtentions
    {
        public static IServiceCollection AddLapBetListResult(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBetListResultDataAccess();
            services.AddBetListResultService(configuration);
            return services;
        }

        private static IServiceCollection AddBetListResultDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IResultDataAccess, ResultDataAccess>();
            services.AddScoped<IResultNumberGameDataAccess, ResultNumberGameDataAccess>();

            return services;
        }

        private static IServiceCollection AddBetListResultService(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddScoped<ISerializer, JsonSerializer>();
            services.TryAddScoped<IWebClient, JsonWebClient>();
            services.AddScoped<IResultService, ResultService>();
            services.AddSingleton<IBetlistResultGetterFactory, BetlistResultGettterFactory>();
            services.AddAllBetResultService(configuration);
            services.AddLinkGamesResultService(configuration);
            services.AddSabaClubResultService(configuration);
            services.AddSAGamingResultService(configuration);
            services.AddSportLotteryResultService(configuration);
            services.AddScoped<IKenoLotteryTGResultFactory, KenoLotteryTGResultApi>();
            services.AddWMResultService(configuration);

            return services;
        }

        private static IServiceCollection AddAllBetResultService(this IServiceCollection services, IConfiguration configuration)
        {
            var path = configuration.GetSection("AllbetResultImagePath").Get<string>();
            var isUseOriginalPath = configuration.GetSection("AllbetUseOriginalPath").Get<bool>();
            services.AddSingleton(new AllbetConfigs { ImagePath = path, UseOriginalPath = isUseOriginalPath });
            services.AddScoped<IAllbetResultBuilder, AllbetResultBuilder>();
            return services;
        }

        private static IServiceCollection AddLinkGamesResultService(this IServiceCollection services, IConfiguration configuration)
        {
            var happy5Config = configuration.GetSection("ResultConfigs:Soccer5Result").Get<LinkedGamesConfigs>();
            services.AddSingleton(happy5Config);
            services.AddScoped<ILinkedGamesResult, LinkedGamesResult>();
            return services;
        }

        private static IServiceCollection AddSabaClubResultService(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ResultConfigs:SabaClubResult").Get<SabaClubConfigs>();
            services.AddSingleton(config);
            services.AddScoped<ISabaClubResult, SabaClubResult>();
            return services;
        }

        private static IServiceCollection AddSAGamingResultService(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<ISAGamingResultBuilder>(_ => new SAGamingResultBuilder(configuration.GetSection("SAGamingResultImagePath").Value));
            return service;
        }

        private static IServiceCollection AddWMResultService(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped<IWMResultBuilder>(_ => new WMResultBuilder(configuration.GetSection("WMResultImagePath").Value));
            return service;
        }

        private static IServiceCollection AddSportLotteryResultService(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ResultConfigs:SportsLotteryResult").Get<SportsLotteryConfigs>();
            services.AddScoped<ISportsLotteryResult>(_ => new SportsLotteryResult(config));
            return services;
        }
    }
}
