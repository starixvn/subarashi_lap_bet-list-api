using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Result.Core.SportsLottery;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class SportsLotteryResultMainPageController(ISportsLotteryResult resultApi)
    {
        [HttpPost]
        public IActionResult ResultPage(SportsLotteryResultMetaModel model)
        {
            var isDesktop = string.Equals(model.Platform, "d", StringComparison.OrdinalIgnoreCase);
            return new ContentResult
            {
                Content = resultApi.GetResultPage(isDesktop, model.LanguageCode)
            };
        }
    }
}
