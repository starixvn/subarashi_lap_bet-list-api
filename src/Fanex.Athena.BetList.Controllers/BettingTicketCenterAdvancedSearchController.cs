using Fanex.Athena.BetList.Controllers.CustomActionResult;
using Fanex.Athena.BetList.Controllers.Utility;
using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [BearerAuthorize(Roles.BtcAdvancedSearch)]
    public class BettingTicketCenterAdvancedSearchController : BettingTicketCenterBaseController
    {
        private const int RowsExceedLimitErrorCode = 1001;
        private const int ServerBusyErrorCode = 1002;

        private const int ExportMultipleFile = 1002;
        private const int ExportSingleFile = 1003;

        private readonly IAppSettings _appSettings;
        private readonly IContextService _contextService;
        private readonly IBettingTicketCenterAdvancedSearchDataAccess _dataAccess;
        private readonly IBettingTicketCenterZipBuilder _zipBuilder;
        private readonly IIPService _ipService;
        private readonly ICustomerClassService _customerClassService;
        private readonly ITempFileService _tempFileService;
        private readonly IBettingTicketCenterSportService _bettingTicketCenterSportService;

        public BettingTicketCenterAdvancedSearchController(
            IAppSettings appSettings,
            IBettingTicketCenterAdvancedSearchDataAccess dataAccess,
            IIPService ipService,
            ITicketBuilderService ticketBuilderService,
            IBettingTicketCenterZipBuilder zipBuilder,
            IBettingTicketCenterExcel bettingTicketCenterExcel,
            IContextService contextService,
            ICustomerClassService customerClassService,
            ITempFileService tempFileService,
            IBettingTicketCenterSportService bettingTicketCenterSportService)
            : base(ticketBuilderService, bettingTicketCenterExcel)
        {
            _appSettings = appSettings;
            _dataAccess = dataAccess;
            _ipService = ipService;
            _zipBuilder = zipBuilder ?? new BettingTicketCenterZipBuilder(_appSettings, null, bettingTicketCenterExcel, ipService);
            _contextService = contextService;
            _customerClassService = customerClassService;
            _tempFileService = tempFileService;
            _bettingTicketCenterSportService = bettingTicketCenterSportService;
        }

        [HttpPost]
        [BearerAuthorize(Roles.BtcAdvancedSearch)]
        [RequestValidation(validateCurrencyList: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindShowingBetFromIP: true, bindProductCodeList: true)]
        public async Task<IActionResult> GetData(AdvancedSearchBetListMetaModel metaModel)
        {
            if (metaModel.IsExport && IsAllowExportLargeBetList())
            {
                return ServerBusy();
            }

            await UpdateMetaModel(metaModel);

            return await GetOrExportData(metaModel);
        }

        [HttpPost]
        [BearerAuthorize(Roles.BtcAdvancedSearch)]
        [RequestValidation(validateCurrencyList: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindShowingBetFromIP: true, bindProductCodeList: true)]
        public async Task<IActionResult> GetExportStatus(AdvancedSearchBetListMetaModel metaModel)
        {
            if (IsAllowExportLargeBetList())
            {
                return ServerBusy();
            }
            await UpdateMetaModel(metaModel);
            var totalRow = await _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, _appSettings.MaxTicketLimit);
            if (totalRow > _appSettings.MaxTicketLimit)
            {
                return new JsonResult(new
                {
                    StatusCode = RowsExceedLimitErrorCode,
                    LimitRows = _appSettings.MaxTicketLimit
                });
            }

            if (totalRow <= _appSettings.MaxRowSupportSingleExportExcel)
            {
                return new JsonResult(new
                {
                    StatusCode = ExportSingleFile,
                    PageSize = _appSettings.MaxRowSupportSingleExportExcel
                });
            }

            return new JsonResult(new
            {
                StatusCode = ExportMultipleFile,
                PageSize = _appSettings.MaxRowForMultipleExcelFiles
            });

        }

        private IActionResult ServerBusy() => new JsonResult(new
        {
            ErrorCode = ServerBusyErrorCode,
            ErrorMsg = ViewResources.ServerBusyMessage
        });

        private async Task UpdateMetaModel(AdvancedSearchBetListMetaModel metaModel)
        {
            metaModel.SportListXML = _bettingTicketCenterSportService.BuildSportTypeXML(metaModel.SportIds);
            metaModel.HasPlayerCom = _contextService.IsAllowedCustomerComm;
            metaModel.SpecialPlatform = _contextService.SpecialPlatform;
            metaModel.FullCustomerClass = string.Join(",", await _customerClassService.GetAllCustomerClassId());
            metaModel.LicenseeName = _contextService.SiteName;
        }

        private async Task<IActionResult> GetOrExportData(AdvancedSearchBetListMetaModel metaModel)
        {
            if (metaModel.IsExport)
            {
                return await Export(metaModel);
            }

            var totalRow = await _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, _appSettings.MaxTicketLimit);

            if (totalRow > _appSettings.MaxTicketLimit)
            {
                return new JsonResult(new
                {
                    StatusCode = RowsExceedLimitErrorCode,
                    LimitRows = _appSettings.MaxTicketLimit
                });
            }

            var ticketResult = ShouldReturnEmptyResult(metaModel)
                ? new TicketResult()
                : await _dataAccess.GetBetListForMultipleCustomersAsync(metaModel);

            return new JsonResult(ReturnData(ticketResult, GetBetListOption(metaModel), metaModel));
        }

        private async Task<IActionResult> Export(AdvancedSearchBetListMetaModel metaModel)
        {
            var title = GetTitle(metaModel);
            var totalText = $"{ViewResources.Totalin} {metaModel.BaseCurrencyName}";

            var ticketResult = await _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, _appSettings.MaxRowPerExcelFile);

            if (!ticketResult.DataPath.Any())
            {
                return NoContent();
            }

            if (ticketResult.TotalRows <= _appSettings.MaxRowPerExcelFile)
            {
                if (ticketResult.TotalRows == 0 || ShouldReturnEmptyResult(metaModel))
                {
                    return NoContent();
                }

                var tickets = GetTicketFromFile(ticketResult);
                return ExportXls(tickets, title, metaModel, totalText);
            }

            if (ShouldExportNextPage(metaModel.PageSize, ticketResult.CurrentPageRowCount))
            {
                Response.StatusCode = (int)HttpStatusCode.PartialContent;
            }

            var zipFile = _zipBuilder.BuildZip(ticketResult, title, metaModel, totalText);

            return zipFile == null ? null : ZipContentResult(zipFile, metaModel);
        }

        private ExcelResult ExportXls(TicketResult ticketResult, string title, AdvancedSearchBetListMetaModel metaModel, string totalText)
        {
            var excelModel = new BettingTicketCenterExcelModel
            {
                Data = ticketResult,
                Option = GetBetListOption(metaModel),
                Title = title,
                TotalText = totalText
            };

            return ExportExcel(excelModel);
        }

        protected FileContentResult ZipContentResult(MemoryStream stream, AdvancedSearchBetListMetaModel metaModel)
        {
            var bytes = stream?.ToArray();
            var fileResult = new FileContentResult(bytes, "application/zip");
            Response.Headers.Append("X-File-Name", $"BettingTicketsCenter_AdvancedSearch{metaModel.PageIndex}");
            fileResult.FileDownloadName = "BettingTicketsCenter_AdvancedSearch.zip";
            return fileResult;
        }

        private static BettingTicketCenterOption GetBetListOption(AdvancedSearchBetListMetaModel metaModel) => new BettingTicketCenterOption
        {
            ShowTotal = true,
            ShowSumStake = true,
            ShowLicenseeWl = true,
            ShowWinLoss = true,
            ShowExternalId = true,
            ShowSystemId = true,
            IsAllowedCustomerComm = metaModel.HasPlayerCom,
            ShowTxId = metaModel.IsAllowTxIDColumn,
            IsShowIpvn = metaModel.IsShowIpvn,
            IsShowIp = metaModel.IsShowIp,
            SpecialPlatform = metaModel.SpecialPlatform,
        };

        private bool ShouldReturnEmptyResult(AdvancedSearchBetListMetaModel metaModel)
            => !string.IsNullOrWhiteSpace(metaModel.Ip) && !metaModel.IsShowIpvn && _ipService.IsVNIP(metaModel.Ip);

        private bool ShouldExportNextPage(int pageSize, int currentPageRowCount)
            => pageSize == currentPageRowCount;

        private TicketResult GetTicketFromFile(GroupTicketResult groupTicket)
        {
            var contentData = _tempFileService.ReadString(groupTicket.DataPath[0]);
            _tempFileService.Delete(groupTicket.DataPath[0]);
            var ticketResult = JsonConvert.DeserializeObject<TicketResult>(contentData);

            ticketResult.SumStake = groupTicket.SumStake;
            ticketResult.SumWinLoss = groupTicket.SumWinLoss;
            ticketResult.SumLicenseeWL = groupTicket.SumLicenseeWL;
            ticketResult.SumComm = groupTicket.SumComm;
            ticketResult.SumPlayerComm = groupTicket.SumPlayerComm;
            ticketResult.TotalRows = groupTicket.TotalRows;
            return ticketResult;
        }

        private bool IsAllowExportLargeBetList()
            => _appSettings.IsDisableExportLargeBetList && _appSettings.LicenseesWithLargeData.Contains(_contextService.SiteName);
    }
}
