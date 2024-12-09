using Fanex.Athena.BetList.Model.MetaModel;
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
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MatchWinLossBetListController : BetListBaseController
    {
        private readonly string STR_SP_BETLIST_MATCHWL_OLD = "Lrf_BetList_MatchWL_Old";
        private readonly string STR_SP_BETLIST_MATCHWL = "Lrf_BetList_MatchWL";

        private readonly ICurrencyValidation _currencyValidation;

        public MatchWinLossBetListController(IContextService contextService,
            IAuthorizationService authorizationService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess balanceUpDataAccess,
            ICurrencyValidation currencyValidation)
            : base(contextService, authorizationService, betListService, ticketBuilderService, subsidiaryService, balanceUpDataAccess)
        {
            _currencyValidation = currencyValidation;
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> BetTypeWinLoss(int page, int rows, [FromQuery] BetListMetaModel metaModel)
        {
            var balanceUp = GetBalanceUp();
            var UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            AssignPageInfoToMetaModel(metaModel, page, rows);

            return await GetBetListPagingResultAsync(metaModel);
        }

        [HttpGet]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> MatchBetType(int page, int rows, [FromQuery] BetListMetaModel metaModel)
        {
            var balanceUp = GetBalanceUp();
            var UspKey = GetSpNameBasedOnBalanceUpDay(metaModel.FromDate, balanceUp.BalanceUpDay);
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
        public async Task<object> MixParlay(MixParlayMetaModel metaModel)
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
                    SubsidiarySearch = metaModel.SubsidiarySearch.GetValueOrDefault(),
                    SubsidiaryList = metaModel.SubsidiaryList,
                    AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(_ => _.Key, _ => _.Value), metaModel.CurrencyId),
                    HasPlayerCom = _contextService.IsAllowedCustomerComm,
                    IsShowIp = _contextService.IsShowIP,
                    IsShowIpvn = _contextService.IsShowIpvn
                };

                await CheckPermissionAndAssignCommonValueToMetaModel(param, UspKey);
                AssignPageInfoToMetaModel(param, metaModel.Page, metaModel.Rows);

                return await GetBetListPagingResultAsync(param);
            }
            return null;
        }

        private string GetSpNameBasedOnBalanceUpDay(DateTime fromDate, DateTime balanceUpDay) => fromDate < balanceUpDay ? STR_SP_BETLIST_MATCHWL_OLD : STR_SP_BETLIST_MATCHWL;
    }
}
