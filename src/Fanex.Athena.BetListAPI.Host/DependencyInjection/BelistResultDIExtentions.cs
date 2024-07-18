using Fanex.Athena.Result.DataAccess;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.Result.Service;
using Fanex.Result.Core.LinkedGames;
using Fanex.Result.Core.Base.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Fanex.Result.Core.SabaClub;
using Fanex.Result.Core.SAGaming;
using Fanex.Result.Core.AllBetCasino;
using Fanex.Result.Core.SportsLottery;
using Fanex.Result.Core.SexyGaming;

namespace Fanex.Athena.BetListAPI.Site.DependencyInjection
{
    public static class BelistResultDIExtentions
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
            services.AddSAGamingResultService();
            services.AddSportLotteryResultService(configuration);
            services.AddSexyGamingResultService();
            services.AddScoped<IKenoApi, KenoApi>();
            services.AddScoped<IKenoLotteryTGResultFactory, KenoLotteryTGResultApi>();

            return services;
        }

        private static IServiceCollection AddAllBetResultService(this IServiceCollection services, IConfiguration configuration)
        {
            var path = configuration.GetSection("AllbetResultImagePath").Get<string>();
            var isUseOriginalPath = configuration.GetSection("AllbetUseOriginalPath").Get<bool>();
            services.AddSingleton(new AllbetConfigs { ImagePath = path, UseOriginalPath = isUseOriginalPath});
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

        private static IServiceCollection AddSAGamingResultService(this IServiceCollection service)
        {
            service.AddScoped<ISAGamingResultBuilder, SAGamingResultBuilder>();
            return service;
        }

        private static IServiceCollection AddSportLotteryResultService(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ResultConfigs:SportsLotteryResult").Get<SportsLotteryConfigs>();
            services.AddSingleton(config);
            services.AddScoped<ISportsLotteryResult, SportsLotteryResult>();
            return services;
        }

        private static IServiceCollection AddSexyGamingResultService(this IServiceCollection services)
        {
            services.AddScoped<ISexyGamingResultBuilder, SexyGamingResultBuilder>();
            return services;
        }
    }
}
