using Fanex.Result.Core.SexyGaming;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SexyGamingResultPopupController
    {
        private readonly ISexyGamingResultBuilder _resultApi;

        public SexyGamingResultPopupController(ISexyGamingResultBuilder resultApi)        
            => _resultApi = resultApi;
        

        [HttpPost]
        public string Index(string transDesc, string matchCode, int betType, string languageCode)       
            => _resultApi.Render(transDesc, matchCode, betType, languageCode);
        
    }
}
