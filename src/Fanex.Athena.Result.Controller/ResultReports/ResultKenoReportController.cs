using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Fanex.Athena.Result.Controller.ResultReports
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ResultKenoReportController
    {
        private readonly IKenoLotteryTGResultFactory _resultApi;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ResultKenoReportController(IKenoLotteryTGResultFactory resultApi,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _resultApi = resultApi;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        [HttpPost]
        public string GetResultPage([FromBody] ResultKenoMetaModel metaModel)
        {
            var result = _resultApi.CreateResultApi(metaModel.VendorProductId);
            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return result.GetResult(metaModel.CustId, metaModel.IsDesktop, DateTimeOffset.Now.DateTime, urlHelper.Action("AccessDenied", "Utility", null, request.Scheme), metaModel.LanguageCode);
        }
    }
}
