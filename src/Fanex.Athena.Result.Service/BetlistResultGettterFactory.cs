namespace Fanex.Athena.Result.Service
{
    using Fanex.Result.Core.ThirdParty;
    using Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class BetlistResultGettterFactory(IConfiguration configuration, ILogger<BetlistResultGettterFactory> logger) : IBetlistResultGetterFactory
    {
        public IThirdPartyResult CreateThirdPartyResultGetter(string configName)
        {
            var config = configuration.GetSection(configName).Get<ThirdPartyConfigs>();
            string message = $"CreateThirdPartyResultGetter {configName}: {JsonConvert.SerializeObject(config)}";
            logger.LogInformation(message);
            return new ThirdPartyResult(config);
        }
    }
}
