using Fanex.Result.Core.SAGaming;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class SAGamingResultPopupController
    {
        private readonly ISAGamingResultBuilder _resultApi;

        public SAGamingResultPopupController(ISAGamingResultBuilder resultApi)
            => _resultApi = resultApi;

        [HttpPost]
        public string Index(SAGamingResultMetaModel model)
            => _resultApi.Render(model.TransDesc, model.MatchCode, model.BetType, model.LanguageCode);

        public class SAGamingResultMetaModel
        {
            public string TransDesc { get; set; }

            public string MatchCode { get; set; }

            public string BetType { get; set; }

            public string LanguageCode { get; set; }
        }
    }
}
