using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [BearerAuthorize(Roles.ReportSabaVirtualSportsWinLoss)]
    [RequestValidation(validateCurrency: true)]
    [ApiBetlistAuthorize]
    public class SabaVirtualSportsWinLossBetListController : BetListBaseController
    {
        private readonly IReportSettingService _reportSettingService;
        private static readonly int[] HiddenColumns = [ExternalIdColumn, SystemIdColumn, BetEffectiveColumn];
        public SabaVirtualSportsWinLossBetListController(IContextService contextService,
            IAuthorizationService authorizationService,
            IBetListService betListService,
            ITicketBuilderService ticketBuilderService,
            ISubsidiaryService subsidiaryService,
            IBalanceUpDataAccess balanceUpDataAccess,
            IReportSettingService reportSettingService,
            ICacheRefAdapter cacheRefAdapter,
            IAppSettings appSettings,
            IIPService iPService)
            : base(contextService,
                   authorizationService,
                   betListService,
                   ticketBuilderService,
                   subsidiaryService,
                   balanceUpDataAccess,
                   cacheRefAdapter,
                   appSettings,
                   iPService)
        {
            _reportSettingService = reportSettingService;
        }

        [HttpGet]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> Index([FromQuery] BetListMetaModel metaModel, bool dataOnly = true)
        {
            if (!IsValidDateRange(metaModel.FromDate, metaModel.ToDate))
            {
                return null;
            }

            await AssignDefaultMetaModel(metaModel);

            if (dataOnly)
            {
                return await GetBetListPagingResultAsync(metaModel);
            }

            var settings = CreateDefaultDayRangeSettings(metaModel);

            var systemIdTitle = _contextService.IsFakeLicensee ? string.Empty : $" ({metaModel.SystemId})";
            settings.Title = metaModel.ExternalId + systemIdTitle;

            return await GetBetListResultWithSettings(metaModel, settings);
        }


        [HttpPost]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<IActionResult> ExportExcel([FromBody] BetListExcelMetaModel metaModel)
        {
            await AssignDefaultMetaModel(metaModel);

            metaModel.Title = "Customer " + ViewResources.BetList + string.Format(
                                         " {0:dd/MM/yyyy} --> {1:dd/MM/yyyy} - {2} {3} - {4}",
                                         metaModel.FromDate,
                                         metaModel.ToDate,
                                         metaModel.ExternalId,
                                         _contextService.IsFakeLicensee
                                             ? string.Empty
                                             : $"({metaModel.SystemId})",
                                         GetViewInCurrency(metaModel));

            metaModel.ShowingOption = new BetListShowingOption
            {
                CustomerCommVisible = _contextService.IsAllowedCustomerComm
            };
            metaModel.HiddenColumns = HiddenColumns;

            return await ProcessExportExcel(metaModel);
        }

        private async Task AssignDefaultMetaModel(BetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_SabaVirtualSports_Mem";
            await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
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
