using Fanex.Athena.BetList.Controllers.CustomActionResult;
using Fanex.Athena.BetList.Controllers.Utility;
using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        private readonly IAppSettings _appSettings;
        private readonly IContextService _contextService;
        private readonly ISportService _sportService;
        private readonly IBettingTicketCenterAdvancedSearchDataAccess _dataAccess;
        private readonly IBettingTicketCenterZipBuilder _zipBuilder;
        private readonly IIPService _ipService;
        private readonly ICustomerClassService _customerClassService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BettingTicketCenterAdvancedSearchController(
            IAppSettings appSettings,
            IBettingTicketCenterAdvancedSearchDataAccess dataAccess,
            IIPService ipService,
            ITicketBuilderService ticketBuilderService,
            IBettingTicketCenterZipBuilder zipBuilder,
            IBettingTicketCenterExcel bettingTicketCenterExcel,
            IContextService contextService,
            ISportService sportService,
            ICustomerClassService customerClassService,
            IHttpContextAccessor httpContextAccessor) : base(ticketBuilderService, bettingTicketCenterExcel)
        {
            _appSettings = appSettings;
            _dataAccess = dataAccess;
            _ipService = ipService;
            _zipBuilder = zipBuilder ?? new BettingTicketCenterZipBuilder(_appSettings, null, bettingTicketCenterExcel, ipService);
            _contextService = contextService;
            _sportService = sportService;
            _customerClassService = customerClassService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [BearerAuthorize(Roles.BtcAdvancedSearch)]
        [RequestValidation(validateCurrencyList: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness:  true, bindShowingBetFromIP: true, bindProductCodeList: true)]
        public async Task<IActionResult> GetData(AdvancedSearchBetListMetaModel metaModel)
        {
            var isHideSaba = SabaHelper.IsHideSaba(_contextService.SpecialPlatform);
            var sabaSport = _sportService.GetSabaSport(metaModel.SportId, isHideSaba);
            if (sabaSport != null)
            {
                metaModel.LeagueGroupIds = string.Join(",", sabaSport.LeagueGroupIds);
                metaModel.SportId = sabaSport.SportTypeId;
            }

            if (metaModel.IsExport &&
                _appSettings.IsDisableExportLargeBetList
                && _appSettings.LicenseesWithLargeData.Contains(_contextService.SiteName))
            {
                return new JsonResult(new
                {
                    ErrorCode = ServerBusyErrorCode,
                    ErrorMsg = ViewResources.ServerBusyMessage
                });
            }

            metaModel.HasPlayerCom = _contextService.IsAllowedCustomerComm;
            metaModel.SpecialPlatform = _contextService.SpecialPlatform;
            metaModel.FullCustomerClass = string.Join(",", await _customerClassService.GetAllCustomerClassId());

            return await GetOrExportData(metaModel);
        }

        private async Task<IActionResult> GetOrExportData(AdvancedSearchBetListMetaModel metaModel)
        {
            if (metaModel.IsExport)
            {
                var totalRow = await _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel);
                if (totalRow > _appSettings.MaxRowSupportExportExcel)
                {
                    return new JsonResult(new
                    {
                        ErrorCode = RowsExceedLimitErrorCode
                    });
                }

                return await Export(metaModel, totalRow);
            }

            var ticketResult = ShouldReturnEmptyResult(metaModel)
                ? new TicketResult()
                : await _dataAccess.GetBetListForMultipleCustomersAsync(metaModel);

            return new JsonResult(ReturnData(ticketResult, GetBetListOption(metaModel), metaModel));
        }

        private async Task<IActionResult> Export(AdvancedSearchBetListMetaModel metaModel,
            int totalRow)
        {
            var title = GetTitle(metaModel);
            var totalText = $"{ViewResources.Totalin} {metaModel.BaseCurrencyName}";

            if (totalRow <= _appSettings.MaxRowPerExcelFile)
            {
                var normalTicketResult = await _dataAccess.GetBetListForMultipleCustomersAsync(metaModel);
                if (normalTicketResult.MainTicketList.Count == 0 || ShouldReturnEmptyResult(metaModel))
                {
                    return new NoContentResult();
                }

                return ExportXls(normalTicketResult, title, metaModel, totalText);
            }

            var ticketResult = await _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, _appSettings.MaxRowPerExcelFile);

            var zipFile = _zipBuilder.BuildZip(_httpContextAccessor.HttpContext.Response,
                ticketResult, title, metaModel, totalText);

            return zipFile == null ? null : ZipContentResult(zipFile);
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

        protected FileContentResult ZipContentResult(MemoryStream stream)
        {
            var bytes = stream?.ToArray();
            var fileResult = new FileContentResult(bytes, "application/zip");
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


    }
}
