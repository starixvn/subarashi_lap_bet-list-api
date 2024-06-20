using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.DataAccess;
using Fanex.Athena.Result.DataAccess;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.Result.Service;

namespace Fanex.Athena.BetListAPI.Site.DependencyInjection
{
    public static class BelistResultDIExtentions
    {
        public static IServiceCollection AddLapBetListResult(this IServiceCollection services)
        {
            services.AddBetListResultDataAccess();
            return services;
        }

        private static IServiceCollection AddBetListResultDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IResultDataAccess, ResultDataAccess>();
            services.AddScoped<IResultNumberGameDataAccess, ResultNumberGameDataAccess>();

            return services;
        }

        private static IServiceCollection AddBetListResultService(this IServiceCollection services)
        {
            services.AddScoped<IKenoApi, KenoApi>();
            services.AddScoped

            return services;
        }
    }
}
