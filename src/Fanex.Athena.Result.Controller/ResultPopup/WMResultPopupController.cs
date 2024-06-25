using Fanex.Result.Core.WM;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WMResultPopupController
    {
        private readonly IWMResultBuilder _resultApi;

        public WMResultPopupController(IWMResultBuilder resultApi)        
            => _resultApi = resultApi;
        

        [HttpPost]
        public string Index(string transDesc, string gameId3rd, int betType, string languageCode)
            => _resultApi.Render(transDesc, gameId3rd, betType, languageCode);
        
    }
}
