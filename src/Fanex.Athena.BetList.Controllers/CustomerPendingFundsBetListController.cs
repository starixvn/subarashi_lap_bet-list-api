namespace Fanex.Athena.BetList.Controllers;

using Common.Constant;
using Common.DataAccess.Interfaces;
using Common.Services.Interfaces;
using Common.Site.Attributes;
using Common.Utility.Helpers;
using Common.Utility.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.MetaModel;
using Resources;
using Service.Interfaces;

[BearerAuthorize(Roles.ReportCustomerPendingFunds)]
[ApiController]
[RequestValidation(validateCurrency: true)]
[Route("api/[controller]/[action]")]
public class CustomerPendingFundsBetListController : BetListBaseController
{
    private static readonly int[] HiddenColList = [ExternalIdColumn, SystemIdColumn, WinLostColumn, BetEffectiveColumn, LicenseeWLColumn];

    public CustomerPendingFundsBetListController(
        IContextService contextService,
        IAuthorizationService authorizationService,
        IBetListService betListService,
        ITicketBuilderService ticketBuilderService,
        ISubsidiaryService subsidiaryService,
        IBalanceUpDataAccess balanceUpDataAccess,
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
    }

    [HttpGet]
    [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
    public async Task<object> Index([FromQuery] BetListMetaModel metaModel, bool dataOnly = true)
    {
        await AssignDefaultMetaModel(metaModel);

        if (dataOnly)
        {
            return await GetBetListPagingResultAsync(metaModel);
        }

        var settings = CreateDefaultSettings(metaModel);
        settings.Title = $"{metaModel.ExternalId}{(_contextService.IsFakeLicensee ? string.Empty : $" ({metaModel.SystemId})")}";

        return await GetBetListResultWithSettings(metaModel, settings);
    }

    [HttpPost]
    [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
    public async Task<IActionResult> ExportExcel([FromBody] BetListExcelMetaModel metaModel)
    {
        await AssignDefaultMetaModel(metaModel);

        metaModel.StartIndex = (metaModel.PageIndex - 1) * metaModel.PageSize + 1;
        metaModel.Title = "Running " + ViewResources.BetList
                                     + string.Format(
                                         " - {0} {1} - {2}",
                                         metaModel.ExternalId,
                                         _contextService.IsFakeLicensee
                                             ? string.Empty
                                             : $"({metaModel.SystemId})",
                                         GetViewInCurrency(metaModel));
        metaModel.HiddenColumns = HiddenColList;
        metaModel.ShowingOption = new BetListShowingOption
        {
            CustomerCommVisible = _contextService.IsAllowedCustomerComm
        };

        return await ProcessExportExcel(metaModel);
    }

    private async Task AssignDefaultMetaModel(BetListMetaModel metaModel)
    {
        const string UspKey = "Shard_Lrf_BetList_SelRunningByDate_Mem";

        metaModel.FromDate = DateTimeHelper.Today.AddYears(-1);
        metaModel.ToDate = DateTimeHelper.Today.AddYears(1);

        await CheckPermissionAndAssignCommonValueToMetaModel(metaModel, UspKey);
    }
}