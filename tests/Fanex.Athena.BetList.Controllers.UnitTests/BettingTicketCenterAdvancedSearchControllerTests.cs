using System.Net;
using Fanex.Athena.BetList.Controllers.Utility;
using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Collections;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using Fanex.Athena.TestHelper;
using Fanex.Athena.TestHelper.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class BettingTicketCenterAdvancedSearchControllerTests
    {
        private const int RowsExceedLimitErrorCode = 1001;
        private const int ServerBusyErrorCode = 1002;

        private const int ExportMultipleFile = 1002;
        private const int ExportSingleFile = 1003;

        private IAppSettings _appSettings;
        private IBettingTicketCenterAdvancedSearchDataAccess _dataAccess;
        private IBettingTicketCenterZipBuilder _zipBuilder;
        private IBettingTicketCenterExcel _bettingTicketCenterExcel;
        private ITicketBuilderService _ticketBuilderService;
        private ITempFileService _tempFileService;
        private IIPService _ipService;
        private IContextService _contextService;
        private ICustomerClassService _customerClassService;
        private IBettingTicketCenterSportService _sportService;

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
            _sportService = Substitute.For<IBettingTicketCenterSportService>();
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

            var totalRow = FakeHelper.Create<int>();
            var maxTotalRows = FakeHelper.Create<int>();
            _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, totalRow).Returns(totalRow);
            _appSettings.MaxTicketLimit.Returns(totalRow);

            var ticketResult = new TicketResult();
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetData(metaModel);

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
        public async Task GetData_WhenExportWithDisabledExportLargeData_ReturnError()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
            metaModel.LicenseeName = "12B";

            var totalRow = FakeHelper.Create<int>();
            _appSettings.IsDisableExportLargeBetList.Returns(true);
            _appSettings.LicenseesWithLargeData.Returns(["12B"]);
            _contextService.SiteName.Returns("12B");

            // Action
            var result = (await controller.GetData(metaModel) as JsonResult).Value;
            var errorCode = result.GetValue("ErrorCode");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(errorCode, Is.EqualTo(ServerBusyErrorCode));
            });

        }

        [Test]
        public async Task GetData_WhenExportExcel_NotExistAnyDataPath_ReturnError()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);

            var totalRow = FakeHelper.Create<int>();
            var maxRowPerFile = totalRow - 1;
            _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

            var ticketResult = new GroupTicketResult
            {
                DataPath = new List<string>(),
                TotalRows = 0
            };
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile).Returns(ticketResult);

            // Action
            var result = await controller.GetData(metaModel) as StatusCodeResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));
            });

        }


        [Test]
        public async Task GetData_WhenExportExcel_RowsExceedMaxRowPerFile_NoMainTicket_ReturnError()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);

            var totalRow = FakeHelper.Create<int>();
            var maxRowPerFile = totalRow - 1;
            _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

            var ticketResult = new GroupTicketResult
            {
                DataPath = new List<string> { "123" },
                TotalRows = 0
            };
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile).Returns(ticketResult);

            // Action
            var result = await controller.GetData(metaModel) as StatusCodeResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));
            });

        }

        [Test]
        public async Task GetData_WhenExportExcel_NotInListAccessIP_ReturnError()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
            metaModel.Ip = FakeHelper.Create<string>();
            metaModel.IsShowIpvn = false;

            var totalRow = FakeHelper.Create<int>();
            var maxRowPerFile = totalRow - 1;
            _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

            var ticketResult = new GroupTicketResult
            {
                DataPath = new List<string> { "123" },
                TotalRows = 1
            };
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile).Returns(ticketResult);
            _ipService.IsVNIP(metaModel.Ip).Returns(true);

            // Action
            var result = await controller.GetData(metaModel) as StatusCodeResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));
            });

        }

        [Test]
        public async Task GetData_WhenExportExcel_ExportWithCorrectParameter()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
            var filePath = FakeHelper.Create<string>();

            var totalRow = FakeHelper.Create<int>();
            var maxRowPerFile = totalRow + 1;
            _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

            var ticketResult = new GroupTicketResult
            {
                TotalRows = totalRow
            };

            ticketResult.DataPath.Add(filePath);
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile).Returns(ticketResult);

            var data = new TicketResult { MainTicketList = new SortableList<LrfTicket> { FakeHelper.Create<LrfTicket>() } };
            var jsonData = JsonConvert.SerializeObject(data);
            _tempFileService.ReadString(Arg.Is<string>(filePath)).Returns(jsonData);

            // Action
            await controller.GetData(metaModel);

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

            var totalRow = FakeHelper.Create<int>();

            var maxRowPerFile = totalRow - 1;
            _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

            var ticketResult = new GroupTicketResult
            {
                TotalRows = totalRow,
                DataPath = new List<string> { "123" }   
            };
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile).Returns(ticketResult);

            SetUpForResponse(controller);
            _zipBuilder.BuildZip(
                ticketResult,
                Arg.Any<string>(),
                Arg.Any<AdvancedSearchBetListMetaModel>(),
                Arg.Any<string>()).Returns(x => null);

            // Action
            var result = await controller.GetData(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
            });
        }

        [Test]
        public async Task GetData_ExportZipFile_WhenHaveMutipleType_ResponsePartialContent()
        {
            // Arrange
            var controller = CreateController();
            var totalRow = FakeHelper.Create<int>();
            var maxRowPerFile = totalRow - 1;
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
            metaModel.PageSize = maxRowPerFile;

            _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);
            _appSettings.MaxRowForMultipleExcelFiles.Returns(maxRowPerFile);

            var ticketResult = new GroupTicketResult
            {
                TotalRows = totalRow,
                DataPath = new List<string> { "123" },
                CurrentPageRowCount = maxRowPerFile
            };
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile).Returns(ticketResult);

            SetUpForResponse(controller);
            _zipBuilder.BuildZip(
                ticketResult,
                Arg.Any<string>(),
                Arg.Any<AdvancedSearchBetListMetaModel>(),
                Arg.Any<string>()).Returns(x => null);

            // Action
            var result = await controller.GetData(metaModel);

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

            var totalRow = FakeHelper.Create<int>();

            var maxRowPerFile = totalRow - 1;
            _appSettings.MaxRowPerExcelFile.Returns(maxRowPerFile);

            var ticketResult = new GroupTicketResult
            {
                TotalRows = totalRow,
                DataPath = new List<string> { "123" }
            };
            _dataAccess.GetBetListForMultipleCustomersAsync(metaModel, maxRowPerFile).Returns(ticketResult);

            SetUpForResponse(controller);
            _zipBuilder.BuildZip(
                ticketResult,
                Arg.Any<string>(),
                Arg.Any<AdvancedSearchBetListMetaModel>(),
                Arg.Any<string>()).Returns(x => new MemoryStream());

            // Action
            var result = await controller.GetData(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf(typeof(FileContentResult)));
            });
        }

        [Test]
        public async Task GetExportStatus_WhenDisabledExportLargeData_ReturnServerBusy()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);
            metaModel.LicenseeName = "12B";

            var totalRow = FakeHelper.Create<int>();
            _appSettings.IsDisableExportLargeBetList.Returns(true);
            _appSettings.LicenseesWithLargeData.Returns(["12B"]);
            _contextService.SiteName.Returns("12B");

            // Action
            var result = (await controller.GetExportStatus(metaModel) as JsonResult).Value;
            var errorCode = result.GetValue("ErrorCode");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(errorCode, Is.EqualTo(ServerBusyErrorCode));
            });
        }

        [Test]
        public async Task GetExportStatus_WhenTotalRowExceedMaxTicketLimit_ReturnRowsExceedLimitErrorCode()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);

            var totalRow = FakeHelper.Create<int>();
            _appSettings.IsDisableExportLargeBetList.Returns(false);
            _appSettings.MaxTicketLimit.Returns(totalRow - 1);
            _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, _appSettings.MaxTicketLimit).Returns(totalRow);

            // Action
            var result = (await controller.GetExportStatus(metaModel) as JsonResult).Value;
            var code = result.GetValue("StatusCode");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(code, Is.EqualTo(RowsExceedLimitErrorCode));
            });
        }

        [Test]
        public async Task GetExportStatus_WhenTotalRowNotExceedMaxRowSupportSingleExportExcel_ReturnExportSingleFile()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);

            var totalRow = FakeHelper.Create<int>();
            _appSettings.IsDisableExportLargeBetList.Returns(false);
            _appSettings.MaxTicketLimit.Returns(totalRow + 1);
            _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, _appSettings.MaxTicketLimit).Returns(totalRow);
            _appSettings.MaxRowSupportSingleExportExcel.Returns(totalRow + 1);

            // Action
            var result = (await controller.GetExportStatus(metaModel) as JsonResult).Value;
            var code = result.GetValue("StatusCode");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(code, Is.EqualTo(ExportSingleFile));
            });
        }

        [Test]
        public async Task GetExportStatus__WhenTotalRowNotExceedMaxRowSupportSingleExportExcel_ReturnMaxRowForMultipleExcelFiles()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<AdvancedSearchBetListMetaModel>(x => x.IsExport, true);

            var totalRow = FakeHelper.Create<int>();
            _appSettings.IsDisableExportLargeBetList.Returns(false);
            _appSettings.MaxTicketLimit.Returns(totalRow + 1);
            _dataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, _appSettings.MaxTicketLimit).Returns(totalRow);
            _appSettings.MaxRowSupportSingleExportExcel.Returns(totalRow - 1);

            // Action
            var result = (await controller.GetExportStatus(metaModel) as JsonResult).Value;
            var code = result.GetValue("StatusCode");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(code, Is.EqualTo(ExportMultipleFile));
            });
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

            controller.When(x => x.InvokeProtectedMethod("ReturnData", Arg.Any<TicketResult>(), Arg.Any<BettingTicketCenterOption>(), Arg.Any<ICommonValueMetaModel>())).DoNotCallBase();
            controller.When(x => x.InvokeProtectedMethod("ExportExcel", Arg.Any<BettingTicketCenterExcelModel>())).DoNotCallBase();

            return controller;
        }


    }
}
