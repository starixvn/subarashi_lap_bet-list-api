namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    using Common.Utility.Collections;
    using Common.Utility.Services.Interfaces;
    using Fanex.BetList.Core.Builder.HybridParlayBuilder;
    using Fanex.BetList.Core.Entities;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using NUnit.Framework;
    using Service.Interfaces;
    using TestHelper;

    public class BetBuilderPlusBetListControllerTests : BaseControllerHelperTests
    {
        private IBetListService _betListService;
        private ITicketHelper _ticketHelper;
        private IHybridParlayBuilder _hybridParlayBuilder;
        private IIPService _ipService;

        [SetUp]
        public void SetUp()
        {
            _betListService = Substitute.For<IBetListService>();
            _ticketHelper = Substitute.For<ITicketHelper>();
            _hybridParlayBuilder = Substitute.For<IHybridParlayBuilder>();
            _ipService = Substitute.For<IIPService>();
        }

        [TearDown]
        public void TearDown()
        {
            _betListService = null;
            _ticketHelper = null;
            _hybridParlayBuilder = null;
            _ipService = null;
        }

        [Test]
        public async Task Detail_WhenCall_ReturnCorrectData()
        {
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetBuilderPlusDetailMetaModel>();

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _betListService.GetMixParlayDetailTicket(metaModel).Returns(ticketResult);

            var expectedResult = FakeHelper.Create<string>();
            _hybridParlayBuilder.Build(
                _ticketHelper,
                Arg.Any<List<ITicket>>(),
                metaModel.TransDesc,
                (int)metaModel.Handicap1,
                (int)metaModel.Handicap2,
                metaModel.BetCheck,
                metaModel.BetTeam,
                metaModel.Currency,
                ticket.Stake,
                false).Returns(expectedResult);

            _ipService.IsAlwaysIpvn(Arg.Any<string>()).Returns(false);
            _ipService.IsVNIP(Arg.Any<string>()).Returns(false);

            var result = await controller.Detail(metaModel);
            var data = result.GetValue("Data");

            Assert.Multiple(() =>
            {
                Assert.That(data, Is.EqualTo(expectedResult));
                AssertClearData(ticketResult);
            });
        }

        [Test]
        public async Task Detail_WhenCall_SetTicketHelperCorrectly()
        {
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetBuilderPlusDetailMetaModel>();
            metaModel.IsShowIp = true;
            metaModel.IsHideSaba = true;

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _betListService.GetMixParlayDetailTicket(metaModel).Returns(ticketResult);

            var expectedResult = FakeHelper.Create<string>();
            _hybridParlayBuilder.Build(
                Arg.Any<ITicketHelper>(),
                Arg.Any<List<ITicket>>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<bool>()).Returns(expectedResult);

            _ipService.IsAlwaysIpvn(Arg.Any<string>()).Returns(false);
            _ipService.IsVNIP(Arg.Any<string>()).Returns(true);

            await controller.Detail(metaModel);

            var testIp = FakeHelper.Create<string>();
            Assert.Multiple(() =>
            {
                Assert.That(_ticketHelper.IsShowOddsWithChoice, Is.True);
                Assert.That(_ticketHelper.HideSabaText, Is.True);
                Assert.That(_ticketHelper.IsVnIp(testIp), Is.True);
            });
        }

        [Test]
        public async Task Detail_IsShowIpFalse_UseAlwaysIpvn()
        {
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetBuilderPlusDetailMetaModel>();
            metaModel.IsShowIp = false;

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _betListService.GetMixParlayDetailTicket(metaModel).Returns(ticketResult);

            var expectedResult = FakeHelper.Create<string>();
            _hybridParlayBuilder.Build(
                Arg.Any<ITicketHelper>(),
                Arg.Any<List<ITicket>>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<bool>()).Returns(expectedResult);

            _ipService.IsAlwaysIpvn(Arg.Any<string>()).Returns(true);
            _ipService.IsVNIP(Arg.Any<string>()).Returns(false);

            await controller.Detail(metaModel);

            var testIp = FakeHelper.Create<string>();
            Assert.That(_ticketHelper.IsVnIp(testIp), Is.True);
        }

        [Test]
        public async Task Detail_IsShowIpTrue_UseIsVNIP()
        {
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetBuilderPlusDetailMetaModel>();
            metaModel.IsShowIp = true;

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _betListService.GetMixParlayDetailTicket(metaModel).Returns(ticketResult);

            var expectedResult = FakeHelper.Create<string>();
            _hybridParlayBuilder.Build(
                Arg.Any<ITicketHelper>(),
                Arg.Any<List<ITicket>>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<bool>()).Returns(expectedResult);

            _ipService.IsAlwaysIpvn(Arg.Any<string>()).Returns(false);
            _ipService.IsVNIP(Arg.Any<string>()).Returns(true);

            await controller.Detail(metaModel);

            var testIp = FakeHelper.Create<string>();
            Assert.That(_ticketHelper.IsVnIp(testIp), Is.True);
        }

        [Test]
        public async Task Detail_NoTickets_ReturnEmptyData()
        {
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetBuilderPlusDetailMetaModel>();

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _betListService.GetMixParlayDetailTicket(metaModel).Returns(ticketResult);

            var expectedResult = FakeHelper.Create<string>();
            _hybridParlayBuilder.Build(
                _ticketHelper,
                Arg.Any<List<ITicket>>(),
                metaModel.TransDesc,
                (int)metaModel.Handicap1,
                (int)metaModel.Handicap2,
                metaModel.BetCheck,
                metaModel.BetTeam,
                metaModel.Currency,
                ticket.Stake,
                false).Returns(expectedResult);

            _ipService.IsAlwaysIpvn(Arg.Any<string>()).Returns(false);

            var result = await controller.Detail(metaModel);
            var data = result.GetValue("Data");

            Assert.Multiple(() =>
            {
                Assert.That(data, Is.EqualTo(expectedResult));
                AssertClearData(ticketResult);
            });
        }

        [Test]
        public async Task Detail_WhenCall_CallHybridParlayBuilderWithCorrectParameters()
        {
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetBuilderPlusDetailMetaModel>();
            metaModel.TransDesc = "TestTransDesc";
            metaModel.Handicap1 = 1;
            metaModel.Handicap2 = 2;
            metaModel.BetCheck = "TestBetCheck";
            metaModel.BetTeam = "TestBetTeam";
            metaModel.Currency = "USD";

            var ticket = FakeHelper.Create<LrfTicket>();
            ticket.Stake = 100.5m;
            var ticketResult = new TicketResult { MainTicketList = new SortableList<LrfTicket> { ticket } };
            _betListService.GetMixParlayDetailTicket(metaModel).Returns(ticketResult);

            var expectedResult = FakeHelper.Create<string>();
            _hybridParlayBuilder.Build(
                _ticketHelper,
                Arg.Any<List<ITicket>>(),
                metaModel.TransDesc,
                1,
                2,
                metaModel.BetCheck,
                metaModel.BetTeam,
                metaModel.Currency,
                ticket.Stake,
                false).Returns(expectedResult);

            _ipService.IsAlwaysIpvn(Arg.Any<string>()).Returns(false);

            await controller.Detail(metaModel);

            _hybridParlayBuilder.Received(1).Build(
                _ticketHelper,
                Arg.Any<List<ITicket>>(),
                metaModel.TransDesc,
                1,
                2,
                metaModel.BetCheck,
                metaModel.BetTeam,
                metaModel.Currency,
                ticket.Stake,
                false);
        }

        [Test]
        public async Task Detail_WhenCall_ClearDataAfterOperation()
        {
            var controller = CreateController();
            var metaModel = FakeHelper.Create<BetBuilderPlusDetailMetaModel>();

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult
            {
                MainTicketList = new SortableList<LrfTicket> { ticket },
                TicketDataList = new List<ITicketData>()
            };
            _betListService.GetMixParlayDetailTicket(metaModel).Returns(ticketResult);

            var expectedResult = FakeHelper.Create<string>();
            _hybridParlayBuilder.Build(
                Arg.Any<ITicketHelper>(),
                Arg.Any<List<ITicket>>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<bool>()).Returns(expectedResult);

            _ipService.IsAlwaysIpvn(Arg.Any<string>()).Returns(false);

            await controller.Detail(metaModel);

            AssertClearData(ticketResult);
        }

        private BetBuilderPlusBetListController CreateController()
        {
            return new BetBuilderPlusBetListController(
                _betListService,
                _ticketHelper,
                _hybridParlayBuilder,
                _ipService);
        }
    }
}

