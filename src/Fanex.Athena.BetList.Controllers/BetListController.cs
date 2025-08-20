namespace Fanex.Athena.BetList.Controllers
{
    using Common.DataAccess.Interfaces;
    using Common.Services.Interfaces;
    using Common.Utility.Services.Interfaces;
    using CustomActionResult;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Model.ViewModel;
    using Service.Interfaces;

    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BetListController : BetListBaseController
    {
        private const string DefaultPage = "Betlist Api default page";

        public BetListController(
            IContextService contextService,
            ICacheRefAdapter cacheRefAdapter,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            IIPService ipService,
            IBalanceUpDataAccess balanceUpDataAccess,
            IAuthorizationService authorizationService,
            ISubsidiaryService subsidiaryService,
            IAppSettings appSettings)
            : base(contextService,
                authorizationService,
                betListService,
                ticketBuilderService,
                subsidiaryService,
                balanceUpDataAccess,
                cacheRefAdapter,
                appSettings,
                ipService)
        {
        }

        [HttpGet]
        public string Index()
        {
            return DefaultPage;
        }

        [HttpPost]
        public Task<BetListResultViewModel> GetBetList(BetListMetaModel metaModel)
            => GetBetListResultAsync(metaModel);

        [HttpPost]
        public Task<object> GetBetListPaging(BetListMetaModel metaModel)
            => GetBetListPagingResultAsync(metaModel);

        [HttpPost]
        public async Task<ExcelResult> ExportExcel(BetListExcelMetaModel metaModel)
        {
            return await ProcessExportExcel(metaModel);
        }
    }
}
