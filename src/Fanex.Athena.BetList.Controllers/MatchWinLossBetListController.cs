using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Report.Common.Helpers;
using Fanex.Athena.Resources;
using Fanex.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace Fanex.Athena.BetList.Controllers
{
    [BearerAuthorize(Roles.ReportMatchWinLoss)]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MatchWinLossBetListController : BetListBaseController
    {
        private readonly string STR_SP_BETLIST_MATCHWL_OLD = "Lrf_BetList_MatchWL_Old";
        private readonly string STR_SP_BETLIST_MATCHWL = "Lrf_BetList_MatchWL";

        private const string KickOfTimeFormat = "{0:MM/dd/yyyy hh:mm:ss tt}";

        protected const int PageIndex = 1;
        protected const int PageSizeExcel = 63000;

        protected static readonly int[] HiddenColumns = { BetEffectiveColumn };

        private static readonly CultureInfo EnglishCulture = new CultureInfo("en-US");

        private readonly ICurrencyValidation _currencyValidation;

        public MatchWinLossBetListController(IContextService contextService,
            IAuthorizationService authorizationService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess balanceUpDataAccess,
            ICurrencyValidation currencyValidation,
            ICacheRefAdapter cacheRefAdapter,
            IAppSettings appSettings,
            IIPService iPService)
            : base(contextService, authorizationService, betListService, ticketBuilderService, subsidiaryService,
                  balanceUpDataAccess, cacheRefAdapter, appSettings, iPService)
        {
            _currencyValidation = currencyValidation;
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> BetTypeWinLoss([FromQuery] BetListMetaModel metaModel, int page, int rows, bool dataOnly = true)
        {
            var balanceUp = GetBalanceUp();
            var UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            AssignPageInfoToMetaModel(metaModel, page, rows);

            if (dataOnly)
            {
                return await GetBetListPagingResultAsync(metaModel);
            }

            var settings = CreateDefaultSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {_cacheRefAdapter.GetBettype(metaModel.BetType)}";
            settings.IsServerPaging = true;

            return await GetPagingBetListResultWithSettings(metaModel, settings, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> MatchBetType(int page, int rows, [FromQuery] BetListMetaModel metaModel, bool dataOnly = true)
        {
            var balanceUp = GetBalanceUp();
            var UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            AssignPageInfoToMetaModel(metaModel, page, rows);

            if (dataOnly)
            {
                return await GetBetListPagingResultAsync(metaModel);
            }

            var settings = CreateDefaultSettings(metaModel);
            settings.Title = GetMatchBetTypeTitle(metaModel);
            settings.IsServerPaging = true;

            return await GetPagingBetListResultWithSettings(metaModel, settings, page, rows);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> Outright([FromQuery] BetListMetaModel metaModel, bool dataOnly = true)
        {
            metaModel.IsServerPaging = true;

            var balanceUp = GetBalanceUp();
            string UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);

            if (dataOnly)
            {
                return await GetBetListResultAsync(metaModel);
            }

            var settings = CreateDefaultSettings(metaModel);
            settings.Title = $"{ViewResources.BetList} - {ViewResources.Outright}";

            return await GetBetListResultWithSettings(metaModel, settings);
        }

        [HttpGet]
        public async Task<object> MixParlay([FromQuery] MixParlayMetaModel metaModel, bool dataOnly = true)
        {
            if (_currencyValidation.IsValidBaseCurrency(metaModel.BaseCurrency)
                && _currencyValidation.IsValidCurrency(metaModel.CurrencyId, _contextService.CurrencyList.ToDictionary(_ => _.Key, _ => _.Value)))
            {
                var balanceUp = GetBalanceUp();
                string UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate.Date, balanceUp.BalanceUpDay);
                var param = new BetListMetaModel
                {
                    LicenseeId = _contextService.LicenseeId,
                    UserId = _contextService.UserID,
                    CurrencyId = metaModel.CurrencyId,
                    BetType = metaModel.BetType,
                    BaseCurrency = metaModel.BaseCurrency ?? string.Empty,
                    FromDate = metaModel.FromDate.Date,
                    ToDate = metaModel.ToDate.Date,
                    SortBy = metaModel.SortBy,
                    SortOrder = metaModel.SortOrder,
                    ProductCodeList = metaModel.ProductCodeList,
                    SubsidiarySearch = metaModel.SubsidiarySearch,
                    SubsidiaryList = metaModel.SubsidiaryList,
                    AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(_ => _.Key, _ => _.Value), metaModel.CurrencyId),
                    HasPlayerCom = _contextService.IsAllowedCustomerComm,
                    IsShowIp = _contextService.IsShowIP,
                    IsShowIpvn = _contextService.IsShowIpvn
                };

                await CheckPermissionAndAssignCommonValueToMetaModel(param, UspKey);
                AssignPageInfoToMetaModel(param, metaModel.Page, metaModel.Rows);

                if (dataOnly)
                {
                    return await GetBetListPagingResultAsync(param);
                }

                var settings = CreateDefaultSettings(param);
                settings.Title = $"{ViewResources.BetList} - {_cacheRefAdapter.GetBettype(metaModel.BetType)}";
                settings.IsServerPaging = true;

                return await GetPagingBetListResultWithSettings(param, settings, metaModel.Page, metaModel.Rows);
            }
            return null;
        }

        [HttpPost]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true, bindAgent: true)]
        public async Task<IActionResult> ExportExcel(BetListExcelMetaModel metaModel)
        {
            var balanceUp = GetBalanceUp();
            string UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);

            metaModel.PageIndex = PageIndex;
            metaModel.PageSize = PageSizeExcel;
            metaModel.HiddenColumns = HiddenColumns;
            metaModel.IsServerPaging = true;

            metaModel.ShowingOption = new BetListShowingOption
            {
                CustomerCommVisible = _contextService.IsAllowedCustomerComm
            };

            var matchName = GetMatchName(metaModel);
            var currencyTitle = GetViewInCurrency(metaModel);

            metaModel.Title = ViewResources.BetList
                              + string.Format(
                                  " - {0}{1} - {2}",
                                  matchName + (metaModel.BetType > 0
                                      ? " - " + _cacheRefAdapter.GetBettype(metaModel.BetType) + " - "
                                      : string.Empty),
                                  metaModel.FromDate.ToShortDateString(),
                                  currencyTitle);

            return await ProcessExportExcel(metaModel);
        }

        private string GetMatchBetTypeTitle(BetListMetaModel metaModel)
        {
            var matchName = GetMatchName(metaModel);
            var betTypeName = metaModel.BetType > 0
                ? " - " + _cacheRefAdapter.GetBettype(metaModel.BetType)
                : string.Empty;

            if (string.IsNullOrEmpty(matchName) && string.IsNullOrEmpty(betTypeName))
            {
                return ViewResources.BetList;
            }

            return $"{ViewResources.BetList} - {matchName}{betTypeName}";
        }

        protected string GetMatchName(BetListMetaModel metaModel)
        {
            if (metaModel.ProductCodeList == ProductCodes.NG)
            {
                var prefix = metaModel.SportType == (int)SportType.NumberGameHappy5
                    ? ViewResources.No + "."
                    : ViewResources.NumberGameNo;
                return prefix + " " + metaModel.MatchCode;
            }

            if (SportHelper.IsVSSportTypes(metaModel.SportType))
            {
                return string.Format(EnglishCulture, KickOfTimeFormat, metaModel.KickOffTime);
            }

            if (SportHelper.IsVirtualBetRadarSports(metaModel.SportType))
            {
                var id = !string.IsNullOrEmpty(metaModel.Distance) ? $" - ID {metaModel.Distance}" : string.Empty;
                return $"{ViewResources.Event} {metaModel.VendorMatchId}{id}";
            }

            return metaModel.HomeId > 0 && metaModel.AwayId > 0
                ? $"{_cacheRefAdapter.GetTeamName(metaModel.HomeId, metaModel.SportType)} vs {_cacheRefAdapter.GetTeamName(metaModel.AwayId, metaModel.SportType)}"
                : string.Empty;
        }

        private string GetSpNameBasedOnBalanceUpDay(DateTime fromDate, DateTime balanceUpDay) => fromDate < balanceUpDay ? STR_SP_BETLIST_MATCHWL_OLD : STR_SP_BETLIST_MATCHWL;
    }
}
