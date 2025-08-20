namespace Fanex.Athena.BetList.Controllers
{
    using Common.Utility.Services.Interfaces;
    using DataAccess.Interfaces;
    using Fanex.BetList.Core.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Extensions;
    using Service.Helpers;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class SingleMatchParlayBetListController
    {
        private readonly ICacheRefAdapter _cacheRefAdapter;
        private readonly ISingleMatchParlayDetailDataAccess _dataAccess;
        private readonly IBetListHTMLBuilder _betlistHtmlBuilder;

        public SingleMatchParlayBetListController(
            ICacheRefAdapter cacheRefAdapter,
            ISingleMatchParlayDetailDataAccess dataAccess,
            IBetListHTMLBuilder betlistHtmlBuilder)
        {
            _cacheRefAdapter = cacheRefAdapter;
            _dataAccess = dataAccess;
            _betlistHtmlBuilder = betlistHtmlBuilder;
        }

        [HttpPost]
        public async Task<IEnumerable<string>> Detail(SingleMatchParlayDetailMetaModel metaModel)
        {
            var detail = await _dataAccess.GetSingleMatchParlayDetail(metaModel);

            if (!detail.Any())
            {
                return null;
            }

            var result = new List<string>();
            const string TextRenderTemplate = "<div class='single-match-parlay-selection'><span class='selection'>{0}</span>{1}</div>";
            const string StatusTextTemplate = " - <span class='status'>{0}</span>";

            var selectType = _cacheRefAdapter.GetBetBuilderSelectionNames(detail.Select(x => x.SelectionType).ToArray());

            for (var i = 0; i < selectType.Length; i++)
            {
                var selectionStatus = detail.ElementAtOrDefault(i)?.Result;
                var statusText = string.Empty;
                if (!BetListHelper.IsRunning(metaModel.Status) && !BetListHelper.IsVoid(metaModel.Status) && !string.IsNullOrEmpty(selectionStatus))
                {
                    statusText = string.Format(StatusTextTemplate, _betlistHtmlBuilder.BuildSingleMatchParlaySelectionStatus(selectionStatus));
                }

                result.Add(string.Format(TextRenderTemplate, selectType[i], statusText));
            }

            return result;
        }
    }
}
