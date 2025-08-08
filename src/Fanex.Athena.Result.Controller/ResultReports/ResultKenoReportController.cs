namespace Fanex.Athena.Result.Controller.ResultReports
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Model.MetaModel;
    using Service.Interfaces;

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
        public IActionResult GetResultPage([FromBody] ResultKenoMetaModel metaModel)
        {
            var result = _resultApi.CreateResultApi(metaModel.VendorProductId);
            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return new ContentResult
            {
                Content = result.GetResult(metaModel.CustId, 
                metaModel.IsDesktop, 
                DateTimeOffset.Now.DateTime, 
                urlHelper.Action("AccessDenied", "Utility", null, request.Scheme), metaModel.LanguageCode)
            };
        }
    }
}
