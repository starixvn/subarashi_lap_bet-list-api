namespace Fanex.Athena.Result.Controller.ResultPopup
{
    using Fanex.Result.Core.WCasino;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class WCasinoResultPopupController
    {
        private readonly IWCasinoResultBuilder _resultApi;

        public WCasinoResultPopupController(IWCasinoResultBuilder resultApi)
            => _resultApi = resultApi;

        [HttpPost]
        public IActionResult Index(WCasinoResultMetaModel model)
            => new ContentResult
            {
                Content = _resultApi.Render(model.TransDesc, model.GameId3rd, model.BetType, model.LanguageCode)
            };

        public class WCasinoResultMetaModel
        {
            public string TransDesc { get; set; }

            public string GameId3rd { get; set; }

            public int BetType { get; set; }

            public string LanguageCode { get; set; }
        }
    }
}