namespace Fanex.Athena.Result.Controller.ResultPopup
{
    using Fanex.Result.Core.ThirdParty.Models;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class GPIResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public GPIResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        [HttpPost]
        public IActionResult Index(ThirdPartyMetaModel metaModel)
        {
            var _resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.GPIResult));

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return new ContentResult { Content = _resultApi.GetResultGET(request) };
        }
    }
}
