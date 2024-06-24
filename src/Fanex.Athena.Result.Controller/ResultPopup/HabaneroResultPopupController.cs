using Fanex.Athena.Result.Controller.MetaModel;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;
using Fanex.Athena.Result.Service.Interfaces;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    public class HabaneroResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public HabaneroResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var ressultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.HabaneroResult));
            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return ressultApi.GetResultGET(request);
        }
    }
}
