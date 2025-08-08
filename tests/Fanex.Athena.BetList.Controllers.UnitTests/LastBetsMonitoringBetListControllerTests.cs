namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    using Common.Services.Interfaces;
    using Common.Utility.Collections;
    using Fanex.BetList.Core.Entities;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using NUnit.Framework;
    using Service.Interfaces;
    using TestHelper;

    public class LastBetsMonitoringBetListControllerTests : BaseControllerHelperTests
    {
        private ILastBetsMonitoringBetListService _lastBetsMonitoringBetListService;
        private ITicketHelper _ticketHelper;
        private IIPService _ipService;
        private ICustomerClassService _customerClassService;
        private IContextService _contextService;
        private ISportService _sportService;

        [SetUp]
        public void FixtureSetUp()
        {
            _lastBetsMonitoringBetListService = Substitute.For<ILastBetsMonitoringBetListService>();
            _ticketHelper = Substitute.For<ITicketHelper>();
            _ipService = Substitute.For<IIPService>();
            _customerClassService = Substitute.For<ICustomerClassService>();
            _contextService = Substitute.For<IContextService>();
            _sportService = Substitute.For<ISportService>();
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _lastBetsMonitoringBetListService = null;
            _ticketHelper = null;
            _ipService = null;
            _customerClassService = null;
            _contextService = null;
        }

        [Test]
        public async Task GetData_NoData_ReturnNoNewData()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LastBetsMonitoringMetaModel>();
            metaModel.SportIds = string.Empty;
            metaModel.Currency = string.Empty;

            _lastBetsMonitoringBetListService.GetLastBetsMonitoringData(metaModel).Returns(Task.FromResult(new TicketResult()));

            // Action
            var result = await controller.GetData(metaModel);

            // Assert
            Assert.That(result.GetValue("NewRow"), Is.False);

        }

        [Test]
        public async Task GetData_HasData_ReturnCorrectData()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LastBetsMonitoringMetaModel>();
            metaModel.SportIds = string.Empty;
            metaModel.Currency = string.Empty;

            var ticket = FakeHelper.Create<LrfTicket>();
            ticket.BetTypeId = 1;
            ticket.OddsType = "1";
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _lastBetsMonitoringBetListService.GetLastBetsMonitoringData(metaModel).Returns(Task.FromResult(ticketResult));

            // Action
            var result = await controller.GetData(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                AssertClearData(ticketResult);
                Assert.That(result.GetValue("NewRow"), Is.True);
                Assert.That(result.GetValue("Rows"), Is.Not.Null);
                Assert.That(result.GetValue("LastTransId"), Is.EqualTo(ticket.TransId.ToString()));
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(20)]
        public async Task GetData_BuildOdds_DecimalOddsBetType_BuildCorrectOddString(int betType)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LastBetsMonitoringMetaModel>();
            metaModel.SportIds = string.Empty;
            metaModel.Currency = string.Empty;

            var ticket = FakeHelper.Create<LrfTicket>();
            ticket.BetTypeId = betType;
            ticket.OddsType = "1";
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _lastBetsMonitoringBetListService.GetLastBetsMonitoringData(metaModel).Returns(Task.FromResult(ticketResult));

            // Action
            var result = await controller.GetData(metaModel);
            var data = (string)result.GetValue("Rows");

            // Assert
            Assert.That(data.Contains("underdog"), Is.True);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(12)]
        [TestCase(20)]
        public async Task GetData_BuildOdds_NegativeDecimalOddsBetType_BuildCorrectOddString(int betType)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<LastBetsMonitoringMetaModel>();
            metaModel.SportIds = string.Empty;
            metaModel.Currency = string.Empty;

            var ticket = FakeHelper.Create<LrfTicket>();
            ticket.OddsType = "1";
            ticket.BetTypeId = betType;
            ticket.Odds = -FakeHelper.Create<uint>();
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _lastBetsMonitoringBetListService.GetLastBetsMonitoringData(metaModel).Returns(Task.FromResult(ticketResult));

            // Action
            var result = await controller.GetData(metaModel);
            var data = (string)result.GetValue("Rows");

            // Assert
            Assert.That(data.Contains("underdog negative"), Is.True);
        }

        private LastBetMonitoringBetListController CreateController()
        {
            return new LastBetMonitoringBetListController(
                 _lastBetsMonitoringBetListService,
                 _ticketHelper,
                 _ipService,
                 _customerClassService,
                 _contextService,
                 _sportService);
        }
    }
}
