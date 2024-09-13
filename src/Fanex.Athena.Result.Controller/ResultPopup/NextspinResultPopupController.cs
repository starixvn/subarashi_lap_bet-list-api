using Fanex.Athena.Result.Controller.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class NextspinResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public NextspinResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var _resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.NextspinResult));

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return _resultApi.GetResultGET(request);
        }
    }
}