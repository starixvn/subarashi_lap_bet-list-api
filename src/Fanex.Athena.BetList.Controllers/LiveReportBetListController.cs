using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Utility.Helpers;
using Microsoft.AspNetCore.Mvc;


namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class LiveReportBetListController
    {
        private readonly IBetListService _betListService;
        private readonly ITicketBuilderService _ticketBuilderService;

        public LiveReportBetListController(IBetListService betListService, ITicketBuilderService ticketBuilderService)
        {
            _betListService = betListService;
            _ticketBuilderService = ticketBuilderService;
        }

        [HttpPost]
        public async Task<object> GetData(LiveReportBetListMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetBetListData(metaModel);

            var result = new
            {
                Main = _ticketBuilderService.BuildTicketText(ticketResult, isShowIpvn: metaModel.IsShowIpvn, isHideSaba: metaModel.IsHideSaba, isAllowedCustomerComm: metaModel.HasPlayerCom),
                Total = new
                {
                    SumWinLoss = ticketResult.SumWinLoss + ticketResult.SumPlayerComm,
                    SumLicenseeWl = ticketResult.SumLicenseeWL + ticketResult.SumComm
                }
            };
            BetListHelper.ClearData(ticketResult);
            return result;
        }

        [HttpPost]
        public async Task<object> GetPagingData(LiveReportBetListMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetBetListData(metaModel);

            var totalPage = ticketResult.TotalRows % metaModel.PageSize == 0
                ? ticketResult.TotalRows / metaModel.PageSize
                : (ticketResult.TotalRows / metaModel.PageSize) + 1;
            var startIndex = ((metaModel.PageIndex - 1) * metaModel.PageSize) + 1;

            var sumLicenseeWl = ticketResult.SumLicenseeWL + ticketResult.SumComm;
            var sumWinLossText = NumberHelper.DecFormat(ticketResult.SumWinLoss + ticketResult.SumPlayerComm, 2);
            var sumLicenseeWlText = NumberHelper.DecFormat(sumLicenseeWl, 2);
            sumLicenseeWlText = sumLicenseeWl < 0
                ? string.Format("<span class='negative-number'>{0}</span>", sumLicenseeWlText)
                : sumLicenseeWlText;

            var result = new
            {
                page = metaModel.PageIndex,
                total = totalPage,
                records = ticketResult.TotalRows,
                rows = _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, isAllowedCustomerComm: metaModel.HasPlayerCom),
                userdata = new
                {
                    WinLoss = sumWinLossText,
                    LicenseeWL = sumLicenseeWlText
                },
                IsHasCustComm = metaModel.HasPlayerCom
            };

            BetListHelper.ClearData(ticketResult);

            return result;
        }
    }
}
