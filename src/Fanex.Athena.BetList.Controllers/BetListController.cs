using Fanex.Athena.BetList.Controllers.CustomActionResult;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
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
