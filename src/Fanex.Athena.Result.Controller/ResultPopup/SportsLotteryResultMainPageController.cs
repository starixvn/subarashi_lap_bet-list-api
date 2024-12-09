using Fanex.Result.Core.SportsLottery;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SportsLotteryResultMainPageController(ISportsLotteryResult resultApi)
    {
        [HttpPost]
        public string ResultPage(string platform, string languageCode)
        {
            var isDesktop = string.Equals(platform, "d", StringComparison.OrdinalIgnoreCase);
            return resultApi.GetResultPage(isDesktop, languageCode);
        }
    }
}
