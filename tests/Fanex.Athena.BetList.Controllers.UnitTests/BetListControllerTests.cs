namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    using Common.DataAccess.Interfaces;
    using Common.Services.Interfaces;
    using Common.Utility.Collections;
    using Common.Utility.Helpers;
    using Common.Utility.Services.Interfaces;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using NUnit.Framework;
    using Service.Interfaces;
    using Service.Optiions;
    using TestHelper;

    public class BetListControllerTests : BaseControllerHelperTests
    {
        private IContextService _contextService;
        private ICacheRefAdapter _cacheRefAdapter;
        private IBetListService _betListService;
        private ITicketBuilderService _ticketBuilderService;
        private IIPService _ipService;
        private IBalanceUpDataAccess _balanceUpDataAccess;
        private IAuthorizationService _authorizationService;
        private ISubsidiaryService _subsidiaryService; 
        private IAppSettings _appSettings;

        [SetUp]
        public void SetUp()
        {
            _contextService = Substitute.For<IContextService>();
            _cacheRefAdapter = Substitute.For<ICacheRefAdapter>();
            _betListService = Substitute.For<IBetListService>();
            _ticketBuilderService = Substitute.For<ITicketBuilderService>();
            _ipService = Substitute.For<IIPService>();
            _balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            _authorizationService = Substitute.For<IAuthorizationService>();
            _subsidiaryService = Substitute.For<ISubsidiaryService>();
            _appSettings = Substitute.For<IAppSettings>();

        }

        [TearDown]
        public void TearDown()
        {
            _contextService = null;
            _cacheRefAdapter = null;
            _betListService = null;
            _ticketBuilderService = null;
            _ipService = null;
            _balanceUpDataAccess = null;
            _authorizationService = null;
            _subsidiaryService = null;
            _appSettings = null;
        }

        [Test]
        public void Index_WhenCall_ReturnCorrectString()
        {
            var controller = CreateController();

            // Action
            var result = controller.Index();

            // Assert
            Assert.That(result, Is.EqualTo("Betlist Api default page"));

        }

        [Test]
        public async Task GetBetList_NoMainTicket_ReturnLeagueNameIsEmpty()
        {
            var controller = CreateController();

            // Arrange
            var metaModel = FakeHelper.Create<BetListMetaModel>();
            metaModel.IsMobileView = false;

            var ticketResult = new TicketResult
            {
                SumWinLoss = FakeHelper.Create<decimal>(),
                SumPlayerComm = FakeHelper.Create<decimal>(),
                SumLicenseeWL = FakeHelper.Create<decimal>(),
                SumComm = FakeHelper.Create<decimal>(),
                SumBetEffective = FakeHelper.Create<decimal>()
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2).ToList();
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText);

            // Action
            var result = (await controller.GetBetList(metaModel));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Main, Is.EqualTo(ticketText));
                Assert.That(result.Total.SumWinLoss, Is.EqualTo(ticketResult.SumWinLoss + ticketResult.SumPlayerComm));
                Assert.That(result.Total.SumLicenseeWl, Is.EqualTo(ticketResult.SumLicenseeWL + ticketResult.SumComm));
                Assert.That(result.Total.SumBetEffective, Is.EqualTo(ticketResult.SumBetEffective));
                Assert.That(result.Total.SumStake, Is.Zero);
                Assert.That(result.LeagueName, Is.Empty);

                AssertClearData(ticketResult);
            });

        }

        [Test]
        public async Task GetBetList_HasMainTicket_ReturnCorrectLeagueName()
        {
            var controller = CreateController();

            // Arrange
            var metaModel = FakeHelper.Create<BetListMetaModel>();

            var ticket = FakeHelper.Create<LrfTicket>();
            var ticketResult = new TicketResult
            {
                SumWinLoss = FakeHelper.Create<decimal>(),
                SumPlayerComm = FakeHelper.Create<decimal>(),
                SumLicenseeWL = FakeHelper.Create<decimal>(),
                SumComm = FakeHelper.Create<decimal>(),
                SumBetEffective = FakeHelper.Create<decimal>(),

                MainTicketList = new SortableList<LrfTicket> { ticket }
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2).ToList();
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText);

            var leagueName = FakeHelper.Create<string>();
            _betListService.GetLeagueName(ticket.LeagueId).Returns(leagueName);

            // Action
            var result = await controller.GetBetList(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Main, Is.EqualTo(ticketText));
                Assert.That(result.Total.SumWinLoss, Is.EqualTo(ticketResult.SumWinLoss + ticketResult.SumPlayerComm));
                Assert.That(result.Total.SumLicenseeWl, Is.EqualTo(ticketResult.SumLicenseeWL + ticketResult.SumComm));
                Assert.That(result.Total.SumBetEffective, Is.EqualTo(ticketResult.SumBetEffective));
                Assert.That(result.Total.SumStake, Is.EqualTo(Math.Round(ticket.Stake, 2)));
                Assert.That(result.LeagueName, Is.EqualTo(leagueName));

                AssertClearData(ticketResult);
            });

        }

        [TestCase(1, 20, 50, 3)]
        [TestCase(2, 30, 60, 2)]
        [TestCase(3, 10, 60, 6)]
        public async Task GetPagingData_WhenCall_ReturnCorrectData(int pageIndex, int pageSize, int totalRows, int totalPage)
        {
            var controller = CreateController();

            // Arrange
            var metaModel = FakeHelper.Create<BetListMetaModel>();
            metaModel.PageIndex = pageIndex;
            metaModel.PageSize = pageSize;

            var ticketResult = new TicketResult { TotalRows = totalRows };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>()).Returns(ticketText.ToList());

            // Action
            var result = await controller.GetBetListPaging(metaModel);
            var page = result.GetValue("page");
            var total = result.GetValue("total");
            var records = result.GetValue("records");
            var rows = result.GetValue("rows");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(page, Is.EqualTo(pageIndex));
                Assert.That(total, Is.EqualTo(totalPage));
                Assert.That(records, Is.EqualTo(totalRows));
                Assert.That(rows, Is.EqualTo(ticketText));
                AssertClearData(ticketResult);
            });
        }

        [TestCase(1.123, 2.345, "3.47")]
        [TestCase(1.123, 2.34, "3.46")]
        [TestCase(-1.123, -2.345, "<span class='negative-number'>-3.47</span>")]
        public async Task GetPagingData_WhenCall_ReturnCorrectLicenseeWLText(decimal sumLicenseeWL, decimal sumComm, string expectedLicenseeWLText)
        {
            var controller = CreateController();

            // Arrange
            var metaModel = FakeHelper.Create<BetListMetaModel>();

            var ticketResult = new TicketResult
            {
                SumLicenseeWL = sumLicenseeWL,
                SumComm = sumComm,
                SumWinLoss = FakeHelper.Create<decimal>(),
                SumPlayerComm = FakeHelper.Create<decimal>()
            };
            _betListService.GetBetListData(metaModel).Returns(ticketResult);

            var ticketText = FakeHelper.CreateMany<TicketText>(2);
            _ticketBuilderService.BuildTicketText(ticketResult, metaModel, Arg.Any<int>(), Arg.Any<BuildTicketOption>())
                .Returns(ticketText.ToList());

            var expectedWinloss = NumberHelper.DecFormat(ticketResult.SumWinLoss + ticketResult.SumPlayerComm);

            // Action
            var result = await controller.GetBetListPaging(metaModel);
            var userdata = result.GetValue("userdata");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(userdata.GetValue("WinLoss"), Is.EqualTo(expectedWinloss));
                Assert.That(userdata.GetValue("LicenseeWL"), Is.EqualTo(expectedLicenseeWLText));
            });

        }


        private BetListController CreateController()
        {
            return Substitute.ForPartsOf<BetListController>( _contextService, 
                _cacheRefAdapter, 
                _betListService, 
                _ticketBuilderService, 
                _ipService,
                _balanceUpDataAccess,
                _authorizationService,
                _subsidiaryService,
                _appSettings);
        }
    }
}
