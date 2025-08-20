namespace Fanex.Athena.BetList.Controllers
{
    using Common.Constant;
    using Common.DataAccess.Interfaces;
    using Common.Services.Interfaces;
    using Common.Site.Attributes;
    using Common.Utility.Helpers;
    using Common.Utility.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Model.DTO;
    using Model.MetaModel;
    using Model.ViewModel;
    using Resources;
    using Service.Helpers;
    using Service.Interfaces;
    using Service.Optiions;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [BearerAuthorize(Roles.ReportLiveReport, Roles.PortalSuperAdmin)]
    public class LiveReportBetListController(
        IContextService contextService,
        IAuthorizationService authorizationService,
        IBetListService betListService,
        ITicketBuilderService ticketBuilderService,
        ISubsidiaryService subsidiaryService,
        IBalanceUpDataAccess balanceUpDataAccess,
        ICacheRefAdapter cacheRefAdapter,
        IAppSettings appSettings,
        IIPService iPService)
        : BetListBaseController(contextService, authorizationService, betListService, ticketBuilderService,
            subsidiaryService, balanceUpDataAccess, cacheRefAdapter, appSettings, iPService)
    {
        protected const string ExcelFileName = "LiveReportBetList";

        private const string LiveReportSpKey = "Lrf_BetList_LiveReport";
        private const string MatchSpKey = "Lrf_BetList_LiveReport_SelWinlossByMatch";
        private const string OutRightSpKey = "Lrf_BetList_LiveReport_SelWinlossOutright";
        private const string NumberGameSpKey = "Lrf_BetList_LiveReport_SelWinlossNumberGame";
        private const string MixParlaySpKey = "Lrf_BetList_LiveReport_SelWinlossParlay";
        private const string VirtualSportSpKey = "Lrf_BetList_LiveReport_SelWinlossVHRacing";

        private static readonly int[] HiddenColumns = { BetEffectiveColumn };

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> Match([FromQuery] LiveReportBetListMetaModel metaModel, bool dataOnly = true)
        {
            metaModel.UspKey = MatchSpKey;
            if (dataOnly)
            {
                return await GetData(metaModel);
            }
            var settings = CreateDefaultDayRangeSettings(metaModel);
            settings.Title = GetMatchTitle(metaModel);

            return await GetDataWithSettings(metaModel, settings);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> MatchExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = MatchSpKey;
            return await ExportExcel(metaModel);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> Outright([FromQuery] LiveReportBetListMetaModel metaModel, bool dataOnly)
        {
            metaModel.UspKey = OutRightSpKey;
            if (dataOnly)
            {
                return await GetData(metaModel);
            }

            var settings = CreateDefaultDayRangeSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {ViewResources.Outright}";

            return await GetDataWithSettings(metaModel, settings);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> OutrightExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = OutRightSpKey;

            return await ExportExcel(metaModel, ViewResources.Outright);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> NumberGame([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows, bool dataOnly = true)
        {
            metaModel.IsServerPaging = true;
            metaModel.UspKey = NumberGameSpKey;
            if (dataOnly)
            {
                return await GetPagingData(metaModel, page, rows);
            }

            var settings = CreateDefaultDayRangeSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {ViewResources.ProductNG}";
            settings.IsServerPaging = true;

            return await GetPagingDataWithSettings(metaModel, settings, page, rows);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> NumberGameExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = NumberGameSpKey;
            metaModel.IsServerPaging = true;

            return await ExportExcel(metaModel, ViewResources.ProductNG);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> MixParlay([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows, bool dataOnly)
        {
            metaModel.UspKey = MixParlaySpKey;

            metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(_ => _.Key, _ => _.Value), metaModel.CurrencyId);

            if (dataOnly)
            {
                return await GetPagingData(metaModel, page, rows);
            }

            var settings = CreateDefaultDayRangeSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {_cacheRefAdapter.GetBettype(metaModel.BetType)}";
            settings.IsServerPaging = true;

            return await GetPagingDataWithSettings(metaModel, settings, page, rows);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> MixParlayExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = MixParlaySpKey;
            metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(_ => _.Key, _ => _.Value), metaModel.CurrencyId);

            return await ExportExcel(metaModel);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> VirtualSports([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows, bool dataOnly = true)
        {
            metaModel.UspKey = VirtualSportSpKey;

            if (dataOnly)
            {
                return await GetPagingData(metaModel, page, rows);
            }

            var settings = CreateDefaultSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {_cacheRefAdapter.GetSportName(metaModel.SportType)}";
            settings.IsServerPaging = true;

            return await GetPagingDataWithSettings(metaModel, settings, page, rows);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> VirtualSportsExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = VirtualSportSpKey;

            return await ExportExcel(metaModel, _cacheRefAdapter.GetSportName(metaModel.SportType));
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> SportsLottery([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows, bool dataOnly = true)
        {
            metaModel.UspKey = LiveReportSpKey;
            metaModel.ProductCodeList += $",{ProductCodes.SLT}";

            if (dataOnly)
            {
                return await GetPagingData(metaModel, page, rows);
            }

            var settings = CreateDefaultSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {_cacheRefAdapter.GetSportName(metaModel.SportType)}";
            settings.IsServerPaging = true;

            return await GetPagingDataWithSettings(metaModel, settings, page, rows);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> SportsLotteryExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = LiveReportSpKey;
            metaModel.ProductCodeList += $",{ProductCodes.SLT}";

            return await ExportExcel(metaModel, _cacheRefAdapter.GetSportName(metaModel.SportType));
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> BitCoin([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows, bool dataOnly = true)
        {
            metaModel.UspKey = LiveReportSpKey;
            metaModel.ProductCodeList += $",{ProductCodes.BTC}";

            if (dataOnly)
            {
                return await GetPagingData(metaModel, page, rows);
            }

            var settings = CreateDefaultDayRangeSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {metaModel.ProductName}";
            settings.IsServerPaging = true;

            return await GetPagingDataWithSettings(metaModel, settings, page, rows);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> BitCoinExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = LiveReportSpKey;
            metaModel.ProductCodeList += $",{ProductCodes.BTC}";

            return await ExportExcel(metaModel, metaModel.ProductName);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> VS3([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows, bool dataOnly = true)
        {
            metaModel.UspKey = LiveReportSpKey;

            if (dataOnly)
            {
                return await GetPagingData(metaModel, page, rows);
            }

            var settings = CreateDefaultDayRangeSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {_cacheRefAdapter.GetSportName(metaModel.SportType)}";
            settings.IsServerPaging = true;

            return await GetPagingDataWithSettings(metaModel, settings, page, rows);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> VS3ExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
        {
            metaModel.UspKey = LiveReportSpKey;

            return await ExportExcel(metaModel);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> VSNewRacing([FromQuery] LiveReportBetListMetaModel metaModel, int page, int rows, bool dataOnly)
        {
            metaModel.UspKey = LiveReportSpKey;
            if (dataOnly)
            {
                return await GetPagingData(metaModel, page, rows);
            }

            var settings = CreateDefaultDayRangeSettings(metaModel);

            var id = !string.IsNullOrEmpty(metaModel.Distance) ? $" - ID {metaModel.Distance}" : string.Empty;
            var matchName = $"{ViewResources.Event} {metaModel.VendorMatchId}{id}";

            settings.Title = $"{ViewResources.BetList} - {matchName}";
            settings.IsServerPaging = true;

            return await GetPagingDataWithSettings(metaModel, settings, page, rows);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindPlayerCom: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> VSNewRacingExportExcel([FromBody] LiveReportBetListMetaModel metaModel)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            metaModel.UspKey = LiveReportSpKey;

            var id = !string.IsNullOrEmpty(metaModel.Distance) ? $" - ID {metaModel.Distance}" : string.Empty;
            var matchName = $"{ViewResources.Event} {metaModel.VendorMatchId}{id}";

            return await ExportExcel(metaModel, matchName);
        }

        private async Task<BetListResultViewModel> GetData(LiveReportBetListMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetBetListData(metaModel);

            var result = new BetListResultViewModel
            {
                Main = _ticketBuilderService.BuildTicketText(ticketResult,
                    buildTicketOption: new BuildTicketOption
                    {
                        IsShowIpvn = metaModel.IsShowIpvn,
                        IsHideSaba = metaModel.IsHideSaba,
                        IsAllowedCustomerComm = metaModel.HasPlayerCom
                    }),
                Total = new BetListTotalData
                {
                    SumWinLoss = ticketResult.SumWinLoss + ticketResult.SumPlayerComm,
                    SumLicenseeWl = ticketResult.SumLicenseeWL + ticketResult.SumComm
                }
            };
            BetListHelper.ClearData(ticketResult);
            return result;
        }

        private async Task<object> GetPagingData(LiveReportBetListMetaModel metaModel, int pageIndex, int pageSize)
        {
            const int DefautlPageSize = 100;
            metaModel.PageIndex = pageIndex < 1 ? 1 : pageIndex;
            metaModel.PageSize = pageSize < 1 ? DefautlPageSize : pageSize;
            metaModel.LicenseeId = _contextService.LicenseeId;

            var ticketResult = await _betListService.GetBetListData(metaModel);

            var totalPage = ticketResult.TotalRows % metaModel.PageSize == 0
                ? ticketResult.TotalRows / metaModel.PageSize
                : (ticketResult.TotalRows / metaModel.PageSize) + 1;
            var startIndex = ((metaModel.PageIndex - 1) * metaModel.PageSize) + 1;

            var sumLicenseeWl = ticketResult.SumLicenseeWL + ticketResult.SumComm;
            var sumWinLossText = NumberHelper.DecFormat(ticketResult.SumWinLoss + ticketResult.SumPlayerComm);
            var sumLicenseeWlText = NumberHelper.DecFormat(sumLicenseeWl);
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

        private async Task<BetListResultWithSettingViewModel<object>> GetPagingDataWithSettings(
            LiveReportBetListMetaModel metaModel,
            BetListSettingViewModel settings,
            int pageIndex,
            int pageSize)
        {
            AssignDefaultPagingBetListMetaModel(metaModel, pageIndex, pageSize);
            var data = await GetBetListPagingResultAsync(metaModel);

            settings.IsAllowedCustomerCommission = metaModel.HasPlayerCom;
            return new BetListResultWithSettingViewModel<object>
            {
                Data = data,
                Settings = settings
            };
        }

        private string GetMatchTitle(BetListMetaModel metaModel)
        {
            var matchName = metaModel.HomeId > 0 && metaModel.AwayId > 0 ?
                _cacheRefAdapter.GetTeamName(metaModel.HomeId, metaModel.SportType) + " vs " +
                _cacheRefAdapter.GetTeamName(metaModel.AwayId, metaModel.SportType) :
                string.Empty;

            var betTypeName = _cacheRefAdapter.GetBettype(metaModel.BetType);

            if (!string.IsNullOrEmpty(betTypeName))
            {
                betTypeName = $" - {betTypeName}";
            }

            if (string.IsNullOrEmpty(matchName) && string.IsNullOrEmpty(betTypeName))
            {
                return ViewResources.BetList;
            }

            return $"{ViewResources.BetList} - {matchName}{betTypeName}";
        }

        private async Task<BetListResultWithSettingViewModel<BetListResultViewModel>>
            GetDataWithSettings(LiveReportBetListMetaModel metaModel,
            BetListSettingViewModel settings)
        {
            AssignDefaultBetListMetaModel(metaModel);

            var data = await GetData(metaModel);

            return new BetListResultWithSettingViewModel<BetListResultViewModel>
            {
                Data = data,
                Settings = settings
            };
        }

        private async Task<IActionResult> ExportExcel(LiveReportBetListMetaModel metaModel, string caption = null)
        {
            AssignDefaultBetListMetaModel(metaModel);

            metaModel.StartIndex = (metaModel.PageIndex - 1) * metaModel.PageSize + 1;
            metaModel.Title = GetExcelTitle(metaModel, caption);
            metaModel.HiddenColumns = HiddenColumns;
            metaModel.ShowingOption = new BetListShowingOption
            {
                CustomerCommVisible = _contextService.IsAllowedCustomerComm
            };

            return await ProcessExportExcel(metaModel);
        }

        private string GetExcelTitle(LiveReportBetListMetaModel metaModel, string caption = null)
        {
            var betListCaption = string.IsNullOrEmpty(caption) ?
                GetMatchName(metaModel) + _cacheRefAdapter.GetBettype(metaModel.BetType) :
                caption;
            var viewInCaption = GetViewInCurrency(metaModel);
            return
                $"{ViewResources.BetList} - {betListCaption} - {metaModel.FromDate.ToShortDateString()} --> {metaModel.ToDate.ToShortDateString()} - {viewInCaption}";
        }

        private string GetMatchName(LiveReportBetListMetaModel model) =>
            model.HomeId > 0 && model.AwayId > 0 ?
                _cacheRefAdapter.GetTeamName(model.HomeId, model.SportType) + " vs " + _cacheRefAdapter.GetTeamName(model.AwayId, model.SportType) + " " :
                string.Empty;
    }
}