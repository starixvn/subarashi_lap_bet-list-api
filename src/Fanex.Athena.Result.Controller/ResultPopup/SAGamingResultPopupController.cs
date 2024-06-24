using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.Result.Controller.ResultPopup
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SAGamingResultPopupController
    {
        private readonly ISAGamingResultBuilder _resultApi;

        public SAGamingResultPopupController(ISAGamingResultBuilder resultApi)
        {
            _resultApi = resultApi;
        }

        [HttpPost]
        public string Index(string transDesc, string matchCode, string betType, string languageCode)
        {
            return _resultApi.Render(transDesc, matchCode, betType, languageCode);
        }
    }
}
