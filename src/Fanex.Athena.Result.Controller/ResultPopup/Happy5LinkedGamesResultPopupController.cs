using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.LinkedGames;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
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
        public string Index(ThirdPartyMetaModel metaModel)
        {
            const string defaultPlatform = "D";
            var resultApi = _factory.CreateThirdPartyResultGetter(ConfigHelpers.GetConfigKey(ConfigName.LinkedGamesResult));
            var request = new ResultRequest(metaModel.CustId, metaModel.TransId3rd, metaModel.GameId3rd, metaModel.CurrencyId, metaModel.SiteId, metaModel.LanguageCode)
            {
                Platform = defaultPlatform,
                BetTypeId = metaModel.BetTypeId
            };

            return resultApi.GetResultGET(request);
        }

        [HttpPost]
        public string Soccer5(string matchId, string balls, string platform, string languageCode)
        {
            var isDesktop = string.Equals(platform, "d", StringComparison.OrdinalIgnoreCase);
            return _resultApi.GetSoccer5Result(matchId, balls, isDesktop, language: languageCode);
        }
    }
}
