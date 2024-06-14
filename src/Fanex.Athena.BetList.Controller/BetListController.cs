
namespace Fanex.Athena.BetList.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Utility.Helpers;
    using Common.Utility.Services.Interfaces;
    using Excel;
    using Fanex.Athena.BetList.Controllers.CustomActionResult;
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.BetList.Service.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Model.ViewModel;
    using Service.Helpers;

    [Route("[controller]/[action]")]
    [ApiController]
    public class BetListController 
    {
        protected const int PageSizeExcel = 63000;

        private const string DefaultPage = "Betlist Api default page";

        private readonly ICacheRefAdapter _cacheRefAdapter;

        private readonly IBetListService _betListService;

        private readonly ITicketBuilderService _ticketBuilderService;

        private readonly IIPService _ipService;

        public BetListController(ICacheRefAdapter cacheRefAdapter, IBetListService betListService, ITicketBuilderService ticketBuilderService, IIPService ipService)
        {
            _cacheRefAdapter = cacheRefAdapter;
            _betListService = betListService;
            _ticketBuilderService = ticketBuilderService;
            _ipService = ipService;
        }

        [HttpGet]
        public string Index()
        {
            return DefaultPage;
        }

        [HttpPost]
        public async Task<BetListResultViewModel> GetBetList(BetListMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetBetListData(metaModel);
            
            var leagueName = string.Empty;
            var firstOrDefault = ticketResult.MainTicketList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                leagueName = _betListService.GetLeagueName(firstOrDefault.LeagueId);
            }

            var main = _ticketBuilderService.BuildTicketText(
                ticketResult,
                metaModel,
                isAllowedCustomerComm: metaModel.HasPlayerCom,
                isMobileView: metaModel.IsMobileView);

            var result = new BetListResultViewModel
            {
                Main = main,
                Total = new BetListTotalData
                {
                    SumWinLoss = ticketResult.SumWinLoss + ticketResult.SumPlayerComm,
                    SumLicenseeWl = ticketResult.SumLicenseeWL + ticketResult.SumComm,
                    SumBetEffective = ticketResult.SumBetEffective,
                    SumStake = ticketResult.MainTicketList.Sum(x => Math.Round(x.Stake, 2))
                },
                LeagueName = leagueName,
            };

            BetListHelper.ClearData(ticketResult);
            return result;
        }

        [HttpPost]
        public async Task<object> GetBetListPaging(BetListMetaModel metaModel)
        {
            metaModel.IsServerPaging = true;

            var ticketResult = await _betListService.GetBetListData(metaModel);

            var totalPage = (int)Math.Ceiling((double)ticketResult.TotalRows / metaModel.PageSize);
            var pageIndex = metaModel.PageIndex > totalPage ? totalPage : metaModel.PageIndex;

            var startIndex = ((pageIndex - 1) * metaModel.PageSize) + 1;

            var sumLicenseeWl = ticketResult.SumLicenseeWL + ticketResult.SumComm;
            var sumWinLossText = NumberHelper.DecFormat(ticketResult.SumWinLoss + ticketResult.SumPlayerComm, 2);
            var sumLicenseeWlText = NumberHelper.DecFormat(sumLicenseeWl, 2);
            sumLicenseeWlText = sumLicenseeWl < 0
                ? string.Format("<span class='negative-number'>{0}</span>", sumLicenseeWlText)
                : sumLicenseeWlText;

            var result = new
            {
                page = pageIndex,
                total = totalPage,
                records = ticketResult.TotalRows,
                rows = _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, isAllowedCustomerComm: metaModel.HasPlayerCom),
                userdata = new
                {
                    WinLoss = sumWinLossText,
                    LicenseeWL = sumLicenseeWlText
                }
            };

            BetListHelper.ClearData(ticketResult);
            return result;
        }

        [HttpPost]
        public async Task<ExcelResult> ExportExcel(BetListExcelMetaModel metaModel)
        {
            return await ProcessExportExcel(metaModel);
        }

        [HttpPost]
        public async Task<ExcelResult> ExportExcelLiveReport(LiveReportBetListMetaModel metaModel)
        {
            return await ProcessExportExcel(metaModel);
        }

        protected internal virtual ExcelResult Export(BetListExcelMetaModel metaModel, TicketResult ticketResult)
        {
            var excel = new BetListExcel(metaModel.ShowingOption, metaModel.LicenseeId, metaModel.HiddenColumns, metaModel.IsShowSumStake, metaModel.IsShowIpvn, metaModel.IsShowIp, _ipService);

            var workbook = excel.Export(ticketResult, metaModel.Title, metaModel.StartIndex);

            return new ExcelResult(workbook);
        }

        protected internal virtual async Task<ExcelResult> ProcessExportExcel(BetListExcelMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetBetListData(metaModel);

            const string LeagueNamePlaceHolder = "(LEAGUENAME)";
            if (metaModel.Title.Contains(LeagueNamePlaceHolder))
            {
                var firstTicket = ticketResult.MainTicketList.FirstOrDefault();
                var leagueName = firstTicket != null ? _cacheRefAdapter.GetLeagueName(firstTicket.LeagueId) : string.Empty;
                metaModel.Title = metaModel.Title.Replace(LeagueNamePlaceHolder, leagueName);
            }

            return Export(metaModel, ticketResult);
        }
    }
}
