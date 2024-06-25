using Fanex.Athena.Result.Controller.MetaModel;
using Fanex.Result.Core.ThirdParty.Models;
using Fanex.Result.Core.ThirdParty;
using Microsoft.AspNetCore.Mvc;
using Fanex.Athena.Result.Service.Interfaces;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    public class SealEntertainmentResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public SealEntertainmentResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.SealEntertainmentResult));

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return resultApi.GetResultGET(request);
        }
    }
}
