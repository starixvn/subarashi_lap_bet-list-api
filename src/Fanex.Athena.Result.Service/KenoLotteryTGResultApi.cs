namespace Fanex.Athena.Result.Service
{
    using Fanex.Athena.Result.Model.Const;
    using Fanex.Athena.Result.Service.Interfaces;
    using Fanex.Result.Core.KenoLotteryTG;
    using Microsoft.Extensions.Configuration;

    public class KenoLotteryTGResultApi : IKenoLotteryTGResultFactory
    {
        private const string PREFIX = "KenoConfigs:";

        private const string KENO_RESULT_CONFIG = "KenoResult";
        private const string LOTTERY_RESULT_CONFIG = "LotteryResult";
        private const string TABLEGAME_RESULT_CONFIG = "TableGameResult";
        private const string VIRTUALGAMES_RESULT_CONFIG = "VirtualGamesResult";

        private readonly IConfiguration _configuration;

        public KenoLotteryTGResultApi(IConfiguration configuration)
            => _configuration = configuration;

        public IKenoLotteryTGResult CreateResultApi(int vendorProductId)
        {
            var config = _configuration.GetSection(PREFIX + GetConfigSetting(vendorProductId)).Get<KenoLotteryTGConfigs>();
            var result = new KenoLotteryTGResult(config);
            return result;
        }

        private static string GetConfigSetting(int vendorProductId)
        {
            switch (vendorProductId)
            {
                case ResultVendorProductTypes.Lottery:
                    return LOTTERY_RESULT_CONFIG;

                case ResultVendorProductTypes.Table:
                    return TABLEGAME_RESULT_CONFIG;

                case ResultVendorProductTypes.VirtualGames:
                    return VIRTUALGAMES_RESULT_CONFIG;

                default:
                    return KENO_RESULT_CONFIG;
            }
        }
    }
}