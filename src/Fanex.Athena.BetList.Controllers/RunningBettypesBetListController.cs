using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
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
             IBalanceUpDataAccess balanceUpDataAccess)
            : base(contextService, authoriztionService, betListService, ticketBuilderService, subsidiaryService, balanceUpDataAccess)
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
            return await GetBelistResultAsync(metaModel);
        }
    }
}
