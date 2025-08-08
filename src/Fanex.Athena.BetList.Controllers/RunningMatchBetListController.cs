namespace Fanex.Athena.BetList.Controllers
{
    using Common.DataAccess.Interfaces;
    using Common.Services.Interfaces;
    using Common.Site.Attributes;
    using Common.Utility.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Model.ViewModel;
    using Service.Interfaces;

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RunningMatchBetListController : BetListBaseController
    {
        public RunningMatchBetListController(IContextService contextService,
             IAuthorizationService authoriztionService,
             IBetListService betListService,
             ITicketBuilderService ticketBuilderService,
             ISubsidiaryService subsidiaryService,
             IBalanceUpDataAccess balanceUpDataAccess,
             ICacheRefAdapter cacheRefAdapter,
             IAppSettings appSettings,
             IIPService iPService)
            : base(contextService, authoriztionService, betListService, ticketBuilderService, subsidiaryService,
                  balanceUpDataAccess, cacheRefAdapter, appSettings, iPService)
        {
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<BetListResultViewModel> GetData(BetListMetaModel metaModel)
        {
            const string UspKey = "Shard_Lrf_BetList_SelRunningByBettypeMatch";
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            return await GetBetListResultAsync(metaModel);
        }
    }
}
