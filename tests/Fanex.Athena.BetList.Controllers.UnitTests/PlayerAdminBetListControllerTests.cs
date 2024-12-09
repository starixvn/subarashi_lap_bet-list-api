using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Utility.Collections;
using Fanex.Athena.TestHelper;
using NUnit.Framework;
using System.Net;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.Resources;
using Fanex.Athena.TestHelper.Extensions;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class PlayerAdminBetListControllerTests : BaseControllerHelperTests
    {
        private IPlayerAdminBetListDataAccess _dataAccess;
        private ITicketBuilderService _ticketBuilderService;
        private IBettingTicketCenterExcel _bettingTicketCenterExcel;

        [SetUp]
        public void FixtureSetUp()
        {
            _dataAccess = Substitute.For<IPlayerAdminBetListDataAccess>();
            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
            _bettingTicketCenterExcel = Substitute.For<IBettingTicketCenterExcel>();
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _bettingTicketCenterExcel.Dispose();
            _dataAccess = null;
            _ticketBuilderService = null;
            _bettingTicketCenterExcel = null;
        }

        [Test]
        public async Task GetBetList_WhenCall_GenerateDataWithCorrectOption()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<PlayerAdminBetListMetaModel>();

            var ticketResult = new TicketResult();
            _dataAccess.GetRunningBetListAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetBetList(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ReturnData",
                    ticketResult,
                    Arg.Is<BettingTicketCenterOption>(x =>
                        x.ShowTotal
                        && x.ShowSumStake
                        && !x.ShowLicenseeWl
                        && !x.ShowWinLoss
                        && !x.IsAllowedCustomerComm),
                    metaModel);
            });
        }

        [Test]
        public async Task Export_EmptyData_ReturnNoContentStatus()
        {

            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<PlayerAdminBetListMetaModel>();
            metaModel.PageIndex = 1;
            metaModel.PageSize = int.MaxValue - 1;

            var ticketResult = new TicketResult();
            _dataAccess.GetRunningBetListAsync(metaModel).Returns(ticketResult);

            // Action
            var result = await controller.Export(metaModel) as NoContentResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));
        }

        [Test]
        public async Task Export_WhenExport_ExportWithCorrectParameter()
        {
                // Assert
                var controller = CreateController();
                var metaModel = FakeHelper.Create<PlayerAdminBetListMetaModel>();

                var ticketResult = new TicketResult
                {
                    MainTicketList = new SortableList<LrfTicket>
                    {
                        FakeHelper.Create<LrfTicket>()
                    }
                };
                _dataAccess.GetRunningBetListAsync(metaModel).Returns(ticketResult);

                // Action
                await controller.Export(metaModel);

                // Assert
                Assert.Multiple(() =>
                {
                    controller.Received(1).InvokeProtectedMethod("ExportExcel", Arg.Is<BettingTicketCenterExcelModel>(x =>
                        x.Option.ShowTotal
                        && x.Option.ShowSumStake
                        && !x.Option.ShowLicenseeWl
                        && !x.Option.ShowWinLoss
                        && x.TotalText == ViewResources.Total
                        && !x.Option.IsAllowedCustomerComm
                        && x.Title == $"{ViewResources.Currency}: {metaModel.Currency}"));
                });
        }

        private PlayerAdminBetListController CreateController()
        {
            var controller = Substitute.ForPartsOf<PlayerAdminBetListController>(
                _dataAccess,
                _ticketBuilderService,
                _bettingTicketCenterExcel);

            controller.When(x => x.InvokeProtectedMethod("ReturnData", Arg.Any<TicketResult>(), Arg.Any<BettingTicketCenterOption>(), Arg.Any<ICommonValueMetaModel>())).DoNotCallBase();
            controller.When(x => x.InvokeProtectedMethod("ExportExcel", Arg.Any<BettingTicketCenterExcelModel>())).DoNotCallBase();

            return controller;
        }
    }
}
