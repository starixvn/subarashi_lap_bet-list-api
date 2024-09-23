using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Microsoft.AspNetCore.Mvc;


namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RunningMatchBetListController : BetListBaseController
    {
        public RunningMatchBetListController(IContextService contextService,
             IAuthorizationService authoriztionService,
             IBetListService betListService,
             ITicketBuilderService ticketBuilderService,
             ISubsidiaryService subsidiaryService,
             IBalanceUpDataAccess balanceUpDataAccess)
            : base(contextService, authoriztionService, betListService, ticketBuilderService, subsidiaryService, balanceUpDataAccess)
        {
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<BetListResultViewModel> GetData(BetListMetaModel metaModel)
        {
            const string UspKey = "Shard_Lrf_BetList_SelRunningByBettypeMatch";
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            return await GetBelistResultAsync(metaModel);
        }
    }
}
