using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.TestHelper;
using NSubstitute;
using NUnit.Framework;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.BetList.Service.Optiions;
using Fanex.Athena.Common.Constant;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class LiveReportBetListControllerTests : BaseControllerHelperTests
    {
        private const string AgentListXml = "<xml/>";

        private IBetListService _betListService;
        private ITicketBuilderService _ticketBuilderService;
        private IContextService _contextService;

        [SetUp]
        public void SetUp()
        {
            _betListService = Substitute.For<IBetListService>();
            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
            _contextService = Substitute.For<IContextService>();
        }

        [TearDown]
        public void TearDown()
        {
            _betListService = null;
            _ticketBuilderService = null;
            _contextService = null;
        }

        [Test]
        public async Task Match_WhenCall_ReturnCorrectData()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossByMatch";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();

            var ticketResult = new TicketResult
            {
                SumWinLoss = FakeHelper.Create<decimal>(),
                SumPlayerComm = FakeHelper.Create<decimal>(),
                SumLicenseeWL = FakeHelper.Create<decimal>(),
                SumComm = FakeHelper.Create<decimal>(),
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, buildTicketOption: new BuildTicketOption { IsShowIpvn = metaModel.IsShowIpvn, IsHideSaba = metaModel.IsHideSaba }).ReturnsForAnyArgs(ticketText.ToList());

            // Action
            var result = await controller.Match(metaModel);
            var main = result.GetValue("Main");
            var total = result.GetValue("Total");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.UspKey, Is.EqualTo(UspKey));
                Assert.That(main, Is.EqualTo(ticketText));
                Assert.That(total.GetValue("SumWinLoss"), Is.EqualTo(ticketResult.SumWinLoss + ticketResult.SumPlayerComm));
                Assert.That(total.GetValue("SumLicenseeWl"), Is.EqualTo(ticketResult.SumLicenseeWL + ticketResult.SumComm));
                AssertClearData(ticketResult);
            });
        }

        [Test]
        public async Task Outright_WhenCall_ReturnCorrectData()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossOutright";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();

            var ticketResult = new TicketResult
            {
                SumWinLoss = FakeHelper.Create<decimal>(),
                SumPlayerComm = FakeHelper.Create<decimal>(),
                SumLicenseeWL = FakeHelper.Create<decimal>(),
                SumComm = FakeHelper.Create<decimal>(),
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, buildTicketOption: new BuildTicketOption { IsShowIpvn = metaModel.IsShowIpvn, IsHideSaba = metaModel.IsHideSaba }).ReturnsForAnyArgs(ticketText.ToList());

            // Action
            var result = await controller.Outright(metaModel);
            var main = result.GetValue("Main");
            var total = result.GetValue("Total");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.UspKey, Is.EqualTo(UspKey));
                Assert.That(main, Is.EqualTo(ticketText));
                Assert.That(total.GetValue("SumWinLoss"), Is.EqualTo(ticketResult.SumWinLoss + ticketResult.SumPlayerComm));
                Assert.That(total.GetValue("SumLicenseeWl"), Is.EqualTo(ticketResult.SumLicenseeWL + ticketResult.SumComm));
                AssertClearData(ticketResult);
            });
        }

        [TestCase(1, 20, 50, 1, 3)]
        [TestCase(2, 30, 60, 31, 2)]
        [TestCase(3, 10, 60, 21, 6)]
        public async Task NumberGame_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int startIndex, int totalPage)
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossNumberGame";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();
            metaModel.PageIndex = pageIndex;
            metaModel.PageSize = pageSize;

            var ticketResult = new TicketResult()
            {
                TotalRows = totalRows,
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom }).Returns(ticketText.ToList());

            // Action
            var result = await controller.NumberGame(metaModel, pageIndex, pageSize);
            var page = result.GetValue("page");
            var total = result.GetValue("total");
            var records = result.GetValue("records");
            var rows = result.GetValue("rows");
            var isHasCustComm = result.GetValue("IsHasCustComm");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.UspKey, Is.EqualTo(UspKey));
                Assert.That(page, Is.EqualTo(pageIndex));
                Assert.That(total, Is.EqualTo(totalPage));
                Assert.That(records, Is.EqualTo(totalRows));
                Assert.That(rows, Is.EqualTo(ticketText));
                Assert.That(isHasCustComm, Is.EqualTo(metaModel.HasPlayerCom));
                AssertClearData(ticketResult);
            });
        }

        [TestCase(1.123, 2.345, "3.47")]
        [TestCase(1.123, 2.34, "3.46")]
        [TestCase(-1.123, -2.345, "<span class='negative-number'>-3.47</span>")]
        public async Task MixParlay_WhenCall_ReturnCorrectLicenseeWLText(decimal sumLicenseeWL, decimal sumComm, string expectedLicenseeWLText)
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossParlay";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();

            var ticketResult = new TicketResult()
            {
                SumLicenseeWL = sumLicenseeWL,
                SumComm = sumComm,
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>()).Returns(ticketText.ToList());

            var expectedWinloss = NumberHelper.DecFormat(ticketResult.SumWinLoss + ticketResult.SumPlayerComm, 2);

            // Action
            var result = await controller.MixParlay(metaModel, 1, 10);
            var userdata = result.GetValue("userdata");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.UspKey, Is.EqualTo(UspKey));
                Assert.That(userdata.GetValue("WinLoss"), Is.EqualTo(expectedWinloss));
                Assert.That(userdata.GetValue("LicenseeWL"), Is.EqualTo(expectedLicenseeWLText));
            });

        }

        [TestCase(1, 20, 50, 1, 3)]
        public async Task VirtualSports_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int startIndex, int totalPage)
        {
            // Arrange
            const string UspName = "Lrf_BetList_LiveReport_SelWinlossVHRacing";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();
            metaModel.PageIndex = pageIndex;
            metaModel.PageSize = pageSize;

            var ticketResult = new TicketResult()
            {
                TotalRows = totalRows,
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom }).Returns(ticketText.ToList());

            // Action
            var result = await controller.VirtualSports(metaModel, pageIndex, pageSize);
            var page = result.GetValue("page");
            var total = result.GetValue("total");
            var records = result.GetValue("records");
            var rows = result.GetValue("rows");
            var isHasCustComm = result.GetValue("IsHasCustComm");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.UspKey, Is.EqualTo(UspName));
                Assert.That(page, Is.EqualTo(pageIndex));
                Assert.That(total, Is.EqualTo(totalPage));
                Assert.That(records, Is.EqualTo(totalRows));
                Assert.That(rows, Is.EqualTo(ticketText));
                Assert.That(isHasCustComm, Is.EqualTo(metaModel.HasPlayerCom));
                AssertClearData(ticketResult);
            });
        }

        [TestCase(1, 20, 50, 1, 3)]
        public async Task SportsLottery_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int startIndex, int totalPage)
        {
            // Arrange
            const string UspName = "Lrf_BetList_LiveReport";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();
            metaModel.PageIndex = pageIndex;
            metaModel.PageSize = pageSize;

            var ticketResult = new TicketResult()
            {
                TotalRows = totalRows,
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom }).Returns(ticketText.ToList());

            // Action
            var result = await controller.SportsLottery(metaModel, pageIndex, pageSize);
            var page = result.GetValue("page");
            var total = result.GetValue("total");
            var records = result.GetValue("records");
            var rows = result.GetValue("rows");
            var isHasCustComm = result.GetValue("IsHasCustComm");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.ProductCodeList, Contains.Substring(ProductCodes.SLT));
                Assert.That(metaModel.UspKey, Is.EqualTo(UspName));
                Assert.That(page, Is.EqualTo(pageIndex));
                Assert.That(total, Is.EqualTo(totalPage));
                Assert.That(records, Is.EqualTo(totalRows));
                Assert.That(rows, Is.EqualTo(ticketText));
                Assert.That(isHasCustComm, Is.EqualTo(metaModel.HasPlayerCom));
                AssertClearData(ticketResult);
            });
        }

        [TestCase(1, 20, 50, 1, 3)]
        public async Task BitCoin_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int startIndex, int totalPage)
        {
            // Arrange
            const string UspName = "Lrf_BetList_LiveReport";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();
            metaModel.PageIndex = pageIndex;
            metaModel.PageSize = pageSize;

            var ticketResult = new TicketResult()
            {
                TotalRows = totalRows,
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom }).Returns(ticketText.ToList());

            // Action
            var result = await controller.BitCoin(metaModel, pageIndex, pageSize);
            var page = result.GetValue("page");
            var total = result.GetValue("total");
            var records = result.GetValue("records");
            var rows = result.GetValue("rows");
            var isHasCustComm = result.GetValue("IsHasCustComm");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.ProductCodeList, Contains.Substring(ProductCodes.BTC));
                Assert.That(metaModel.UspKey, Is.EqualTo(UspName));
                Assert.That(page, Is.EqualTo(pageIndex));
                Assert.That(total, Is.EqualTo(totalPage));
                Assert.That(records, Is.EqualTo(totalRows));
                Assert.That(rows, Is.EqualTo(ticketText));
                Assert.That(isHasCustComm, Is.EqualTo(metaModel.HasPlayerCom));
                AssertClearData(ticketResult);
            });
        }

        [TestCase(1, 20, 50, 1, 3)]
        public async Task VS3_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int startIndex, int totalPage)
        {
            // Arrange
            const string UspName = "Lrf_BetList_LiveReport";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();
            metaModel.PageIndex = pageIndex;
            metaModel.PageSize = pageSize;

            var ticketResult = new TicketResult()
            {
                TotalRows = totalRows,
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom }).Returns(ticketText.ToList());

            // Action
            var result = await controller.VS3(metaModel, pageIndex, pageSize);
            var page = result.GetValue("page");
            var total = result.GetValue("total");
            var records = result.GetValue("records");
            var rows = result.GetValue("rows");
            var isHasCustComm = result.GetValue("IsHasCustComm");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.UspKey, Is.EqualTo(UspName));
                Assert.That(page, Is.EqualTo(pageIndex));
                Assert.That(total, Is.EqualTo(totalPage));
                Assert.That(records, Is.EqualTo(totalRows));
                Assert.That(rows, Is.EqualTo(ticketText));
                Assert.That(isHasCustComm, Is.EqualTo(metaModel.HasPlayerCom));
                AssertClearData(ticketResult);
            });
        }

        [TestCase(1, 20, 50, 1, 3)]
        public async Task Betradar_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int startIndex, int totalPage)
        {
            // Arrange
            const string UspName = "Lrf_BetList_LiveReport";
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();
            metaModel.PageIndex = pageIndex;
            metaModel.PageSize = pageSize;

            var ticketResult = new TicketResult()
            {
                TotalRows = totalRows,
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, startIndex, new BuildTicketOption { IsAllowedCustomerComm = metaModel.HasPlayerCom }).Returns(ticketText.ToList());

            // Action
            var result = await controller.Betradar(metaModel, pageIndex, pageSize);
            var page = result.GetValue("page");
            var total = result.GetValue("total");
            var records = result.GetValue("records");
            var rows = result.GetValue("rows");
            var isHasCustComm = result.GetValue("IsHasCustComm");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metaModel.UspKey, Is.EqualTo(UspName));
                Assert.That(page, Is.EqualTo(pageIndex));
                Assert.That(total, Is.EqualTo(totalPage));
                Assert.That(records, Is.EqualTo(totalRows));
                Assert.That(rows, Is.EqualTo(ticketText));
                Assert.That(isHasCustComm, Is.EqualTo(metaModel.HasPlayerCom));
                AssertClearData(ticketResult);
            });
        }

        private LiveReportBetListController CreateController()
        {
            return new LiveReportBetListController(_betListService, _ticketBuilderService, _contextService);
        }
    }
}
