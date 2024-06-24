using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.ThirdParty;
using Microsoft.Extensions.Configuration;

namespace Fanex.Athena.Result.Service
{
    public class BetlistResultGettterFactory : IBetlistResultGetterFactory
    {
        private readonly IConfiguration Configuration;

        public BetlistResultGettterFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IThirdPartyResult CreateThirdPartyResultGetter(string configName)
        {
            var config = Configuration.GetSection(configName).Get<ThirdPartyConfigs>();
            return new ThirdPartyResult(config);
        }
    }
}
