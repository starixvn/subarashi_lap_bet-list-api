using Fanex.Athena.Common.Utility.Services;
using Fanex.RemotingRef;
using NSubstitute;

namespace Fanex.Athena.Common.Utility.Tests.Services
{
    public class CacheRefAdapterTests
    {
        private IRemotingRefService remotingRefServiceMock;
        private CacheRefAdapter cacheRefAdapter;

        [SetUp]
        public void SetUp()
        {
            remotingRefServiceMock = Substitute.For<IRemotingRefService>();
            cacheRefAdapter = new CacheRefAdapter(remotingRefServiceMock);
        }

        [TearDown]
        public void TearDown()
        {
            remotingRefServiceMock = null;
        }

        [Test]
        public void GetSportName_WithValidSportId_ReturnsSportName()
        {
            // Arrange
            object sportId = 1;
            string expectedSportName = "Football";
            remotingRefServiceMock.GetSportName(sportId.ToString(), Arg.Any<string>(), string.Empty)
                .Returns(expectedSportName);

            // Act
            string result = cacheRefAdapter.GetSportName(sportId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedSportName));
        }

        [Test]
        public void GetSportName_WithInvalidSportId_ReturnsEmptyString()
        {
            // Arrange
            object sportId = null;

            // Act
            string result = cacheRefAdapter.GetSportName(sportId);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetLeagueName_WithValidLeagueId_ReturnsLeagueName()
        {
            // Arrange
            object leagueId = 1;
            string expectedLeagueName = "Premier League";
            remotingRefServiceMock.GetLeagueName(leagueId.ToString(), Arg.Any<string>(), string.Empty)
                .Returns(expectedLeagueName);

            // Act
            string result = cacheRefAdapter.GetLeagueName(leagueId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedLeagueName));
        }

        [Test]
        public void GetLeagueName_WithInvalidLeagueId_ReturnsEmptyString()
        {
            // Arrange
            object leagueId = null;

            // Act
            string result = cacheRefAdapter.GetLeagueName(leagueId);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetLeagueName3rd_WithValidLeagueId_ReturnsLeagueName()
        {
            // Arrange
            object leagueId = 1;
            string expectedLeagueName = "La Liga";
            remotingRefServiceMock.GetLeagueName3rd(leagueId.ToString(), Arg.Any<string>(), string.Empty)
                .Returns(expectedLeagueName);

            // Act
            string result = cacheRefAdapter.GetLeagueName3rd(leagueId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedLeagueName));
        }

        [Test]
        public void GetLeagueName3rd_WithInvalidLeagueId_ReturnsEmptyString()
        {
            // Arrange
            object leagueId = null;

            // Act
            string result = cacheRefAdapter.GetLeagueName3rd(leagueId);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetCombinationTypeCombo_ReturnsCombinationTypeCombo()
        {
            // Arrange
            string betTeam = "Team A";
            string betCheck = "Check";
            decimal handicap1 = 1.0m;
            decimal handicap2 = 2.0m;
            string[] expectedCombo = { "Combo1", "Combo2" };
            remotingRefServiceMock.GetCombinationTypeCombo(betTeam, betCheck, 1, 2, Arg.Any<string>())
                .Returns(expectedCombo);

            // Act
            string[] result = cacheRefAdapter.GetCombinationTypeCombo(betTeam, betCheck, handicap1, handicap2);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCombo));
        }

        [Test]
        public void GetBetBuilderSelectionNames_ReturnsBetBuilderSelectionNames()
        {
            // Arrange
            int[] selectionIds = { 1, 2, 3 };
            string[] expectedSelectionNames = { "Selection1", "Selection2", "Selection3" };
            remotingRefServiceMock.GetBetBuilderSelectionNames(selectionIds, Arg.Any<string>())
                .Returns(expectedSelectionNames);

            // Act
            string[] result = cacheRefAdapter.GetBetBuilderSelectionNames(selectionIds);

            // Assert
            Assert.That(result, Is.EqualTo(expectedSelectionNames));
        }

    }
}
