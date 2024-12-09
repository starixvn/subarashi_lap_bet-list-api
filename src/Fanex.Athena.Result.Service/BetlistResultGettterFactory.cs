using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.ThirdParty;
using Microsoft.Extensions.Configuration;

namespace Fanex.Athena.Result.Service
{
    public class BetlistResultGettterFactory(IConfiguration configuration) : IBetlistResultGetterFactory
    {
        public IThirdPartyResult CreateThirdPartyResultGetter(string configName)
        {
            var config = configuration.GetSection(configName).Get<ThirdPartyConfigs>();
            return new ThirdPartyResult(config);
        }
    }
}
