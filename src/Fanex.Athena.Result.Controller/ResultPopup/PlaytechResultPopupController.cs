namespace Fanex.Athena.Result.Controller.ResultPopup
{
    using Fanex.Result.Core.ThirdParty.Models;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class PlaytechResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public PlaytechResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        [HttpPost]
        public IActionResult Index(ThirdPartyMetaModel metaModel)
        {
            var _resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.PlaytechResult));

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return new ContentResult { Content = _resultApi.GetResultPOST(request) };
        }
    }
}