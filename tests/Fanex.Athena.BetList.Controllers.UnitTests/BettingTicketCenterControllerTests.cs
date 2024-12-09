using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Collections;
using Fanex.Athena.Resources;
using Fanex.Athena.TestHelper;
using NSubstitute;
using NUnit.Framework;
using System.Net;
using Fanex.Athena.TestHelper.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class BettingTicketCenterControllerTests : BaseControllerHelperTests
    {
        private IBettingTicketCenterDataAccess _dataAccess;
        private IIPService _ipService;
        private ITicketBuilderService _ticketBuilderService;
        private IBettingTicketCenterExcel _bettingTicketCenterExcel;
        private ICustomerClassService _customerClassService;
        private IBettingTicketCenterSportService _bettingTicketCenterSportService;

        [SetUp]
        public void FixtureSetUp()
        {
            _dataAccess = Substitute.For<IBettingTicketCenterDataAccess>();
            _ipService = Substitute.For<IIPService>();
            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
            _bettingTicketCenterExcel = Substitute.For<IBettingTicketCenterExcel>();
            _customerClassService = Substitute.For<ICustomerClassService>();
            _bettingTicketCenterSportService = Substitute.For<IBettingTicketCenterSportService>();
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _bettingTicketCenterExcel.Dispose();
            _dataAccess = null;
            _ipService = null;
            _ticketBuilderService = null;
            _bettingTicketCenterExcel = null;
            _customerClassService = null;
            _bettingTicketCenterSportService = null;
        }

        [Test]
        public async Task GetBySingleCustomer_WhenGet_GenerateDataWithCorrectOption()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<SingleCustomersBetListMetaModel>(x => x.IsExport, false);

            var ticketResult = new TicketResult();
            _dataAccess.GetBetListForSingleCustomerAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetBySingleCustomer(metaModel);

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
        public async Task GetBySingleCustomer_WhenExport_EmptyData_ReturnNoContentStatus()
        {
            var controller = CreateController();

            // Assert
            var metaModel = FakeHelper.Create<SingleCustomersBetListMetaModel>(x => x.IsExport, true);

            var ticketResult = new TicketResult();
            _dataAccess.GetBetListForSingleCustomerAsync(metaModel).Returns(ticketResult);

            // Action
            var result = await controller.GetBySingleCustomer(metaModel) as StatusCodeResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));

        }

        [Test]
        public async Task GetBySingleCustomer_WhenExport_ExportWithCorrectParameter()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<SingleCustomersBetListMetaModel>(x => x.IsExport, true);

            var ticketResult = new TicketResult
            {
                MainTicketList = new SortableList<LrfTicket>
                    {
                        FakeHelper.Create<LrfTicket>()
                    }
            };
            _dataAccess.GetBetListForSingleCustomerAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetBySingleCustomer(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ExportExcel", Arg.Is<BettingTicketCenterExcelModel>(x =>
                    x.Option.ShowTotal
                    && x.Option.ShowSumStake
                    && x.Option.ShowLicenseeWl
                    && x.Option.ShowWinLoss
                    && x.TotalText == ViewResources.Total
                    && x.Option.ShowExternalId
                    && x.Option.ShowSystemId
                    && x.Option.IsAllowedCustomerComm == metaModel.HasPlayerCom));
            });
        }

        [Test]
        public async Task GetByRefno_WhenGet_GenerateDataWithCorrectOption()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<RefNoBetListMetaModel>(x => x.IsExport, false);

            var ticketResult = new TicketResult();
            _dataAccess.GetBetListByRefnoAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByRefno(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ReturnData",
                    ticketResult,
                    Arg.Is<BettingTicketCenterOption>(x =>
                        !x.ShowTotal
                        && !x.ShowSumStake
                        && x.ShowLicenseeWl
                        && x.ShowWinLoss
                        && x.ShowExternalId
                        && x.ShowSystemId
                        && x.IsAllowedCustomerComm == metaModel.HasPlayerCom),
                    metaModel);
            });
        }

        [Test]
        public async Task GetByRefno_WhenExport_ExportWithCorrectParameter()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<RefNoBetListMetaModel>(x => x.IsExport, true);

            var ticketResult = new TicketResult
            {
                MainTicketList = new SortableList<LrfTicket>
                    {
                        FakeHelper.Create<LrfTicket>()
                    }
            };
            _dataAccess.GetBetListByRefnoAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByRefno(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ExportExcel", Arg.Is<BettingTicketCenterExcelModel>(x =>
                    !x.Option.ShowTotal
                    && !x.Option.ShowSumStake
                    && x.Option.ShowLicenseeWl
                    && x.Option.ShowWinLoss
                    && x.Option.ShowExternalId
                    && x.Option.ShowSystemId
                    && x.TotalText == null
                    && x.Option.IsAllowedCustomerComm == metaModel.HasPlayerCom));
            });

        }

        [Test]
        public async Task GetByRunning_WhenGet_GenerateDataWithCorrectOption()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<RunningBetListMetaModel>(x => x.IsExport, false);

            var ticketResult = new TicketResult();
            _dataAccess.GetRunningBetListAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByRunning(metaModel);

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
                        && x.ShowExternalId
                        && x.ShowSystemId
                        && !x.IsAllowedCustomerComm),
                    metaModel);
            });

        }

        [Test]
        public async Task GetByRunning_WhenExport_ExportWithCorrectParameter()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<RunningBetListMetaModel>(x => x.IsExport, true);

            var ticketResult = new TicketResult
            {
                MainTicketList = new SortableList<LrfTicket>
                    {
                        FakeHelper.Create<LrfTicket>()
                    }
            };
            _dataAccess.GetRunningBetListAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByRunning(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ExportExcel", Arg.Is<BettingTicketCenterExcelModel>(x =>
                    x.Option.ShowTotal
                    && x.Option.ShowSumStake
                    && !x.Option.ShowLicenseeWl
                    && !x.Option.ShowWinLoss
                    && x.Option.ShowExternalId
                    && x.Option.ShowSystemId
                    && x.TotalText == $"{ViewResources.Totalin} {metaModel.BaseCurrencyName}"
                    && !x.Option.IsAllowedCustomerComm));
            });

        }

        [Test]
        public async Task GetByTxID_WhenGet_GenerateDataWithCorrectOption()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<TxIDBetListMetaModel>(x => x.IsExport, false);

            var ticketResult = new TicketResult();
            _dataAccess.GetBetListByTxIDAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByTxID(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ReturnData",
                    ticketResult,
                    Arg.Is<BettingTicketCenterOption>(x =>
                        !x.ShowTotal
                        && !x.ShowSumStake
                        && x.ShowLicenseeWl
                        && x.ShowWinLoss
                        && x.ShowExternalId
                        && x.ShowSystemId
                        && x.IsAllowedCustomerComm == metaModel.HasPlayerCom),
                    metaModel);
            });

        }

        [Test]
        public async Task GetByTxID_WhenExport_ExportWithCorrectParameter()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<TxIDBetListMetaModel>(x => x.IsExport, true);

            var ticketResult = new TicketResult
            {
                MainTicketList = new SortableList<LrfTicket>
                    {
                        FakeHelper.Create<LrfTicket>()
                    }
            };
            _dataAccess.GetBetListByTxIDAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByTxID(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ExportExcel", Arg.Is<BettingTicketCenterExcelModel>(x =>
                    !x.Option.ShowTotal
                    && !x.Option.ShowSumStake
                    && x.Option.ShowLicenseeWl
                    && x.Option.ShowWinLoss
                    && x.Option.ShowExternalId
                    && x.Option.ShowSystemId
                    && x.TotalText == null
                    && x.Option.IsAllowedCustomerComm == metaModel.HasPlayerCom));
            });

        }

        [Test]
        public async Task GetByBookedBet_WhenGet_GenerateDataWithCorrectOption()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BookedBetBetListMetaModel>(x => x.IsExport, false);

            var ticketResult = new TicketResult();
            _dataAccess.GetBetListForBookedBetAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByBookedBet(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ReturnData",
                    ticketResult,
                    Arg.Is<BettingTicketCenterOption>(x =>
                        x.IsBookedBet
                        && x.ShowTotal
                        && x.ShowSumStake
                        && x.ShowSystemId
                        && x.ShowExternalId),
                    metaModel);
            });
        }

        [Test]
        public async Task GetByBookedBet_WhenExport_ExportWithCorrectParameter()
        {
            // Assert
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BookedBetBetListMetaModel>(x => x.IsExport, true);

            var ticketResult = new TicketResult
            {
                MainTicketList = new SortableList<LrfTicket>
                {
                    FakeHelper.Create<LrfTicket>()
                }
            };
            _dataAccess.GetBetListForBookedBetAsync(metaModel).Returns(ticketResult);

            // Action
            await controller.GetByBookedBet(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                controller.Received(1).InvokeProtectedMethod("ExportExcel", Arg.Is<BettingTicketCenterExcelModel>(x =>
                    x.Option.IsBookedBet
                    && x.Option.ShowTotal
                    && x.Option.ShowSumStake
                    && x.Option.ShowExternalId
                    && x.Option.ShowSystemId));
            });

        }

        private BettingTicketCenterController CreateController()
        {
            return Substitute.ForPartsOf<BettingTicketCenterController>(
                _dataAccess,
                _ticketBuilderService,
                _bettingTicketCenterExcel,
                _ipService,
                _customerClassService,
                _bettingTicketCenterSportService);
        }
    }
}
