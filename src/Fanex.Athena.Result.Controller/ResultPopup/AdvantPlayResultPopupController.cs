using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class AdvantPlayResultPopupController
    {
        private readonly IBetlistResultGetterFactory _factory;

        public AdvantPlayResultPopupController(IBetlistResultGetterFactory factory)
            => _factory = factory;


        [HttpPost]
        public IActionResult Index(ThirdPartyMetaModel metaModel)
        {
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.AdvantPlayResult));
            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return new ContentResult { Content = resultApi.GetResultGET(request) };
        }
    }
}