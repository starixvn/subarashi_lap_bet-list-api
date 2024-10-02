using Fanex.Result.Core.SportsLottery;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SportsLotteryResultMainPageController
    {
        private readonly ISportsLotteryResult _resultApi;

        public SportsLotteryResultMainPageController(ISportsLotteryResult resultApi)

            => _resultApi = resultApi;


        [HttpPost]
        public string ResultPage(string platform, string languageCode)
        {
            var isDesktop = string.Equals(platform, "d", StringComparison.OrdinalIgnoreCase);
            return _resultApi.GetResultPage(isDesktop, languageCode);
        }
    }
}
