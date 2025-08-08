namespace Fanex.Athena.Result.DataAccess.Tests
{
    using Fanex.Result.Core.VirtualRacing.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;

    public partial class ResultDataAccessTests
    {
        [Test]
        public async Task GetBetradaSportMatchesAsync_WhenCall_ReturnResultCorrectly()
        {
            // Arrange
            var metaMedel = FakeHelper.Create<ResultMetaModel>();
            var data = FakeHelper.Create<VirtualRacing>();
            var detailsData = FakeHelper.Create<VirtualRacingDetail>();
            var mapColDic = new Dictionary<string, object>
            {
                //Main
                {"LeagueID", data.LeagueId},
                {"EventID", data.EventId},
                {"VendorMatchId", data.VendorMatchId},
                {"EventTime", data.EventTime},
                {"EventStatus", data.EventStatus},
                {"BetStatus", data.BetStatus},
                {"Distance", data.Distance},

                //Details
                {"Orid", detailsData.Orid},
                {"Placing", detailsData.Placing},
                {"Lane", detailsData.Lane},
                {"RacerID", detailsData.RacerId},
            };
            Reader.Read().Returns(true, false, true, false);
            Reader.NextResult().Returns(true, false);
            MapReaderData(mapColDic);

            // Action
            var result = await DataAccess.GetVSNewRacingSportMatchesAsync(metaMedel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].LeagueId, Is.EqualTo(data.LeagueId));
                Assert.That(result[0].EventId, Is.EqualTo(data.EventId));
                Assert.That(result[0].VendorMatchId, Is.EqualTo(data.VendorMatchId));
                Assert.That(result[0].EventStatus, Is.EqualTo(data.EventStatus));
                Assert.That(result[0].BetStatus, Is.EqualTo(data.BetStatus));
                Assert.That(result[0].Distance, Is.EqualTo(data.Distance));
                Assert.That(result[0].Details.Count, Is.EqualTo(1));
                Assert.That(result[0].Details[0].Orid, Is.EqualTo(detailsData.Orid));
                Assert.That(result[0].Details[0].Placing, Is.EqualTo(detailsData.Placing));
                Assert.That(result[0].Details[0].Lane, Is.EqualTo(detailsData.Lane));
                Assert.That(result[0].Details[0].RacerId, Is.EqualTo(detailsData.RacerId));
            });
        }

        [Test]
        public async Task GetPopUpBetradarAsync_WhenCall_ReturnResultCorrectly()
        {
            // Arrange
            var eventId = FakeHelper.Create<int>();
            var data = FakeHelper.Create<VirtualRacing>();
            var detailsData = FakeHelper.Create<VirtualRacingDetail>();
            var mapColDic = new Dictionary<string, object>
            {
                //Main
                {"LeagueID", data.LeagueId},
                {"EventID", data.EventId},
                {"VendorMatchId", data.VendorMatchId},
                {"EventTime", data.EventTime},
                {"EventStatus", data.EventStatus},
                {"BetStatus", data.BetStatus},
                {"Distance", data.Distance},

                //Details
                {"Orid", detailsData.Orid},
                {"Placing", detailsData.Placing},
                {"Lane", detailsData.Lane},
                {"RacerID", detailsData.RacerId},
            };
            Reader.Read().Returns(true, true, false);
            Reader.NextResult().Returns(true, false);
            MapReaderData(mapColDic);

            // Action
            var result = await DataAccess.GetPopUpBetradarAsync(eventId, DateTime.MaxValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].LeagueId, Is.EqualTo(data.LeagueId));
                Assert.That(result[0].EventId, Is.EqualTo(data.EventId));
                Assert.That(result[0].VendorMatchId, Is.EqualTo(data.VendorMatchId));
                Assert.That(result[0].EventStatus, Is.EqualTo(data.EventStatus));
                Assert.That(result[0].BetStatus, Is.EqualTo(data.BetStatus));
                Assert.That(result[0].Distance, Is.EqualTo(data.Distance));
                Assert.That(result[0].Details.Count, Is.EqualTo(1));
                Assert.That(result[0].Details[0].Orid, Is.EqualTo(detailsData.Orid));
                Assert.That(result[0].Details[0].Placing, Is.EqualTo(detailsData.Placing));
                Assert.That(result[0].Details[0].Lane, Is.EqualTo(detailsData.Lane));
                Assert.That(result[0].Details[0].RacerId, Is.EqualTo(detailsData.RacerId));
            });

        }
    }
}
