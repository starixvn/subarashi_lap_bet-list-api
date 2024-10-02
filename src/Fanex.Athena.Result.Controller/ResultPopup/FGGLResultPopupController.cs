using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    public class FGGLResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public FGGLResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var _resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.FGGResult));

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId,
                EndTime = metaModel.WinLostDate
            };

            return _resultApi.GetResultGET(request);
        }
    }
}
