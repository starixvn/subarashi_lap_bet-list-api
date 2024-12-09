using Fanex.Athena.TestHelper.Extensions;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.Sportsbook.DTO;
using NSubstitute;

namespace Fanex.Athena.Result.DataAccess.Tests
{
    public partial class ResultDataAccessTests
    {
        [Test]
        public async Task GetMatchesAsync_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            dynamic param = null;
            ObjectDb.ExecuteReaderAsync(Arg.Do<dynamic>(x => param = x));
            _metaModel.SportId = 1;
            _metaModel.LeagueId = 2;

            var expected = new
            {
                fromdate = _metaModel.FromDate,
                sporttype = _metaModel.SportId,
                leagueid = _metaModel.LeagueId,
                movebetsday = _balance.MoveBetsDay,
                isshowall = true
            };

            // Action
            await DataAccess.GetMatchesAsync<Sportsbook>(_metaModel);

            // Assert
            Assert.That(param, Tobe.SameProperties(expected));
        }

        [Test]
        public async Task GetMatchesAsync_TypeIsSportsbook_ReturnCorrectData()
        {
            // Arrange
            Reader.Read().Returns(true, false, true, false);
            Reader.NextResult().Returns(true);

            SetUpForReader();

            var dataMapping = GetReaderDataMappingForSportsbookType();
            var dataMappingForGameDetail = GetreaderDataMappingForGameDetail();
            MergeReaderDataMapping(dataMapping, dataMappingForGameDetail);
            DataAccessTestHelper.MapReaderData(Reader, dataMapping);

            var sportsbookData = GetExpectedSportsbookData();
            sportsbookData.BetID = "0";
            sportsbookData.Bettype = 0;
            var expectedResult = new List<Sportsbook> { sportsbookData };

            // Action
            var result = await DataAccess.GetMatchesAsync<Sportsbook>(_metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(expectedResult.Count));
                Assert.That(result[0], Tobe.SameProperties(expectedResult[0]));
            });
        }

        [Test]
        public void GetMatchesAsync_TypeIsNotSupported_ThrowException()
        {
            // Action
            AsyncTestDelegate result = () => DataAccess.GetMatchesAsync<object>(_metaModel);

            // Assert
            Assert.ThrowsAsync<NotSupportedException>(result, "The type 'object' is not supported.");
        }
    }
}