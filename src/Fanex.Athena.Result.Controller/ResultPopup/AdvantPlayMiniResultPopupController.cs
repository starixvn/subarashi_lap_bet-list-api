using Fanex.Result.Core.ThirdParty.Models;
using Fanex.Result.Core.ThirdParty;
using Microsoft.AspNetCore.Mvc;
using Fanex.Athena.Result.Controller.MetaModel;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    public class AdvantPlayMiniResultPopupController : ControllerBase
    {      
        private readonly IThirdPartyResult _resultApi;

        public AdvantPlayMiniResultPopupController(IThirdPartyResult resultApi)
        {
            _resultApi = resultApi ?? new ThirdPartyResult("AdvantPlayMiniResultConfigs");
        }

        [HttpPost]
        public string Index(ThirdPartyMetaModel metaModel)
        {
            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                BetTypeId = metaModel.BetTypeId
            };

            return _resultApi.GetResultGET(request);
        }
    }
}
