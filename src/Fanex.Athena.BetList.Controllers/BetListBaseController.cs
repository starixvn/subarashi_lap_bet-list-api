using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.BetList.Service.Optiions;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Model.MetaModel;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Helpers;


namespace Fanex.Athena.BetList.Controllers
{
    public class BetListBaseController
    {
        protected readonly IContextService _contextService;
        protected readonly IAuthorizationService _authorizationService;
        protected readonly IBetListService _betListService;
        protected readonly IBalanceUpDataAccess _balanceUpDataAccess;
        protected readonly ITicketBuilderService _ticketBuilderService;
        private readonly ISubsidiaryService _subsidiaryService;

        protected BetListBaseController(IContextService contextService,
            IAuthorizationService authorizationService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess balanceUpDataAccess
            )
        {
            _contextService = contextService;
            _authorizationService = authorizationService;
            _balanceUpDataAccess = balanceUpDataAccess;
            _betListService = betListService;
            _ticketBuilderService = ticketBuilderService;
            _subsidiaryService = subsidiaryService;
        }

        protected void CheckDataPermission(int custId, long transId)
        {
            if (!_authorizationService.HasPermissionOnData(
                _contextService.AgentList,
                custId,
                transId))
            {
                throw new UnauthorizedAccessException();
            }
        }

        protected async Task CheckPermissionAndAssignCommonValueToMetaModel(BetListMetaModel metaModel, string uspKey)
        {
            CheckDataPermission(metaModel.CustId, metaModel.TransId);

            var balanceUp = GetBalanceUp();
            metaModel.PageIndex = 1;
            metaModel.PageSize = int.MaxValue - 1;
            metaModel.LicenseeId = _contextService.LicenseeId;
            metaModel.BalanceUpDay = DateTime.SpecifyKind(balanceUp.BalanceUpDay.Date, DateTimeKind.Utc);
            metaModel.MoveBetsDay = DateTime.SpecifyKind(balanceUp.MoveBetsDay.Date, DateTimeKind.Utc);
            metaModel.HasPlayerCom = _contextService.IsAllowedCustomerComm;
            metaModel.UspKey = uspKey;
            metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(await CurrencyAgentListBySubsidiaryBusiness(metaModel), metaModel.CurrencyId);
        }

        protected void AssignPageInfoToMetaModel(BetListMetaModel metaModel, int pageIndex, int pageSize)
        {
            const int DefaultPageSize = 100;
            metaModel.PageIndex = pageIndex < 1 ? 1 : pageIndex;
            metaModel.PageSize = pageSize < 1 ? DefaultPageSize : pageSize;
        }

        protected async Task<BetListResultViewModel> GetBelistResultAsync(BetListMetaModel metaModel)
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
                buildTicketOption: new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom, IsMobileView = metaModel.IsMobileView });

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

        protected async Task<object> GetBetListPagingResultAsync(BetListMetaModel metaModel)
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
                rows = _ticketBuilderService.BuildTicketText(ticketResult,
                    metaModel,
                    startIndex,
                    new BuildTicketOption
                    {
                        IsAllowedCustomerComm = metaModel.HasPlayerCom,
                        IsHideSaba = metaModel.IsHideSaba
                    }),
                userdata = new
                {
                    WinLoss = sumWinLossText,
                    LicenseeWL = sumLicenseeWlText
                }
            };

            BetListHelper.ClearData(ticketResult);
            return result;
        }

        protected BalanceUp GetBalanceUp()
            => _balanceUpDataAccess.GetBalanceUp();

        private async Task<IDictionary<int, int[]>> CurrencyAgentListBySubsidiaryBusiness(IMetaModelSubsidiarySearch metaModel)
            => _subsidiaryService.IsIgnoredSubsidiarySearch(metaModel.SubsidiarySearch, metaModel.SubsidiaryList) ? _contextService.AgentList.ToDictionary(x => x.Key, x => x.Value)
            : await _subsidiaryService.GetAgentList(metaModel.SubsidiaryList);

    }
}
