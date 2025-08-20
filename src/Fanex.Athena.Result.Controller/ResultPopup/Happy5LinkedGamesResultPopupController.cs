namespace Fanex.Athena.Result.Controller.ResultPopup
{
    using Fanex.Result.Core.LinkedGames;
    using Fanex.Result.Core.ThirdParty.Models;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class Happy5LinkedGamesResultPopupController
    {
        private readonly ILinkedGamesResult _resultApi;
        private readonly IBetlistResultGetterFactory _factory;

        public Happy5LinkedGamesResultPopupController(ILinkedGamesResult resultApi, IBetlistResultGetterFactory factory)
        {
            _resultApi = resultApi;
            _factory = factory;
        }

        [HttpPost]
        public IActionResult Index(ThirdPartyMetaModel metaModel)
        {
            const string defaultPlatform = "D";
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.LinkedGamesResult));
            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                Platform = defaultPlatform,
                BetTypeId = metaModel.BetTypeId
            };

            return new ContentResult { Content = resultApi.GetResultGET(request) };
        }

        [HttpPost]
        public IActionResult Soccer5(Soccer5Request metaModel)
        {
            var isDesktop = string.Equals(metaModel.Platform, "d", StringComparison.OrdinalIgnoreCase);
            return new ContentResult{
                Content = _resultApi.GetSoccer5Result(metaModel.MatchId, metaModel.Balls, isDesktop, language: metaModel.LanguageCode) 
            }; 
        }

        public class Soccer5Request
        {
            public string MatchId { get; set; }

            public string Balls { get; set; }

            public string Platform { get; set; }

            public string LanguageCode { get; set; }
        }
    }
}
