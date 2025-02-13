using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class VirtualGamesResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public VirtualGamesResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        public string Index(ThirdPartyMetaModel metaModel)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.VirtualGamesDetailResult));

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode);

            return resultApi.GetResultGET(request);
        }
    }
}
