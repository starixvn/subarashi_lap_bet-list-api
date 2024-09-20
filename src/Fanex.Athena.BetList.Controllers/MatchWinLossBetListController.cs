

using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [BearerAuthorize(Roles.ReportMatchWinLoss)]
    public class MatchWinLossBetListController : BetListBaseController
    {
        private readonly string STR_SP_BETLIST_MATCHWL_OLD = "Lrf_BetList_MatchWL_Old";
        private readonly string STR_SP_BETLIST_MATCHWL = "Lrf_BetList_MatchWL";

        private readonly ICurrencyValidation _currencyValidation;

        public MatchWinLossBetListController(IContextService contextService, 
            IAuthorizationService authoriztionService, 
            IBetListService betListService, 
            ITicketBuilderService ticketBuilderService, 
            ISubsidiaryService subsidiaryService, 
            IBalanceUpDataAccess balanceUpDataAccess,
            ICurrencyValidation currencyValidation) 
            : base(contextService, authoriztionService, betListService, ticketBuilderService, subsidiaryService, balanceUpDataAccess)
        {
            _currencyValidation = currencyValidation;
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<BetListResultViewModel> BetTypeWinLoss(int page, int rows, [FromQuery] BetListMetaModel metaModel)
        {
            var balanceUp = GetBalanceUp();
            string UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            AssignPageInfoToMetaModel(metaModel, page, rows);

            return await GetBetListPagingResultAsync(metaModel);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<dynamic> MatchBetType(int page, int rows, [FromQuery] BetListMetaModel metaModel)
        {
            var balanceUp = GetBalanceUp();
            string UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay); ;

            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            AssignPageInfoToMetaModel(metaModel, page, rows);

            return await GetBetListPagingResultAsync(metaModel);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<dynamic> Outright(BetListMetaModel metaModel)
        {
            metaModel.IsServerPaging = true;

            var balanceUp = GetBalanceUp();
            string UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            return await GetBelistResultAsync(metaModel);
        }

        [HttpGet]
        public async Task<dynamic> MixParlay(
         int page,
         int rows,
         int currencyId,
         string baseCurrency,
         int betType,
         DateTime fromDate,
         DateTime toDate,
         string productCodeList,
         bool? subsidiarySearch = false,
         string subsidiaryList = null,
         string sortBy = null,
         string sortOrder = null)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            if (_currencyValidation.IsValidBaseCurrency(baseCurrency)
                && _currencyValidation.IsValidCurrency(currencyId, _contextService.CurrencyList.ToDictionary(_ => _.Key, _ => _.Value)))
            {
                var balanceUp = GetBalanceUp();
                string UspKey = GetSpNameBasedOnBalanceUpDay(fromDate.Date, balanceUp.BalanceUpDay);
                var metaModel = new BetListMetaModel
                {
                    LicenseeId = _contextService.LicenseeId,
                    UserId = _contextService.UserID,
                    CurrencyId = currencyId,
                    BetType = betType,
                    BaseCurrency = baseCurrency ?? string.Empty,
                    FromDate = fromDate.Date,
                    ToDate = toDate.Date,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    ProductCodeList = productCodeList,
                    SubsidiarySearch = subsidiarySearch.GetValueOrDefault(),
                    SubsidiaryList = subsidiaryList,
                    AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(_ => _.Key, _ => _.Value), currencyId),
                    HasPlayerCom = _contextService.IsAllowedCustomerComm,
                    IsShowIp = _contextService.IsShowIP,
                    IsShowIpvn = _contextService.IsShowIpvn
                };

                await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
                AssignPageInfoToMetaModel(metaModel, page, rows);

                return await GetBetListPagingResultAsync(metaModel);
            }
            return null;
        }

        private string GetSpNameBasedOnBalanceUpDay(DateTime fromDate, DateTime balanceUpDay) => fromDate < balanceUpDay ? STR_SP_BETLIST_MATCHWL_OLD : STR_SP_BETLIST_MATCHWL;
    }
}
