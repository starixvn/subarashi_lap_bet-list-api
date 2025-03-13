using AutoFixture;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using NSubstitute;

namespace Fanex.Athena.Common.Service.UnitTests
{
    public class SportServiceTests
    {
        private ISportService _sportService;
        private ICacheRefAdapter _cacheRefAdapter;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _cacheRefAdapter = Substitute.For<ICacheRefAdapter>();
            _sportService = new SportService(_cacheRefAdapter);
            _fixture = new Fixture();
        }

        [TearDown]
        public void TearDown()
        {
            _cacheRefAdapter = null;
            _sportService = null;
            _fixture = null;
        }

        [Test]
        public void GetSabaSports_ShouldReturnSabaSports()
        {
            // Arrange
            var isHideSaba = false;
            var sabaVirtualSports = SabaSportList;

            _cacheRefAdapter.GetSportName(Arg.Any<int>()).Returns("TestSportName");

            // Act
            var result = _sportService.GetSabaSports(isHideSaba);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(sabaVirtualSports.Count));

            foreach (var sabaVirtualSport in sabaVirtualSports)
            {
                var sabaSport = result.FirstOrDefault(s => s.Id == sabaVirtualSport.Id);
                if (!SportNameDic.TryGetValue(sabaVirtualSport.Id, out var sportName))
                {
                    sportName = "TestSportName";
                };
                Assert.That(sabaSport, Is.Not.Null);
                Assert.That(sabaSport.Id, Is.EqualTo(sabaVirtualSport.Id));
                Assert.That(sabaSport.LeagueGroupIds, Is.EqualTo(sabaVirtualSport.LeagueGroupIds));
                Assert.That(sabaSport.Name, Is.EqualTo($"{ViewResources.Saba}{sportName}"));
                Assert.That(sabaSport.SportTypeId, Is.EqualTo(sabaVirtualSport.SportTypeId));
            }
        }

        [Test]
        public void GetSabaSport_WithValidId_ShouldReturnSabaSport()
        {
            // Arrange
            var id = (int)SportType.SabaSoccer;
            var isHideSaba = false;
            var sabaVirtualSports = SabaSportList;

            _cacheRefAdapter.GetSportName(Arg.Any<int>()).Returns("TestSportName");

            // Act
            var result = _sportService.GetSabaSport(id, isHideSaba);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.LeagueGroupIds, Is.EqualTo(sabaVirtualSports.FirstOrDefault(s => s.Id == id).LeagueGroupIds));
            Assert.That(result.Name, Is.EqualTo($"{ViewResources.Saba}TestSportName"));
            Assert.That(result.SportTypeId, Is.EqualTo(sabaVirtualSports.FirstOrDefault(s => s.Id == id).SportTypeId));
        }

        [Test]
        public void GetSabaSport_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var id = 999;
            var isHideSaba = true;

            // Act
            var result = _sportService.GetSabaSport(id, isHideSaba);

            // Assert
            Assert.That(result, Is.Null);
        }

        private static List<(int Id, int SportTypeId, IEnumerable<int> LeagueGroupIds)> SabaSportList
            => new List<(int Id, int SportTypeId, IEnumerable<int> LeagueGroupIds)>
            {
                        ((int)SportType.SabaSoccer, (int)SportType.Soccer, new [] { 42, 74 }),
                        ((int)SportType.SabaBasketball, (int)SportType.BasketBall, new [] { 42 }),
                        ((int)SportType.SabaCricket, (int)SportType.Cricket, new [] { 42 }),
                        ((int)SportType.SabaFutsal, (int)SportType.Soccer, new [] { 108 }),
                        ((int)SportType.SabaTennis, (int)SportType.Tennis, new [] { 42 }),
                        ((int)SportType.SabaIceHockey, (int)SportType.IceHockey, new [] { 42 }),
                        ((int)SportType.SabaKabaddi, (int)SportType.Kabaddi, new [] { 42 }),
                        ((int)SportType.SabaSoccerPingoal, (int)SportType.Soccer, new [] { 113 }),
                        ((int)SportType.SabaBasketballPinGoal, (int)SportType.BasketBall, new [] { 122 }),
                        ((int)SportType.SabaESportsPinGoal, (int)SportType.Esports, new [] { 151 })
            };


        private static Dictionary<int, string> SportNameDic
            => new Dictionary<int, string>
            {
                { (int)SportType.SabaSoccerPingoal, ViewResources.SoccerPinGoal },
                { (int)SportType.SabaBasketballPinGoal,  ViewResources.BasketballPinGoal },
                { (int)SportType.SabaESportsPinGoal, ViewResources.ESportsPinGoal },
            };
    }
}
