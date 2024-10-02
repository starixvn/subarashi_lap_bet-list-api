using Fanex.Result.Core.AllBetCasino;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AllBetResultPopupController
    {
        private readonly IAllbetResultBuilder _resultApi;

        public AllBetResultPopupController(IAllbetResultBuilder resultApi)
            => _resultApi = resultApi;


        [HttpPost]
        public string Index(string transDesc, string betCheck, string betType, string languageCode)
            => _resultApi.Render(transDesc, betCheck, betType, languageCode);

    }
}
