using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Microsoft.AspNetCore.Mvc;


namespace Fanex.Athena.BetList.Controllers
{
    [BearerAuthorize(Roles.TotalBetCorrectScore)]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CorrectScoreBetListController : BetListBaseController
    {
        public CorrectScoreBetListController(
            IContextService contextService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            IBalanceUpDataAccess balanceUpDataAccess,
            IAuthorizationService authorizationService,
            ISubsidiaryService subsidiaryService)
            : base(contextService, authorizationService, betListService, ticketBuilderService, subsidiaryService, balanceUpDataAccess) 
        { }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList:true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<BetListResultViewModel> GetData(BetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_TotalBets_Sel430";
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);

            return await GetBelistResultAsync(metaModel);
        }
    }
}
