using Fanex.Athena.TestHelper.Extensions;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.Sportsbook.DTO;
using NSubstitute;

namespace Fanex.Athena.Result.DataAccess.Tests
{
    public partial class ResultDataAccessTests
    {
        [Test]
        public async Task GetPopUpSingleMatchParlay_WhenCall_ParameterPassedCorrectly()
        {
            // Arrange
            var matchId = FakeHelper.Create<int>();
            var marketId = FakeHelper.Create<string>();
            Reader.Read().Returns(false);

            // Action
            await DataAccess.GetPopUpSingleMatchParlayAsync(matchId, marketId);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("matchid", matchId);
                ObjectDb.Received(1).SetParameterValue("marketId", marketId);
            });
        }

        [Test]
        public async Task GetPopUpSingleMatchParlay_WhenCall_ReturnCorrectMainData()
        {
            // Arrange
            var matchId = FakeHelper.Create<int>();
            var marketId = FakeHelper.Create<string>();

            Reader.Read().Returns(true, false);
            Reader.NextResult().Returns(false);
            SetUpForReader();

            var dataMapping = GetReaderDataMappingForSportsbookType();
            MapReaderData(dataMapping);

            var sportsbookData = GetExpectedSportsbookData(new List<GameDetail>());

            // Action
            var result = await DataAccess.GetPopUpSingleMatchParlayAsync(matchId, marketId);

            // Assert
            Assert.That(result, Tobe.SameProperties(sportsbookData));
        }

        [Test]
        public async Task GetPopUpSingleMatchParlay_WhenCall_ReturnCorrectBetBuilderDetailsData()
        {
            // Arrange
            var matchId = FakeHelper.Create<int>();
            var marketId = FakeHelper.Create<string>();

            Reader.Read().Returns(true, true, false);
            Reader.NextResult().Returns(true);
            Reader.FieldCount.Returns(0);

            var expectedDetail = new Dictionary<string, object>
            {
                { "SelectionType", FakeHelper.Create<int>() }, { "Result", FakeHelper.Create<string>() }
            };
            MergeReaderDataMapping(expectedDetail, GetReaderDataMappingForSportsbookType());

            MapReaderData(expectedDetail);

            // Action
            var result = await DataAccess.GetPopUpSingleMatchParlayAsync(matchId, marketId);
            var detailResult = result.BetBuilderDetails[0];

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(detailResult.SelectionType, Is.EqualTo(expectedDetail["SelectionType"]));
                Assert.That(detailResult.Result, Is.EqualTo(expectedDetail["Result"]));
            });
        }
    }
}