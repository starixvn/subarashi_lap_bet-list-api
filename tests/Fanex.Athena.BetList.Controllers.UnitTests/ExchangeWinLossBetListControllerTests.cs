using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.TestHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class ExchangeWinLossBetListControllerTests
    {
        private IExchangeBetListService _service;
        private IExchangeRunningExcel _exchangeRunningExcel;
        private IExchangeWinLossExcel _exchangeWinLossExcel;
        private IWebHostEnvironment _webHostEnvironment;
        private IAuthorizationService _authorizationService;
        private IContextService _contextService;

        [SetUp]
        public void SetUp()
        {
            _service = Substitute.For<IExchangeBetListService>();
            _exchangeRunningExcel = Substitute.For<IExchangeRunningExcel>();
            _exchangeWinLossExcel = Substitute.For<IExchangeWinLossExcel>();
            _webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            _authorizationService = Substitute.For<IAuthorizationService>();
            _contextService = Substitute.For<IContextService>();
        }

        [TearDown]
        public void TearDown()
        {
            _service = null;
            _exchangeRunningExcel = null;
            _exchangeWinLossExcel = null;
            _webHostEnvironment = null;
            _authorizationService = null;
            _contextService = null;
        }

        [Test]
        public async Task GetData_WhenNotValidRequest_ReturnNoMainData()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ExchangeBetListMetaModel>();
            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(false);

            // Action
            var jsonResult = await controller.GetData(metaModel) as JsonResult;
            var result = jsonResult.Value as ExchangeBetListViewModel;

            // Assert
            Assert.That(result.MainData, Is.Null);

        }

        [Test]
        public async Task GetData_WhenValidRequest_ReturnCorrectResult()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ExchangeBetListMetaModel>();
            metaModel.IsExportExcel = false;
            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);
            var expectedResult = FakeHelper.Create<ExchangeBetListViewModel>();
            _service.GetBetListDataAsync(metaModel).Returns(expectedResult);
            
            // Action
            var jsonResult = await controller.GetData(metaModel) as JsonResult;
            var result = jsonResult.Value as ExchangeBetListViewModel;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));

        }

        [Test]
        public async Task GetData_WhenExportExcel_ReturnCorrectResult()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ExchangeBetListMetaModel>();
            metaModel.IsExportExcel = true;
            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);
            byte[] expectedResult = [1, 2, 3];
            _exchangeWinLossExcel.ExportExcel(default, default, default).ReturnsForAnyArgs(expectedResult);

            // Action
            var result = await controller.GetData(metaModel) as FileContentResult;

            // Assert
            Assert.That(result.FileContents, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task Detail_WhenNotValidRequest_ReturnEmptyList()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ExchangeBetListDetailMetaModel>();
            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(false);

            // Action
            var result = await controller.Detail(metaModel);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task Detail_WhenValidRequest_ReturnCorrectResult()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ExchangeBetListDetailMetaModel>();
            _authorizationService.HasPermissionOnData(default, default, default).ReturnsForAnyArgs(true);
            var expectedResult = FakeHelper.CreateMany<ExchangeTicketDetail>();
            _service.GetTicketDetailListAsync(metaModel).Returns(expectedResult);

            // Action
            var result = await controller.Detail(metaModel);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GetRunningTickets_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ExchangeRunningTicketMetaModel>();
            var expectedResult = FakeHelper.Create<ExchangeResult<ExchangeRunningTicket>>();
            _service.GetRunningTicketsAsync(metaModel).Returns(expectedResult);

            // Action
            var result = await controller.GetRunningTickets(metaModel);

            // Assert
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task ExportRunningTickets_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ExchangeRunningTicketMetaModel>();
            byte[] expectedResult = [1, 2, 3];
            _exchangeRunningExcel.ExportRunningExcel(default, default, default).ReturnsForAnyArgs(expectedResult);

            // Action
            var result = await controller.ExportRunningTickets(metaModel);

            // Assert
            Assert.That(result.FileContents, Is.EqualTo(expectedResult));
        }

        private ExchangeWinLossBetListController CreateController()
            => new ExchangeWinLossBetListController(_service, _exchangeRunningExcel, _exchangeWinLossExcel, _webHostEnvironment, _authorizationService, _contextService);

    }
}
