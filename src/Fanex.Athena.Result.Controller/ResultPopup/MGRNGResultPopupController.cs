using Fanex.Athena.Result.Controller.MetaModel;
using Fanex.Result.Core.ThirdParty.Models;
using Fanex.Athena.Result.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MGRNGResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public MGRNGResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var resultAPi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.MGRNGResult));
            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return resultAPi.GetResultGET(request);
        }
    }
}
