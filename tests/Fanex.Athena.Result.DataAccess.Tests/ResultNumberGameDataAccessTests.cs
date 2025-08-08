namespace Fanex.Athena.Result.DataAccess.Tests
{
    using Fanex.Result.Core.Happy5.DTO;
    using Fanex.Result.Core.NumberGame.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;
    using TestHelper.Extensions;

    public class ResultNumberGameDataAccessTests : BaseDataAccessHelperTest<ResultNumberGameDataAccess>
    {
        private const int MatchId = 122;

        private readonly ResultMetaModel _metaModel = new ResultMetaModel
        {
            FromDate = new DateTime(2017, 1, 1),
            ToDate = new DateTime(2017, 2, 2),
            SportId = 123
        };

        private readonly Dictionary<string, object> _readerDataMapping = new()
        {
            { "leagueid", 111 }, { "matchid", 222 }, { "eventstatus", "win" }, { "eventdate", "03/16/2017" },
            { "isrefund", "1" },
            { "kickofftime", "1/1/2017" }, { "matchcode", "test" }, { "seq", 3 }, { "number", 4 }
        };

        [Test]
        public async Task GetPopUpNumberGameMatchAsync_WhenCall_ParameterPassedCorrectly()
        {
            // Arrange
            dynamic result = null;
            ObjectDb.ExecuteReaderAsync(Arg.Do<dynamic>(x => result = x));
            var expected = new
            {
                matchId = MatchId
            };

            // Action
            await DataAccess.GetPopUpNumberGameMatchAsync(MatchId);

            // Assert
            Assert.That(result, Tobe.SameProperties(expected));
        }

        [Test]
        public async Task GetPopUpNumberGameMatchAsync_WhenCall_ReturnCorrectData()
        {
            // Arrange
            Reader.Read().Returns(true, true, false);
            Reader.FieldCount.Returns(1);
            Reader.GetName(0).Returns("isrefund");

            MapReaderData(_readerDataMapping);
            var dataItem = GetSampleDataItem();
            dataItem.IsPending = true;
            var expectedResult = new List<NumberGame> { dataItem };

            // Action
            var result = await DataAccess.GetPopUpNumberGameMatchAsync(222);

            // Assert
            Assert.That(ObjectHelper.ToJson(result), Is.EqualTo(ObjectHelper.ToJson(expectedResult)));
        }

        [Test]
        public async Task GetPopUpNumberGameMatchAsync_IsRefundColumnNotExist_ReturnCorrectData()
        {
            // Arrange
            Reader.Read().Returns(true, true, false);
            MapReaderData(_readerDataMapping);
            var dataItem = GetSampleDataItem();
            dataItem.IsPending = false;
            var expectedResult = new List<NumberGame> { dataItem };

            // Action
            var result = await DataAccess.GetPopUpNumberGameMatchAsync(222);

            // Assert
            Assert.That(ObjectHelper.ToJson(result), Is.EqualTo(ObjectHelper.ToJson(expectedResult)));
        }

        [Test]
        public async Task GetNumberGameMatchesAsync_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            _metaModel.LeagueId = 1;

            dynamic result = null;
            ObjectDb.ExecuteReaderAsync(Arg.Do<dynamic>(x => result = x));
            var expectedResult = new
            {
                fromdate = _metaModel.FromDate,
                leagueid = _metaModel.LeagueId,
                sporttype = _metaModel.SportId,
                isshowall = true
            };

            // Action
            await DataAccess.GetNumberGameMatchesAsync(_metaModel);

            // Assert
            Assert.That(result, Tobe.SameProperties(expectedResult));
        }

        [Test]
        public async Task GetNumberGameMatchesAsync_WhenCall_ReturnCorrectData()
        {
            // Arrange
            _metaModel.LeagueId = 1;

            Reader.Read().Returns(true, true, false);
            Reader.FieldCount.Returns(1);
            Reader.GetName(0).Returns("isrefund");

            MapReaderData(_readerDataMapping);

            var expectedResult = new List<NumberGame>
            {
                GetSampleDataItem()
            };

            // Action
            var result = await DataAccess.GetNumberGameMatchesAsync(_metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(expectedResult.Count));
                Assert.That(result[0], Tobe.SameProperties(expectedResult[0]));
            });
        }

        [Test]
        public async Task GetHappy5MatchesAsync_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            _metaModel.LeagueId = 1;

            dynamic result = null;
            ObjectDb.ExecuteReaderAsync(Arg.Do<dynamic>(x => result = x));
            var expectedResult = new
            {
                fromdate = _metaModel.FromDate,
                leagueid = _metaModel.LeagueId,
                sporttype = _metaModel.SportId,
                isshowall = true
            };

            // Action
            await DataAccess.GetHappy5MatchesAsync(_metaModel);

            // Assert
            Assert.That(result, Tobe.SameProperties(expectedResult));
        }

        [Test]
        public async Task GetHappy5MatchesAsync_WhenCall_ReturnCorrectData()
        {
            // Arrange
            _metaModel.LeagueId = 1;

            Reader.Read().Returns(true, true, false);
            Reader.FieldCount.Returns(1);
            Reader.GetName(0).Returns("isrefund");

            MapReaderData(_readerDataMapping);

            var expectedResult = new List<Happy5>
            {
                GetHappy5SampleDataItem()
            };

            // Action
            var result = await DataAccess.GetHappy5MatchesAsync(_metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(expectedResult.Count));
                Assert.That(result[0], Tobe.SameProperties(expectedResult[0]));
            });
        }

        [Test]
        public async Task GetPopUpHappy5MatchAsync_WhenCall_ParameterPassedCorrectly()
        {
            // Arrange
            dynamic result = null;
            ObjectDb.ExecuteReaderAsync(Arg.Do<dynamic>(x => result = x));
            var expected = new
            {
                matchId = MatchId
            };

            // Action
            await DataAccess.GetPopUpHappy5MatchAsync(MatchId);

            // Assert
            Assert.That(result, Tobe.SameProperties(expected));
        }

        [Test]
        public async Task GetPopUpHappy5MatchAsync_WhenCall_ReturnCorrectData()
        {
            // Arrange
            Reader.Read().Returns(true, true, false);
            Reader.FieldCount.Returns(1);
            Reader.GetName(0).Returns("isrefund");

            MapReaderData(_readerDataMapping);
            var dataItem = GetHappy5SampleDataItem();
            dataItem.IsPending = true;
            var expectedResult = new List<Happy5> { dataItem };

            // Action
            var result = await DataAccess.GetPopUpHappy5MatchAsync(222);

            // Assert
            Assert.That(ObjectHelper.ToJson(result), Is.EqualTo(ObjectHelper.ToJson(expectedResult)));
        }

        private static Happy5 GetHappy5SampleDataItem()
        {
            return new Happy5
            {
                LeagueId = 111,
                MatchId = 222,
                EventStatus = "win",
                EventDate = new DateTime(2017, 3, 16),
                IsPending = true,
                KickOffTime = new DateTime(2017, 1, 1),
                MatchCode = "test",
                Seq = new List<int> { 3, 3 },
                Ball = new List<int> { 4, 4 }
            };
        }

        private static NumberGame GetSampleDataItem()
        {
            return new NumberGame
            {
                LeagueId = 111,
                MatchId = 222,
                EventStatus = "win",
                EventDate = new DateTime(2017, 3, 16),
                IsPending = true,
                KickOffTime = new DateTime(2017, 1, 1),
                MatchCode = "test",
                Seg = new List<int> { 3, 3 },
                Ball = new List<int> { 4, 4 }
            };
        }
    }
}