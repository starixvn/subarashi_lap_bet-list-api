namespace Fanex.Athena.Result.Controller.ResultPopup
{
    using Fanex.Result.Core.SportsLottery;
    using Fanex.Result.Core.ThirdParty.Models;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
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
        public IActionResult Index(ThirdPartyMetaModel metaModel)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.SportsLotteryDetailResult));

            const string defaultPlatform = "D";

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                Platform = defaultPlatform,
                BetTypeId = metaModel.BetTypeId
            };

            return new ContentResult { Content = resultApi.GetResultGET(request) };
        }

        [HttpPost]
        public IActionResult ResultPage(SportsLotteryResultMetaModel model)
        {
            var isDesktop = string.Equals(model.Platform, "d", StringComparison.OrdinalIgnoreCase);
            return new ContentResult
            {
                Content = _sportsLotteryResult.GetResultPage(isDesktop, model.LanguageCode)
            };
        }
    }
}
