namespace Fanex.Athena.BetList.Controllers.UnitTests;

using System.Net;
using Common.Services.Interfaces;
using Common.Utility.Collections;
using Common.Utility.Services.Interfaces;
using DataAccess.Interfaces;
using Excel;
using Excel.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Model.DTO;
using Model.MetaModel;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Resources;
using Service.Interfaces;
using TestHelper;
using TestHelper.Extensions;
using Utility;

public class BettingTicketCenterAdvancedSearchControllerTests
{
    private const int RowsExceedLimitErrorCode = 1001;
    private const int ExportMultipleFile = 1002;

    private IAppSettings _appSettings;
    private IBettingTicketCenterExcel _bettingTicketCenterExcel;
    private IContextService _contextService;
    private ICustomerClassService _customerClassService;
    private IBettingTicketCenterAdvancedSearchDataAccess _dataAccess;
    private IIPService _ipService;
    private ISportService _sportService;
    private ITempFileService _tempFileService;
    private ITicketBuilderService _ticketBuilderService;
    private IBettingTicketCenterZipBuilder _zipBuilder;

    [SetUp]
    public void SetUp()
    {
        _ticketBuilderService = Substitute.For<ITicketBuilderService>();
        _appSettings = Substitute.For<IAppSettings>();
        _dataAccess = Substitute.For<IBettingTicketCenterAdvancedSearchDataAccess>();
        _zipBuilder = Substitute.For<IBettingTicketCenterZipBuilder>();
        _bettingTicketCenterExcel = Substitute.For<IBettingTicketCenterExcel>();
        _ipService = Substitute.For<IIPService>();
        _tempFileService = Substitute.For<ITempFileService>();
            _sportService = Substitute.For<ISportService>();
        _contextService = Substitute.For<IContextService>();
        _customerClassService = Substitute.For<ICustomerClassService>();
    }

    [TearDown]
    public void TearDown()
    {
        _bettingTicketCenterExcel.Dispose();
        _appSettings = null;
        _dataAccess = null;
        _zipBuilder = null;
        _bettingTicketCenterExcel = null;
        _ticketBuilderService = null;
        _tempFileService = null;
        _ipService = null;
        _contextService = null;
        _customerClassService = null;
        _sportService = null;
    }

