namespace Fanex.Athena.Result.Controller.ResultPopup
{
    using Fanex.Result.Core.ThirdParty.Models;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class VirtualGamesResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public VirtualGamesResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;

        public IActionResult Index(ThirdPartyMetaModel metaModel)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.VirtualGamesDetailResult));

            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode);

            return new ContentResult { Content = resultApi.GetResultGET(request) };
        }
    }
}
