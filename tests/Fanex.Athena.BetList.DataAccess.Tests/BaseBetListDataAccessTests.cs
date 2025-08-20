namespace Fanex.Athena.BetList.DataAccess.Tests
{
    using AutoFixture;
    using Common.Model.MetaModel;
    using Data;
    using Fanex.BetList.Core.Entities;
    using Model.DTO;
    using NSubstitute;
    using TestClasses;
    using TestHelper;
    using TestHelper.Extensions;

    [TestFixture]
    public class BaseBetListDataAccessTests : BaseDataAccessHelperTest<BaseBetListDataAccessTestClass>
    {
        private Fixture _fixture = new Fixture();

        protected override void SetUpDataAccess()
        {
            DataAccess = Substitute.ForPartsOf<BaseBetListDataAccessTestClass>(CurrencyDataAccess);
        }

        [Test]
        public void ReadLrfTicket_WhenCall_DataReturnedCorrectly()
        {
            // Arrange
            var expected = GetFakeTicket();
            var dataList = GetFakeBetListData(expected);
            Reader.Read().Returns(true, false);
            ConfigureReader(dataList);

            // Action
            var result = DataAccess.ReadBetListTicket(Reader, UserCurrencyList)[0];

            // Assert
            Assert.That(result, Tobe.SameProperties(expected));
        }

        [Test]
        public async Task GetCurrencyListAsync_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            var expected = UserCurrencyList;

            // Action
            var result = await DataAccess.GetCurrencyListAsync();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("SPName_SabaCoins")]
        [TestCase("SPName")]
        public async Task GetMainTicketListBasedRangeAsync_WhenUseShardDb_DataReturnedCorrectly(string spName)
        {
            // Arrange
            var expected = GetFakeTicket();
            var dataList = GetFakeBetListData(expected);
            Reader.Read().Returns(true, false, true, false);
            ConfigureReader(dataList);
            var metaModel = Substitute.For<IShardDBMetaModel>();
            metaModel.UseShardDB.Returns(true);

            //Action
            var result = await DataAccess.GetMainTicketListBasedRangeAsync(metaModel, spName, Substitute.For<Action<IObjectDb>>());

            //Assert
            Assert.That(result.MainTicketList, Has.Count.EqualTo(2));
            Assert.That(result.MainTicketList[0], Tobe.SameProperties(expected));
            Assert.That(result.MainTicketList[1], Tobe.SameProperties(expected));
        }

        [Test]
        public async Task GetMainTicketListBasedRangeAsync_WhenNotUseShardDb_DataReturnedCorrectly()
        {
            // Arrange
            var expected = GetFakeTicket();
            var dataList = GetFakeBetListData(expected);
            Reader.Read().Returns(true, false, true, false);
            ConfigureReader(dataList);
            var metaModel = Substitute.For<IShardDBMetaModel>();
            metaModel.UseShardDB.Returns(false);

            //Action
            var result = await DataAccess.GetMainTicketListBasedRangeAsync(metaModel, _fixture.Create<string>(), Substitute.For<Action<IObjectDb>>());

            //Assert
            Assert.That(result.MainTicketList, Has.Count.EqualTo(1));
            Assert.That(result.MainTicketList[0], Tobe.SameProperties(expected));
        }

        [Test]
        public void ReadReferneceTicket_WhenCall_ReturnResultCorrectly()
        {
            // Arrange
            var expected = new TicketData
            {
                BetTeam = "BetTeamTest",
                BetTypeId = _fixture.Create<int>(),
                Odds = 200.8m,
                RefNo = "RefNoTest",
                Stake = 200.6m,
                Status = "StatusTest"
            };
            var dataList = GetFakeTicketData(expected);
            ConfigureReader(dataList);
            Reader.Read().Returns(true, false);

            //Action
            var result = DataAccess.ReadReferneceTicket(Reader).ToList();

            //Assert
            Assume.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First(), Tobe.SameProperties(expected));
        }

        private static IDictionary<string, object> GetFakeBetListData(LrfTicket ticket)
        {
            var playerCommCol = "playercomm";
            var commCol = "comm";
            var licenseeWlCol = "licenseewl";
            var winlostCol = "winlost";
            var beteffectiveCol = "beteffective";
            var stakeCol = "stake";
            var actualStakeCol = "actual_stake";
            var originalStakeCol = "originalstake";

            var dataList = new Dictionary<string, object>
            {
                { playerCommCol, ticket.PlayerComm },
                { commCol, ticket.Comm },
                { licenseeWlCol, ticket.LicenseeWL },
                { winlostCol, ticket.Winlost },
                { beteffectiveCol, ticket.BetEffective },
                { stakeCol, ticket.Stake },
                { actualStakeCol, ticket.ActualStake },
                { originalStakeCol, ticket.OriginalStake }
            };

            AddStringData(dataList, ticket);
            AddBooleanData(dataList, ticket);
            AddDateTimeData(dataList, ticket);
            AddNumbericData(dataList, ticket);

            return dataList;
        }

        private static IDictionary<string, object> GetFakeTicketData(TicketData ticketData)
        {
            var stake = "stake";
            var betteam = "betteam";
            var refno = "refno";
            var status = "status";
            var odds = "odds";
            var bettype = "bettype";

            var dataList = new Dictionary<string, object>
            {
                { stake, ticketData.Stake },
                { betteam, ticketData.BetTeam },
                { refno, ticketData.RefNo },
                { status, ticketData.Status },
                { odds, ticketData.Odds },
                { bettype, ticketData.BetTypeId },
            };

            return dataList;

        }

        private static void AddStringData(Dictionary<string, object> dataList, LrfTicket ticket)
        {
            dataList.Add("username", ticket.UserName);
            dataList.Add("refno", ticket.RefNo);
            dataList.Add("matchcode", ticket.MatchCode);
            dataList.Add("status", ticket.Status);
            dataList.Add("betteam", ticket.BetTeam);
            dataList.Add("betstatus", ticket.EventStatus);
            dataList.Add("showtime", ticket.ShowTime);
            dataList.Add("oddstype", ticket.OddsType);
            dataList.Add("nickname", ticket.ExternalId);
            dataList.Add("race", ticket.Race);
            dataList.Add("betfrom", ticket.BetFrom);
            dataList.Add("betcheck", ticket.BetCheck);
        }

        private static void AddBooleanData(Dictionary<string, object> dataList, LrfTicket ticket)
        {
            dataList.Add("liveindicator", ticket.IsLive);
            dataList.Add("isneutral", ticket.IsNeutral);
        }

        private static void AddDateTimeData(Dictionary<string, object> dataList, LrfTicket ticket)
        {
            dataList.Add("winlostdate", ticket.WinlostDate);
            dataList.Add("transdate", ticket.TransDate);
            dataList.Add("eventdate", ticket.EventDate);
        }

        private static void AddNumbericData(Dictionary<string, object> dataList, LrfTicket ticket)
        {
            dataList.Add("sporttype", ticket.SportTypeId);
            dataList.Add("bettype", ticket.BetTypeId);
            dataList.Add("leagueid", ticket.LeagueId);
            dataList.Add("awayid", ticket.AwayId);
            dataList.Add("homeid", ticket.HomeId);
            dataList.Add("matchid", ticket.MatchId);
            dataList.Add("custid", ticket.CustId);
            dataList.Add("livehomescore", ticket.LiveHomeScore);
            dataList.Add("liveawayscore", ticket.LiveAwayScore);
            dataList.Add("SpecialLevel", ticket.SpecialLevel);
            dataList.Add("StatusId", ticket.StatusId);

            dataList.Add("transid", ticket.TransId);
            dataList.Add("betid", ticket.BetId);

            dataList.Add("odds", ticket.Odds);
            dataList.Add("modds", ticket.MasterOdds);
            dataList.Add("hdp1", ticket.Handicap1);
            dataList.Add("hdp2", ticket.Handicap2);
            dataList.Add("transdesc", ticket.TransDesc);
            dataList.Add("currency", ticket.CurrencyId);

            dataList.Add("betdaqid", ticket.BetdaqId);
            dataList.Add("extrainfo", ticket.ExtraInfo);
            dataList.Add("originalbettype", ticket.OriginalBetType);
            dataList.Add("oddsspread", ticket.OddsSpread);
            dataList.Add("SettlementStatus", ticket.SettlementStatus);
        }

        private void ConfigureReader(IDictionary<string, object> dataList)
        {
            var index = 0;

            Reader.FieldCount.Returns(dataList.Count);

            foreach (var item in dataList)
            {
                Reader.GetName(index).Returns(item.Key);
                Reader[item.Key].Returns(item.Value);
                index++;
            }
        }

        private LrfTicket GetFakeTicket()
        {
            return new LrfTicket
            {
                UserName = _fixture.Create<string>(),
                RefNo = "RefNoTest",
                MatchCode = "MatchCodeTest",
                Status = "StatusTest",
                BetTeam = "BetTeamTest",
                EventStatus = "EventStatusTest",
                ShowTime = "ShowTimeTest",
                OddsType = "OddsTypeTest",
                ExternalId = "ExternalIdTest",
                Race = "RaceTest",
                BetFrom = "BetFromTest",
                BetCheck = "BetCheckTest",
                IsLive = _fixture.Create<bool>(),
                IsNeutral = _fixture.Create<bool>(),
                SportTypeId = _fixture.Create<int>(),
                BetTypeId = _fixture.Create<int>(),
                LeagueId = 3,
                HomeId = 4,
                AwayId = 5,
                MatchId = 6,
                CustId = 7,
                CurrencyId = CurrencyId,
                LiveHomeScore = 9,
                LiveAwayScore = 10,
                SpecialLevel = 11,
                StatusId = 12,
                TransId = 100,
                BetId = 101,
                WinlostDate = new DateTime(2016, 8, 10),
                TransDate = new DateTime(2016, 8, 17),
                EventDate = new DateTime(2016, 8, 11),
                PlayerComm = 200.1m,
                Comm = 200.2m,
                LicenseeWL = 200.3m,
                Winlost = 200.4m,
                BetEffective = 200.5m,
                Stake = 200.6m,
                ActualStake = 200.7m,
                Odds = 200.8m,
                MasterOdds = 200.9m,
                Handicap1 = 200.11m,
                Handicap2 = 200.12m,
                TransDesc = "TransDescTest",
                Currency = CurrencyName,
                BetdaqId = 4534,
                ExtraInfo = "sdfsdf",
                OriginalStake = 3434.23m,
                OriginalBetType = 234,
                OddsSpread = 934.23m,
                SettlementStatus = 23
            };
        }
    }
}