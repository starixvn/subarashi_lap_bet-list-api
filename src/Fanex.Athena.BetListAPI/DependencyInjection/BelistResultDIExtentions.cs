using Fanex.Athena.Result.DataAccess;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.Result.Service;
using Fanex.Result.Core.LinkedGames;
using Fanex.Result.Core.Base.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Fanex.Result.Core.SabaClub;

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
            services.AddLinkGamesResultService(configuration);
            services.AddSabaClubService(configuration);
            services.AddScoped<IKenoApi, KenoApi>();

            return services;
        }

        private static IServiceCollection AddLinkGamesResultService(this IServiceCollection services, IConfiguration configuration)
        {
            var happy5Config = configuration.GetSection("ResultConfigs:Soccer5Result").Get<LinkedGamesResult>();
            services.AddSingleton(happy5Config);
            services.AddScoped<ILinkedGamesResult, LinkedGamesResult>();
            return services;
        }

        private static IServiceCollection AddSabaClubService(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ResultConfigs:SabaClubResult").Get<SabaClubConfigs>();
            services.AddSingleton(config);
            services.AddScoped<ISabaClubResult, SabaClubResult>();
            return services;
        }
    }
}
