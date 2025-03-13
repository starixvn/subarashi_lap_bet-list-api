using Fanex.Athena.TestHelper.Extensions;
using Fanex.Athena.TestHelper;
using NSubstitute;

namespace Fanex.Athena.Result.DataAccess.Tests
{
    public partial class ResultDataAccessTests
    {
        [Test]
        public async Task GetPopUpParalayMatchAsync_WhenCall_ParameterPassedCorrectly()
        {
            // Arrange
            dynamic result = null;
            ObjectDb.ExecuteReaderAsync(Arg.Do<dynamic>(x => result = x));
            var refNo = 1;
            var expected = new
            {
                refno = refNo
            };

            // Action
            await DataAccess.GetPopUpParalayMatchAsync(refNo);

            // Assert
            Assert.That(result, Tobe.SameProperties(expected));
        }

        [Test]
        public async Task GetPopUpParalayMatchAsync_WhenCall_ReturnCorrectData()
        {
            // Arrange

            Reader.Read().Returns(true, false, true, false);
            Reader.NextResult().Returns(true);

            SetUpForReader();

            var refNo = FakeHelper.Create<long>();
            var sportId = FakeHelper.Create<int>();

            var sportsbookData = GetExpectedSportsbookData();
            sportsbookData.RefNo = refNo.ToString();

            var dataMapping = GetReaderDataMappingForSportsbookType();
            dataMapping["sporttype"] = sportId;
            dataMapping["bettype"] = sportsbookData.Bettype;
            dataMapping["betcheck"] = sportsbookData.BetID;
            var dataMappingForGameDetail = GetreaderDataMappingForGameDetail();
            MergeReaderDataMapping(dataMapping, dataMappingForGameDetail);
            MapReaderData(dataMapping);

            // Action
            var result = await DataAccess.GetPopUpParalayMatchAsync(refNo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result[0].LeagueId, Is.EqualTo(sportsbookData.LeagueId));
                Assert.That(result[0].SportId, Is.EqualTo(sportId));
                Assert.That(result[0].Sportbook[0], Tobe.SameProperties(sportsbookData));
                Assert.That(result[0].Sportbook[0].GameDetails[0], Tobe.SameProperties(sportsbookData.GameDetails[0]));
            });
        }

        [Test]
        public async Task
            GetPopUpParalayMatchAsync_ParlayItemHasMultipleSportsbookItems_ReturnCorrectLengthOfSportsbookList()
        {
            // Arrange
            Reader.Read().Returns(true, true, false, true, false);
            Reader.NextResult().Returns(true);

            var dataMapping = GetReaderDataMappingForSportsbookType();
            dataMapping["LeagueID"] = 11;
            dataMapping["sporttype"] = 12;
            var dataMappingForGameDetail = GetreaderDataMappingForGameDetail();
            MergeReaderDataMapping(dataMapping, dataMappingForGameDetail);
            DataAccessTestHelper.MapReaderData(Reader, dataMapping);

            // Action
            var result = await DataAccess.GetPopUpParalayMatchAsync(FakeHelper.Create<long>());

            // Assert
            Assert.That(result[0].Sportbook.Count, Is.EqualTo(2));
        }
    }
}