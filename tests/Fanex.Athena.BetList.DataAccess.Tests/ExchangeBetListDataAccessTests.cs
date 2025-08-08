namespace Fanex.Athena.BetList.DataAccess.Tests
{
    using AutoFixture;
    using Common.Constant;
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;

    public class ExchangeBetListDataAccessTests : BaseDataAccessHelperTest<ExchangeBetListDataAccess>
    {
        private const string BetListUsp = "Lrf_BetList_Exchange_Mem";

        private Fixture _fixture => new Fixture();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public async Task GetBetListData_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = _fixture.Create<ExchangeBetListMetaModel>();

            // Action
            await DataAccess.GetBetListDataAsync(metaModel).ConfigureAwait(false);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("CustId", metaModel.CustId);
                ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
                ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
                ObjectDb.Received(1).SetParameterValue("SumWinLost", null);
            });
        }

        [Test]
        public async Task GetBetListData_NoData_ReturnEmptyResult()
        {
            // Arrange
            Reader.Read().Returns(false);
            ObjectDb.GetParameterValue("SumWinLost").Returns(0);

            // Action
            var result = await DataAccess.GetBetListDataAsync(new ExchangeBetListMetaModel());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SumWinLossList, Is.Empty);
                Assert.That(result.TicketList, Is.Empty);

                Assert.That(result.GrandTotal, Is.Zero);
            });
        }

        [Test]
        public async Task GetBetListData_HasSumWinLossListData_ReturnCorrectResult()
        {
            // Arrange
            var sumWL = _fixture.Create<decimal>();
            ObjectDb.GetParameterValue("SumWinLost").Returns(sumWL);

            var ticket = _fixture.Create<ExchangeSumWinLoss>();

            MapReaderData(ticket);

            Reader.Read().Returns(true, false, false);
            Reader.NextResult().Returns(true);

            var metaModel = new ExchangeBetListMetaModel();

            // Action
            var result = await DataAccess.GetBetListDataAsync(metaModel);
            var firstSumWL = result.SumWinLossList.ElementAt(0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(firstSumWL.UserId, Is.EqualTo(ticket.UserId));
                Assert.That(firstSumWL.EventId, Is.EqualTo(ticket.EventId));
                Assert.That(firstSumWL.EventName, Is.EqualTo(ticket.EventName));
                Assert.That(firstSumWL.EventTypeName, Is.EqualTo(ticket.EventTypeName));
                Assert.That(firstSumWL.MarketId, Is.EqualTo(ticket.MarketId));
                Assert.That(firstSumWL.MarketName, Is.EqualTo(ticket.MarketName));
                Assert.That(firstSumWL.WinLost, Is.EqualTo(ticket.WinLost));
                Assert.That(firstSumWL.Commission, Is.EqualTo(ticket.Commission));

                Assert.That(result.GrandTotal, Is.EqualTo(sumWL));
            });
        }

        [Test]
        public async Task GetBetListData_HasTicketDataData_ReturnCorrectResult()
        {
            // Arrange
            var ticket = _fixture.Create<ExchangeTicket>();
            ticket.BetPlaced = FakeHelper.TruncateMiliseconds(ticket.BetPlaced);
            MapReaderData(ticket);

            Reader.Read().Returns(false, true, false);
            Reader.NextResult().Returns(true);

            var metaModel = new ExchangeBetListMetaModel();

            // Action
            var result = (await DataAccess.GetBetListDataAsync(metaModel).ConfigureAwait(false)).TicketList.ElementAt(0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.UserId, Is.EqualTo(ticket.UserId));
                Assert.That(result.EventId, Is.EqualTo(ticket.EventId));
                Assert.That(result.MarketId, Is.EqualTo(ticket.MarketId));
                Assert.That(result.BetId, Is.EqualTo(ticket.BetId));
                Assert.That(result.SelectionName, Is.EqualTo(ticket.SelectionName));
                Assert.That(result.OddsAvg, Is.EqualTo(ticket.OddsAvg));
                Assert.That(result.WinLost, Is.EqualTo(ticket.WinLost));
                Assert.That(result.Stake, Is.EqualTo(ticket.Stake));
                Assert.That(result.BetPlaced, Is.EqualTo(ticket.BetPlaced));
                Assert.That(result.BetPlacedDateString, Is.EqualTo(ticket.BetPlaced.ToString(FormatKey.DateTimeFormat)));
            });
        }

        [Test]
        public async Task GetTicketDetailList_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = FakeHelper.Create<ExchangeBetListDetailMetaModel>();

            // Action
            await DataAccess.GetTicketDetailListAsync(metaModel).ConfigureAwait(false);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("CustId", metaModel.CustId);
                ObjectDb.Received(1).SetParameterValue("BetID", metaModel.BetId);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            });
        }

        [Test]
        public async Task GetTicketDetailList_NoData_ReturnEmptyResult()
        {
            // Arrange
            Reader.Read().Returns(false);

            // Action
            var result = await DataAccess.GetTicketDetailListAsync(new ExchangeBetListDetailMetaModel()).ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetTicketDetailList_HasData_ReturnCorrectResult()
        {
            // Arrange
            const string BaseCurrency = "usd";

            MapReaderData(new Dictionary<string, object>
            {
                { "BetTaken", "2018/02/02" },
                { "OddsRequest", 2 },
                { "Stake", 3 },
                { "OddsMatched", 4 },
            });

            Reader.Read().Returns(true, false);

            var metaModel = new ExchangeBetListDetailMetaModel
            {
                BaseCurrency = BaseCurrency
            };

            // Action
            var result = await DataAccess.GetTicketDetailListAsync(metaModel).ConfigureAwait(false);
            var firstItem = result.ElementAt(0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(firstItem.BetTaken, Is.EqualTo(new DateTime(2018, 2, 2)));
                Assert.That(firstItem.OddsReq, Is.EqualTo(2));
                Assert.That(firstItem.Stake, Is.EqualTo(3));
                Assert.That(firstItem.OddsMatched, Is.EqualTo(4));
            });
        }

        [TestCase("Lay", "9.00")]
        [TestCase("WTH", "9.00")]
        [TestCase("Back", "-")]
        [TestCase("Yes", "-")]
        public async Task GetTicketDetailList_HasData_ReturnCorrectLiability(
            string exchangeMatchType,
            string expectedLiability)
        {
            // Arrange
            MapReaderData(new Dictionary<string, object>
            {
                { "Stake", 3 },
                { "OddsMatched", 4 },
            });

            Reader.Read().Returns(true, false);

            var metaModel = new ExchangeBetListDetailMetaModel
            {
                Type = exchangeMatchType
            };

            // Action
            var result = await DataAccess.GetTicketDetailListAsync(metaModel).ConfigureAwait(false);

            // Assert
            Assert.That(result.ElementAt(0).Liability, Is.EqualTo(expectedLiability));
        }

        [Test]
        public async Task GetRunningTicketsAsync_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            var metaModel = FakeHelper.Create<ExchangeRunningTicketMetaModel>();
            var dataList = FakeHelper.CreateMany<ExchangeRunningTicket>();
            ObjectDb.QueryAsync<ExchangeRunningTicket>().Returns(dataList);

            // Action
            var result = await DataAccess.GetRunningTicketsAsync(metaModel);

            // Assert
            Assert.That(result.DataList, Is.EqualTo(dataList));
        }

    }
}
