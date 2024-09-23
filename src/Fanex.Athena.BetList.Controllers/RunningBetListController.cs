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
    public class RunningBetListController : BetListBaseController
    {
        public RunningBetListController(IContextService contextService, 
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
        [BearerAuthorize(Roles.TotalBetCleanSheet)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> CleanSheet(BetListMetaModel metaModel)
        {
            metaModel.BetType = (int)BetType.CleanSheet;
            return ProcessRequest(metaModel);
        }

        [HttpPost]
        [RequestValidation]
        [BearerAuthorize(Roles.TotalBetToWinToNil)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> ToWinToNil(BetListMetaModel metaModel)
        {
            metaModel.BetType = (int)BetType.ToWinToNil;
            return ProcessRequest(metaModel);
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [BearerAuthorize(Roles.TotalBet3WayHandicap)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public Task<dynamic> ThreeWayHandicap(BetListMetaModel metaModel)
        {
            metaModel.MultipleBettype = $"{(int)BetType.ThreeWayHandicap},{(int)BetType.FirstHalfThreeWayHandicap}";
            return ProcessRequest(metaModel, "Lrf_BetList_SelRunningByBettypeList");
        }

        private async Task<dynamic> ProcessRequest(BetListMetaModel metaModel, string uspKey = "Lrf_BetList_SelRunningByBettype")
        {
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, uspKey);
            return await GetBelistResultAsync(metaModel);
        }
        
    }
}
