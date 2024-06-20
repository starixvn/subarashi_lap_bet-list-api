namespace Fanex.Athena.Result.Service
{
    using Fanex.Athena.Result.Model.Const;
    using Fanex.Athena.Result.Service.Interfaces;
    using Fanex.Result.Core.ConfigurationHelper;
    using Fanex.Result.Core.KenoLotteryTG;

    public class KenoLotteryTGResultApi : IKenoLotteryTGResultFactory
    {
        private const string KENO_RESULT_CONFIG = "KenoResultConfigs";
        private const string LOTTERY_RESULT_CONFIG = "LotteryResultConfigs";
        private const string TABLEGAME_RESULT_CONFIG = "TableGameResultConfigs";
        private const string VIRTUALGAMES_RESULT_CONFIG = "VirtualGamesResultConfigs";

        public IKenoLotteryTGResult CreateResultApi(int vendorProductId)
        {
            var configurationHelper = new ConfigurationHelper();
            var result = new KenoLotteryTGResult(GetConfigSetting(vendorProductId), configurationHelper);
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