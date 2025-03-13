using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [BearerAuthorize(Roles.ReportCustomerPendingFunds)]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PendingFundsBetListController : BetListBaseController
    {
        public PendingFundsBetListController(IContextService contextService,
            IAuthorizationService authorizationService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess balanceUpDataAccess,
            ICacheRefAdapter cacheRefAdapter,
            IAppSettings appSettings,
            IIPService iPService)
            : base(contextService, authorizationService, betListService, ticketBuilderService, subsidiaryService,
                  balanceUpDataAccess, cacheRefAdapter, appSettings, iPService)
        { }

        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<dynamic> GetData(BetListMetaModel metaModel)
        {
            const string UspKey = "Shard_Lrf_BetList_SelRunningByDate_Mem";

            metaModel.FromDate = DateTimeHelper.Today.AddYears(-1);
            metaModel.ToDate = DateTimeHelper.Today.AddYears(1);

            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);

            return await GetBetListResultAsync(metaModel);
        }
    }
}
