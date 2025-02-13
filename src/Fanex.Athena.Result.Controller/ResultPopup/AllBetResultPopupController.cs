using Fanex.Result.Core.AllBetCasino;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class AllBetResultPopupController
    {
        private readonly IAllbetResultBuilder _resultApi;

        public AllBetResultPopupController(IAllbetResultBuilder resultApi)
            => _resultApi = resultApi;


        [HttpPost]
        public string Index(AllBetResultMetaModel metaModel)
            => _resultApi.Render(metaModel.TransDesc, metaModel.BetCheck, metaModel.BetType, metaModel.LanguageCode);

        public class AllBetResultMetaModel
        {
            public string TransDesc { get; set; }

            public string BetCheck { get; set; }

            public string BetType { get; set; }

            public string LanguageCode { get; set; }
        }
    }
}
