using Fanex.Athena.Result.Controller.MetaModel;
using Fanex.Result.Core.SportsLottery;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;
using Fanex.Athena.Result.Service.Interfaces;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SportsLotteryResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;
        private readonly ISportsLotteryResult _sportsLotteryResult;

        public SportsLotteryResultPopupController(IBetlistResultGetterFactory factory, ISportsLotteryResult sportsLotteryResult)
        {
            _factory = factory;
            _sportsLotteryResult = sportsLotteryResult;
        }

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.SportsLotteryDetailResult));

            const string defaultPlatform = "D";

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                Platform = defaultPlatform,
                BetTypeId = metaModel.BetTypeId
            };

            return resultApi.GetResultGET(request);
        }

        [HttpPost]
        public string ResultPage(string platform, string languageCode)
        {
            var isDesktop = string.Equals(platform, "d", StringComparison.OrdinalIgnoreCase);
            return _sportsLotteryResult.GetResultPage(isDesktop, languageCode);
        }
    }
}
