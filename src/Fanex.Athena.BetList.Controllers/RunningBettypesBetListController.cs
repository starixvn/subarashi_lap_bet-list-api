namespace Fanex.Athena.BetList.Controllers
{
    using Common.Constant;
    using Common.DataAccess.Interfaces;
    using Common.Services.Interfaces;
    using Common.Site.Attributes;
    using Common.Utility.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RunningBettypesBetListController : BetListBaseController
    {
        private const string DoubleChangeBettypes = "24,151,186,431,410";
        private const string BONTeamToScoreBettypes = "26,145,146,433,170,188,427";

        private readonly int[] _parlayBettypes = new[] { (int)BetType.CombinationMixParlay, (int)BetType.SingleMatchParlay };

        public RunningBettypesBetListController(IContextService contextService,
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
        [RequestValidation]
        [BearerAuthorize(Roles.TotalBetMixParlay)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> MixParlay(BetListMetaModel metaModel)
        {
            metaModel.MultipleBettype = string.Join(",", _parlayBettypes);
            return ProcessRequest(metaModel);
        }

        [HttpPost]
        [BearerAuthorize(Roles.TotalBetDoubleChance)]
        [RequestValidation]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> DoubleChange(BetListMetaModel metaModel)
        {
            metaModel.MultipleBettype = DoubleChangeBettypes;
            return ProcessRequest(metaModel);
        }

        [HttpPost]
        [BearerAuthorize(Roles.TotalBetBONTeamToScore)]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> BONTeamToScore(BetListMetaModel metaModel)
        {
            metaModel.MultipleBettype = BONTeamToScoreBettypes;
            return ProcessRequest(metaModel);
        }

        private async Task<dynamic> ProcessRequest(BetListMetaModel metaModel)
        {
            const string uspKey = "Lrf_BetList_SelRunningByBettypeList";
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, uspKey);
            return await GetBetListResultAsync(metaModel);
        }
    }
}
