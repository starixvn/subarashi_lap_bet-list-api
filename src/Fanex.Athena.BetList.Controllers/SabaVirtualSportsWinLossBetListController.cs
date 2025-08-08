namespace Fanex.Athena.BetList.Controllers
{
    using Common.Constant;
    using Common.DataAccess.Interfaces;
    using Common.Services.Interfaces;
    using Common.Site.Attributes;
    using Common.Utility.Helpers;
    using Common.Utility.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("api/[controller]/[action]")]
    [BearerAuthorize(Roles.ReportSabaVirtualSportsWinLoss)]
    [ApiBetlistAuthorize]
    public class SabaVirtualSportsWinLossBetListController : BetListBaseController
    {
        private readonly IReportSettingService _reportSettingService;

        public SabaVirtualSportsWinLossBetListController(IContextService contextService,
            IAuthorizationService authoriztionService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess balanceUpDataAccess,
            IReportSettingService reportSettingService,
            ICacheRefAdapter cacheRefAdapter,
            IAppSettings appSettings,
            IIPService iPService)
            : base(contextService, authoriztionService, betListService, ticketBuilderService, subsidiaryService, 
                  balanceUpDataAccess, cacheRefAdapter, appSettings, iPService)
        {
            _reportSettingService = reportSettingService;
        }

        [HttpPost]
        [RequestValidation(validateCurrency: true)]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<dynamic> GetData(BetListMetaModel metaModel)
        {
            if (!IsValidDateRange(metaModel.FromDate, metaModel.ToDate))
            {
                return null;
            }

            const string UspKey = "Lrf_BetList_SabaVirtualSports_Mem";
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
            return await GetBetListResultAsync(metaModel);
        }

        private bool IsValidDateRange(DateTime fromDate, DateTime toDate)
        {
            var setting = _reportSettingService.GetSetting(Roles.ReportLicenseeWinLoss, _contextService.SiteName);
            if (setting == null)
            {
                return true;
            }

            return DateTimeHelper.IsBetween(fromDate, setting.MinDate, setting.MaxDate) &&
                DateTimeHelper.IsBetween(toDate, setting.MinDate, setting.MaxDate);
        }

    }
}
