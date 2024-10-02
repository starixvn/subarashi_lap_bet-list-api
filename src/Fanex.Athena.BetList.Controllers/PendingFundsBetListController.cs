using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [BearerAuthorize(Roles.ReportCustomerPendingFunds)]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PendingFundsBetListController : BetListBaseController
    {
        public PendingFundsBetListController(IContextService contextService,
            IAuthorizationService authoriztionService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess balanceUpDataAccess)
            : base(contextService, authoriztionService, betListService, ticketBuilderService, subsidiaryService, balanceUpDataAccess)
        { }

        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<dynamic> GetData(BetListMetaModel metaModel)
        {
            const string UspKey = "Shard_Lrf_BetList_SelRunningByDate_Mem";

            metaModel.FromDate = DateTimeHelper.Today.AddYears(-1);
            metaModel.ToDate = DateTimeHelper.Today.AddYears(1);

            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);

            return await GetBelistResultAsync(metaModel);
        }
    }
}