    [Test]
    public async Task GetData_WhenGet_GenerateDataWithCorrectOption()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, false);
        var cancellationToken = new CancellationToken();

        var totalRow = FakeHelper.Create<int>();
        var maxTotalRows = FakeHelper.Create<int>();
        _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, totalRow, cancellationToken).Returns(totalRow);
        _appSettings.MaxTicketLimit.Returns(totalRow);

        var ticketResult = new TicketResult();
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, cancellationToken).Returns(ticketResult);

        // Action
        await controller.GetData(metaModel, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            controller.Received(1).InvokeProtectedMethod("ReturnData",
                ticketResult,
                Arg.Is<BettingTicketCenterOption>(x =>
                    x.ShowTotal
                    && x.ShowSumStake
                    && x.ShowLicenseeWl
                    && x.ShowWinLoss
                    && x.ShowExternalId
                    && x.ShowSystemId
                    && x.IsAllowedCustomerComm == metaModel.HasPlayerCom),
                metaModel);
        });
    }

    [Test]
    public async Task GetData_WhenExportExcel_NotExistAnyDataPath_ReturnError()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();

        var totalRow = FakeHelper.Create<int>();
        var maxRowPerFile = totalRow - 1;
        _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

        var ticketResult = new GroupTicketResult
        {
            DataPath = new List<string>(),
            TotalRows = 0
        };
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile, cancellationToken).Returns(ticketResult);

        // Action
        var result = await controller.GetData(metaModel, cancellationToken) as StatusCodeResult;

        // Assert
        Assert.Multiple(() => { Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent)); });
    }

    [Test]
    public async Task GetData_WhenExportExcel_RowsExceedMaxRowPerFile_NoMainTicket_ReturnError()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();

        var totalRow = FakeHelper.Create<int>();
        var maxRowPerFile = totalRow - 1;
        _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

        var ticketResult = new GroupTicketResult
        {
            DataPath = new List<string> { "123" },
            TotalRows = 0
        };
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile, cancellationToken).Returns(ticketResult);

        // Action
        var result = await controller.GetData(metaModel, cancellationToken) as StatusCodeResult;

        // Assert
        Assert.Multiple(() => { Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent)); });
    }

    [Test]
    public async Task GetData_WhenExportExcel_NotInListAccessIP_ReturnError()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();
        metaModel.Ip = FakeHelper.Create<string>();
        metaModel.IsShowIpvn = false;

        var totalRow = FakeHelper.Create<int>();
        var maxRowPerFile = totalRow - 1;
        _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);
        _appSettings.MaxRowForMultipleExcelFiles.Returns(maxRowPerFile);
        metaModel.PageSize = maxRowPerFile;

        var ticketResult = new GroupTicketResult
        {
            DataPath = new List<string> { "123" },
            TotalRows = 1
        };
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile, cancellationToken).Returns(ticketResult);
        _ipService.IsVNIP(metaModel.Ip).Returns(true);

        // Action
        var result = await controller.GetData(metaModel, cancellationToken) as StatusCodeResult;

        // Assert
        Assert.Multiple(() => { Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent)); });
    }

    [Test]
    public async Task GetData_WhenExportExcel_ExportWithCorrectParameter()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();
        var filePath = FakeHelper.Create<string>();

        var totalRow = FakeHelper.Create<int>();
        var maxRowPerFile = totalRow + 1;
        _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);
        _appSettings.MaxRowForMultipleExcelFiles.Returns(maxRowPerFile);
        metaModel.PageSize = maxRowPerFile;

        var ticketResult = new GroupTicketResult
        {
            TotalRows = totalRow
        };

        ticketResult.DataPath.Add(filePath);
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile, cancellationToken).Returns(ticketResult);

        var data = new TicketResult { MainTicketList = new SortableList<LrfTicket> { FakeHelper.Create<LrfTicket>() } };
        var jsonData = JsonConvert.SerializeObject(data);
        _tempFileService.ReadString(Arg.Is(filePath)).Returns(jsonData);

        // Action
        await controller.GetData(metaModel, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            controller.Received(1).InvokeProtectedMethod("ExportExcel",
                Arg.Is<BettingTicketCenterExcelModel>(x =>
                    x.TotalText == $"{ViewResources.Totalin} {metaModel.BaseCurrencyName}"
                    && x.Option.IsAllowedCustomerComm == metaModel.HasPlayerCom));
        });
    }

    [Test]
    public async Task GetData_ExportZipFile_ZipBuilderReturnNull_ReturnNull()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();

        var totalRow = FakeHelper.Create<int>();

        var maxRowPerFile = totalRow - 1;
        _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

        var ticketResult = new GroupTicketResult
        {
            TotalRows = totalRow,
            DataPath = new List<string> { "123" }
        };
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile, cancellationToken).Returns(ticketResult);

        SetUpForResponse(controller);
        _zipBuilder.BuildZip(
            ticketResult,
            Arg.Any<string>(),
            Arg.Any<AdvancedSearchBetListMetaModel>(),
            Arg.Any<string>(),
            cancellationToken).Returns(x => null);

        // Action
        var result = await controller.GetData(metaModel, cancellationToken);

        // Assert
        Assert.Multiple(() => { Assert.That(result, Is.Null); });
    }

    [Test]
    public async Task GetData_ExportZipFile_WhenHaveMutipleType_ResponsePartialContent()
    {
        // Arrange
        var controller = CreateController();
        var totalRow = FakeHelper.Create<int>();
        var maxRowPerFile = totalRow - 1;
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();
        metaModel.PageSize = maxRowPerFile;

        _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);
        _appSettings.MaxRowForMultipleExcelFiles.Returns(maxRowPerFile);

        var ticketResult = new GroupTicketResult
        {
            TotalRows = totalRow,
            DataPath = new List<string> { "123" },
            CurrentPageRowCount = maxRowPerFile
        };
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile, cancellationToken).Returns(ticketResult);

        SetUpForResponse(controller);
        _zipBuilder.BuildZip(
            ticketResult,
            Arg.Any<string>(),
            Arg.Any<AdvancedSearchBetListMetaModel>(),
            Arg.Any<string>(),
            cancellationToken).Returns(x => null);

        // Action
        var result = await controller.GetData(metaModel, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Null);
            Assert.That(controller.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.PartialContent));
        });
    }

    [Test]
    public async Task GetData_ExportZipFile_ZipBuilderReturnNotNull_ReturnZipFileContent()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();

        var totalRow = FakeHelper.Create<int>();

        var maxRowPerFile = totalRow - 1;
        _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

        var ticketResult = new GroupTicketResult
        {
            TotalRows = totalRow,
            DataPath = new List<string> { "123" }
        };
        _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile, cancellationToken).Returns(ticketResult);

        SetUpForResponse(controller);
        _zipBuilder.BuildZip(
            ticketResult,
            Arg.Any<string>(),
            Arg.Any<AdvancedSearchBetListMetaModel>(),
            Arg.Any<string>(),
            cancellationToken).Returns(x => new MemoryStream());

        // Action
        var result = await controller.GetData(metaModel, cancellationToken);

        // Assert
        Assert.Multiple(() => { Assert.That(result, Is.TypeOf(typeof(FileContentResult))); });
    }

    [Test]
    public async Task GetExportStatus_WhenTotalRowExceedMaxTicketLimit_ReturnRowsExceedLimitErrorCode()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();

        var totalRow = FakeHelper.Create<int>();
        _appSettings.MaxTicketLimit.Returns(totalRow - 1);
        _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, _appSettings.MaxTicketLimit, cancellationToken).Returns(totalRow);

        // Action
        var result = (await controller.GetExportStatus(metaModel, cancellationToken) as JsonResult).Value;
        var code = result.GetValue("StatusCode");

        // Assert
        Assert.Multiple(() => { Assert.That(code, Is.EqualTo(RowsExceedLimitErrorCode)); });
    }

    [Test]
    public async Task GetExportStatus_WhenTotalRowNotExceedMaxRowSupportSingleExportExcel_ReturnMaxRowForMultipleExcelFiles()
    {
        // Arrange
        var controller = CreateController();
        var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
        var cancellationToken = new CancellationToken();

        var totalRow = FakeHelper.Create<int>();
        _appSettings.MaxTicketLimit.Returns(totalRow + 1);
        _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, _appSettings.MaxTicketLimit, cancellationToken).Returns(totalRow);

        // Action
        var result = (await controller.GetExportStatus(metaModel, cancellationToken) as JsonResult).Value;
        var code = result.GetValue("StatusCode");

        // Assert
        Assert.Multiple(() => { Assert.That(code, Is.EqualTo(ExportMultipleFile)); });
    }

    private static void SetUpForResponse(BettingTicketCenterAdvancedSearchController controller)
    {
        var response = Substitute.For<HttpResponse>();

        var httpContext = Substitute.For<HttpContext>();
        httpContext.Response.Returns(response);
        var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());

        controller.ControllerContext = new ControllerContext(actionContext);
    }

    private BettingTicketCenterAdvancedSearchController CreateController()
    {
        var controller = Substitute.ForPartsOf<BettingTicketCenterAdvancedSearchController>(
            _appSettings,
            _dataAccess,
            _ipService,
            _ticketBuilderService,
            _zipBuilder,
            _bettingTicketCenterExcel,
            _contextService,
            _customerClassService,
            _tempFileService,
            _sportService);

        controller.When(x =>
                x.InvokeProtectedMethod("ReturnData", Arg.Any<TicketResult>(), Arg.Any<BettingTicketCenterOption>(), Arg.Any<ICommonValueMetaModel>()))
            .DoNotCallBase();
        controller.When(x => x.InvokeProtectedMethod("ExportExcel", Arg.Any<BettingTicketCenterExcelModel>())).DoNotCallBase();

        return controller;
    }
}