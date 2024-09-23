using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Extension.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
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
            IReportSettingService reportSettingService) 
            : base(contextService, authoriztionService, betListService, ticketBuilderService, subsidiaryService, balanceUpDataAccess)
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
            return await GetBelistResultAsync(metaModel);
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
