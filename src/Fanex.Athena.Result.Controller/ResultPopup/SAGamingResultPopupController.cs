using Fanex.Result.Core.SAGaming;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SAGamingResultPopupController
    {
        private readonly ISAGamingResultBuilder _resultApi;

        public SAGamingResultPopupController(ISAGamingResultBuilder resultApi)
            => _resultApi = resultApi;

        [HttpPost]
        public string Index(string transDesc, string matchCode, string betType, string languageCode)
            => _resultApi.Render(transDesc, matchCode, betType, languageCode);

    }
}
