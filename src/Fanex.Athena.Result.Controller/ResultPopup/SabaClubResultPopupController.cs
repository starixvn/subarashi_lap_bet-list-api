using Fanex.Result.Core.SabaClub;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class SabaClubResultPopupController
    {
        private readonly ISabaClubResult _resultApi;

        public SabaClubResultPopupController(ISabaClubResult resultApi)
            => _resultApi = resultApi;


        [HttpPost]
        public IActionResult Index(SabaClubResultMetaModel model)
            => new ContentResult
            {
                Content = _resultApi.GetResult(model.CustId, model.GameRoundId, model.GameId, model.SiteId, model.SportType, model.LanguageCode)
            };

        public class SabaClubResultMetaModel
        {
            public string CustId { get; set; }

            public string GameRoundId { get; set; }

            public string GameId { get; set; }

            public string SiteId { get; set; }

            public int SportType { get; set; }

            public string LanguageCode { get; set; }
        }
    }
}
