using Fanex.Result.Core.SabaClub;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SabaClubResultPopupController
    {
        private readonly ISabaClubResult _resultApi;

        public SabaClubResultPopupController(ISabaClubResult resultApi)        
            => _resultApi = resultApi;
        

        [HttpPost]
        public string Index(string custId, string gameRoundId, string gameId, string siteId, int sportType, string languageCode)        
            => _resultApi.GetResult(custId, gameRoundId, gameId, siteId, sportType, languageCode);
        
    }
}
