namespace Fanex.Athena.BetList.Service.Tests
{
    using Common.DataAccess.Interfaces;
    using Common.Model.DTO;
    using Common.Services.Interfaces;
    using Common.Utility.Services.Interfaces;
    using DataAccess.Interfaces;
    using Fanex.BetList.Core.Entities;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;

    public class BetListServiceTests
    {
        private IBetListDataAccess _betlistDataAccess;

        private IBetListService _betListService;

        private ITicketHelper _ticketHelper;

        private ICacheRefAdapter _cacheRefAdapter;
        private IShardInfoAccessor _shardInfoAccessor;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            _betlistDataAccess = Substitute.For<IBetListDataAccess>();
            _ticketHelper = Substitute.For<ITicketHelper>();
            _cacheRefAdapter = Substitute.For<ICacheRefAdapter>();
            _shardInfoAccessor = Substitute.For<IShardInfoAccessor>();

            _betListService = new BetListService(balanceUpDataAccess, _betlistDataAccess, _shardInfoAccessor, _cacheRefAdapter);

            balanceUpDataAccess.GetBalanceUp().Returns(new BalanceUp());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _betlistDataAccess = null;
            _betListService = null;
            _shardInfoAccessor = null;
            _cacheRefAdapter = null;
            _ticketHelper = null;
        }

        [TearDown]
        public void TearDown()
        {
            _ticketHelper.ClientDetailSystemParlayFunction = string.Empty;
        }

        [Test]
        public async Task GetMixParlayDetailTicket_WhenCall_ReturnDataFromDataAccess()
        {
            // Arrange
            var expectedResult = new TicketResult();
            var metaModel = new MixParlayDetailMetaModel();
            _betlistDataAccess.GetMixParlayDetailTicket(metaModel).Returns(expectedResult);

            // Action
            var result = await _betListService.GetMixParlayDetailTicket(metaModel);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GetBetListData_WhenCall_ReturnDataFromDataAccess()
        {
            // Arrange
            var expectedResult = new TicketResult
            {
                TotalRows = 1234
            };

            var metaModel = new BetListMetaModel();
            _betlistDataAccess.GetBetListData(metaModel).Returns(expectedResult);

            // Action
            var result = await _betListService.GetBetListData(metaModel);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GetLeagueName_WhenCall_ReturnDataFromCacheRefAdapter()
        {
            // Arrange
            var leagueId = FakeHelper.Create<int>();

            // Action
            _betListService.GetLeagueName(leagueId);

            // Assert
            Assert.Multiple(() =>
            {
                _cacheRefAdapter.Received().GetLeagueName(leagueId);
            });
        }
    }
}
