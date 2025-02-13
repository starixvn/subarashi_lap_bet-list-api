using Fanex.Result.Core.WM;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class WMResultPopupController
    {
        private readonly IWMResultBuilder _resultApi;

        public WMResultPopupController(IWMResultBuilder resultApi)
            => _resultApi = resultApi;


        [HttpPost]
        public string Index(WMResultMetaModel model)
            => _resultApi.Render(model.TransDesc, model.GameId3rd, model.BetType, model.LanguageCode);

        public class WMResultMetaModel
        {
            public string TransDesc { get; set; }

            public string GameId3rd { get; set; }

            public int BetType { get; set; }

            public string LanguageCode { get; set; }
        }

    }
}
