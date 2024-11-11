using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.BetList.Service.Optiions;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [BearerAuthorize(Roles.ReportLiveReport)]
    public class LiveReportBetListController
    {
        private readonly IBetListService _betListService;
        private readonly ITicketBuilderService _ticketBuilderService;
        private readonly IContextService _contextService;

        public LiveReportBetListController(IBetListService betListService, ITicketBuilderService ticketBuilderService, IContextService contextService)
        {
            _betListService = betListService;
            _ticketBuilderService = ticketBuilderService;
            _contextService = contextService;
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> Match(LiveReportBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossByMatch";
            return GetData(metaModel, UspKey);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> Outright(LiveReportBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossOutright";
            return GetData(metaModel, UspKey);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> NumberGame([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows)
        {
            metaModel.IsServerPaging = true;
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossNumberGame";
            return GetPagingData(metaModel, UspKey, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> MixParlay([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows)
        {
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossParlay";

            metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(_ => _.Key, _ => _.Value), metaModel.CurrencyId);

            return GetPagingData(metaModel, UspKey, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> VirtualSports([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows)
        {
            const string UspName = "Lrf_BetList_LiveReport_SelWinlossVHRacing";

            return GetPagingData(metaModel, UspName, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> SportsLottery([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows)
        {
            const string UspName = "Lrf_BetList_LiveReport";
            metaModel.ProductCodeList += $",{ProductCodes.SLT}";

            return GetPagingData(metaModel, UspName, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> BitCoin([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows)
        {
            const string UspName = "Lrf_BetList_LiveReport";
            metaModel.ProductCodeList += $",{ProductCodes.BTC}";

            return GetPagingData(metaModel, UspName, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> VS3([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows)
        {
            const string UspName = "Lrf_BetList_LiveReport";

            return GetPagingData(metaModel, UspName, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<dynamic> Betradar([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows)
        {
            const string UspName = "Lrf_BetList_LiveReport";

            return await GetPagingData(metaModel, UspName, page, rows);
        }

        public async Task<object> GetData(LiveReportBetListMetaModel metaModel, string uspKey)
        {
            metaModel.UspKey = uspKey;
            var ticketResult = await _betListService.GetBetListData(metaModel);

            var result = new
            {
                Main = _ticketBuilderService.BuildTicketText(ticketResult,
                    buildTicketOption: new BuildTicketOption
                    {
                        IsShowIpvn = metaModel.IsShowIpvn,
                        IsHideSaba = metaModel.IsHideSaba,
                        IsAllowedCustomerComm = metaModel.HasPlayerCom
                    }),
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
        public async Task<object> GetPagingData(LiveReportBetListMetaModel metaModel, string uspKey, int pageIndex, int pageSize)
        {
            const int DefautlPageSize = 100;
            metaModel.PageIndex = pageIndex < 1 ? 1 : pageIndex;
            metaModel.PageSize = pageSize < 1 ? DefautlPageSize : pageSize;
            metaModel.LicenseeId = _contextService.LicenseeId;
            metaModel.UspKey = uspKey;

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
                rows = _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom }),
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
