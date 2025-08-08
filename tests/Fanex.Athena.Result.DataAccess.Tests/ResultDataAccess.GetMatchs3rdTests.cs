namespace Fanex.Athena.Result.DataAccess.Tests
{
    using Fanex.Result.Core.Bitcoin.DTO;
    using Fanex.Result.Core.VirtualRacing.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;

    public partial class ResultDataAccessTests
    {
        [Test]
        public async Task GetMatches3rdAsyn_WhenGetBitCoin_ReturnResultCorrectly()
        {
            // Arrange
            var metaMedel = FakeHelper.Create<ResultMetaModel>();
            var data = FakeHelper.Create<Bitcoin>();
            var detailsData = FakeHelper.Create<GameDetail>();
            var mapColDic = new Dictionary<string, object>
            {
                //Main
                {"League3rdId", data.League3rdId },
                {"Match3rdId", data.Match3rdId },
                {"betstatus", data.BetStatus},
                {"EventStatus", data.EventStatus},

                //Details
                {"GameResult", detailsData.GameResult},
            };
            Reader.Read().Returns(true, false, true, false);
            Reader.NextResult().Returns(true, false);
            MapReaderData(mapColDic);

            // Action
            var result = await DataAccess.GetMatches3rdAsync<Bitcoin>(metaMedel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].League3rdId, Is.EqualTo(data.League3rdId));
                Assert.That(result[0].Match3rdId, Is.EqualTo(data.Match3rdId));
                Assert.That(result[0].BetStatus, Is.EqualTo(data.BetStatus));
                Assert.That(result[0].EventStatus, Is.EqualTo(data.EventStatus));
                Assert.That(result[0].GameResults.Count, Is.EqualTo(1));
                Assert.That(result[0].GameResults[0].GameResult, Is.EqualTo(detailsData.GameResult));
            });
        }

        [Test]
        public async Task GetMatches3rdAsyn_WhenGetNotBitCoin_NotSupportedException()
        {
            // Arrange
            var metaMedel = FakeHelper.Create<ResultMetaModel>();

            // Assert
            Assert.ThrowsAsync<NotSupportedException>(async () =>
                await DataAccess.GetMatches3rdAsync<VirtualRacing>(metaMedel));
        }

        [Test]
        public async Task GetPopUpMatches3rdAsync_WhenGetNotBitCoin_NotSupportedException()
        {
            // Arrange
            var marchId = 123;

            // Assert
            Assert.ThrowsAsync<NotSupportedException>(async () =>
                await DataAccess.GetPopUpMatches3rdAsync<VirtualRacing>(marchId));
        }

        [Test]
        public async Task GetPopUpMatches3rdAsync_WhenCall_ReturnResultCorrectly()
        {
            // Arrange
            var eventId = FakeHelper.Create<int>();
            var data = FakeHelper.Create<Bitcoin>();
            var detailsData = FakeHelper.Create<GameDetail>();
            var mapColDic = new Dictionary<string, object>
            {
                //Main
                {"League3rdId", data.League3rdId },
                {"Match3rdId", data.Match3rdId },
                {"betstatus", data.BetStatus},
                {"EventStatus", data.EventStatus},

                //Details
                {"GameResult", detailsData.GameResult},
            };
            Reader.Read().Returns(true, false, true, false);
            Reader.NextResult().Returns(true, false);
            MapReaderData(mapColDic);

            // Action
            var result = await DataAccess.GetPopUpMatches3rdAsync<Bitcoin>(eventId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].League3rdId, Is.EqualTo(data.League3rdId));
                Assert.That(result[0].Match3rdId, Is.EqualTo(data.Match3rdId));
                Assert.That(result[0].BetStatus, Is.EqualTo(data.BetStatus));
                Assert.That(result[0].EventStatus, Is.EqualTo(data.EventStatus));
                Assert.That(result[0].GameResults.Count, Is.EqualTo(1));
                Assert.That(result[0].GameResults[0].GameResult, Is.EqualTo(detailsData.GameResult));
            });

        }
    }
}
