
namespace Fanex.Athena.BetList.Controllers
{
    using Common.Utility.Services.Interfaces;
    using Excel;
    using Fanex.Athena.BetList.Controllers.CustomActionResult;
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.BetList.Service.Interfaces;
    using Fanex.Athena.Common.DataAccess.Interfaces;
    using Fanex.Athena.Common.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Model.ViewModel;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BetListController : BetListBaseController
    {
        private const string DefaultPage = "Betlist Api default page";

        private readonly ICacheRefAdapter _cacheRefAdapter;
        private readonly IIPService _ipService;
        private readonly IAppSettings _appSettings;

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
                balanceUpDataAccess)
        {
            _cacheRefAdapter = cacheRefAdapter;
            _ipService = ipService;
            _appSettings = appSettings;
        }

        [HttpGet]
        public string Index()
        {
            return DefaultPage;
        }

        [HttpPost]
        public Task<BetListResultViewModel> GetBetList(BetListMetaModel metaModel)
            => GetBelistResultAsync(metaModel);


        [HttpPost]
        public Task<object> GetBetListPaging(BetListMetaModel metaModel)
            => GetBetListPagingResultAsync(metaModel);

        [HttpPost]
        public async Task<ExcelResult> ExportExcel(BetListExcelMetaModel metaModel)
        {
            return await ProcessExportExcel(metaModel);
        }

        [HttpPost]
        public async Task<ExcelResult> ExportExcelLiveReport(LiveReportBetListMetaModel metaModel)
        {
            return await ProcessExportExcel(metaModel);
        }

        protected internal virtual ExcelResult Export(BetListExcelMetaModel metaModel, TicketResult ticketResult)
        {
            metaModel.ShowingOption.IsHideSaba = metaModel.IsHideSaba;
            metaModel.ShowingOption.IsShowSumStake = metaModel.IsShowSumStake;
            metaModel.ShowingOption.IsShowIpvn = metaModel.IsShowIpvn;
            metaModel.ShowingOption.IsShowIp = metaModel.IsShowIp;
            var excel = new BetListExcel(metaModel.ShowingOption, metaModel.LicenseeId, metaModel.HiddenColumns, _appSettings, _ipService);

            var workbook = excel.Export(ticketResult, metaModel.Title, metaModel.StartIndex);

            return new ExcelResult(workbook);
        }

        protected internal virtual async Task<ExcelResult> ProcessExportExcel(BetListExcelMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetBetListData(metaModel);

            const string LeagueNamePlaceHolder = "(LEAGUENAME)";
            if (metaModel.Title.Contains(LeagueNamePlaceHolder))
            {
                var firstTicket = ticketResult.MainTicketList.FirstOrDefault();
                var leagueName = firstTicket != null ? _cacheRefAdapter.GetLeagueName(firstTicket.LeagueId) : string.Empty;
                metaModel.Title = metaModel.Title.Replace(LeagueNamePlaceHolder, leagueName);
            }

            return Export(metaModel, ticketResult);
        }
    }
}
