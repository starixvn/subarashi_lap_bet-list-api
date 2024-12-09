using Fanex.Athena.BetList.Model.Constants;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using NSubstitute;

namespace Fanex.Athena.BetList.Service.Tests
{
    public class BettingTicketCenterSportServiceTests
    {
        private ICacheRefAdapter _cacheRefAdapter;
        private BettingTicketCenterSportService _service;

        [SetUp]
        public void SetUp()
        {
            _cacheRefAdapter = Substitute.For<ICacheRefAdapter>();
            _service = CreateService();
        }

        [TearDown]
        public void TearDown()
        {
            _cacheRefAdapter = null;
            _service = null;
        }

        [Test]
        public void BuildSportTypeXML_WithEmptySportIds_ReturnsEmptyString()
        {
            // Arrange
            string sportIds = "";

            // Act
            string result = _service.BuildSportTypeXML(sportIds);

            // Assert
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void BuildSportTypeXML_WithAllSportIds_ReturnsAllString()
        {
            // Arrange
            string sportIds = "All";

            // Act
            string result = _service.BuildSportTypeXML(sportIds);

            // Assert
            Assert.That(result, Is.EqualTo("All"));
        }

        [Test]
        public void BuildSportTypeXML_WithValidSportIds_ReturnsCorrectXML()
        {
            // Arrange
            string sportIds = "1,2,3";

            // Act
            string result = _service.BuildSportTypeXML(sportIds);

            // Assert
            Assert.That(result, Is.EqualTo("<ROOT><Sport SportType=\"1\" LeagueGroupIds=\"0\"/><Sport SportType=\"2\" LeagueGroupIds=\"0\"/><Sport SportType=\"3\" LeagueGroupIds=\"0\"/></ROOT>"));
        }

        [Test]
        public void BuildSportTypeXML_WhenSportIdIsParlay_ShouldReplaceWithParlaySportType()
        {
            // Arrange
            const string sportIds = "-1";
            const string expectedXml = "<ROOT><Sport SportType=\"999\" LeagueGroupIds=\"0\"/></ROOT>";

            // Act
            var result = _service.BuildSportTypeXML(sportIds);

            // Assert
            Assert.That(result, Is.EqualTo(expectedXml));
        }

        [Test]
        public void BuildSportTypeXML_WhenSportIdIsSabaVirtuaalSports_ShouldReplaceWithSabaVirtualSports()
        {
            // Arrange
            var sportIds = $"{(int)BettingTicketCenterSportType.SabaSoccer}";
            const string expectedXml = "<ROOT><Sport SportType=\"1\" LeagueGroupIds=\"42,74\"/></ROOT>";

            // Act
            var result = _service.BuildSportTypeXML(sportIds);

            // Assert
            Assert.That(result, Is.EqualTo(expectedXml));
        }

        [Test]
        public void GetSabaSports_WhenCall_ShouldReturnCorrectResult()
        {
            // Arrange
            var expected = new SabaSport { Id = (int)BettingTicketCenterSportType.SabaSoccer, LeagueGroupIds = new[] { 42, 74 } };

            // Act
            var result = _service.GetSabaSports().First();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(expected.Id));
                Assert.That(result.LeagueGroupIds, Is.EquivalentTo(expected.LeagueGroupIds));
                Assert.That(result.SportTypeId, Is.EqualTo((int)SportType.Soccer));
            });
        }

        [Test]
        public void GetSabaSport_WhenCallWithHideSaba_ShouldReturnEmptyResult()
        {
            // Arrange
            var expected = new SabaSport { Id = (int)BettingTicketCenterSportType.SabaSoccer, LeagueGroupIds = new[] { 42, 74 } };

            // Act
            var result = _service.GetSabaSport((int)BettingTicketCenterSportType.SabaSoccer);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(expected.Id));
                Assert.That(result.LeagueGroupIds, Is.EquivalentTo(expected.LeagueGroupIds));
                Assert.That(result.SportTypeId, Is.EqualTo((int)SportType.Soccer));
            });
        }

        private BettingTicketCenterSportService CreateService()
        {
            return new BettingTicketCenterSportService(_cacheRefAdapter);
        }
    }
}
