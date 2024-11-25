

using AutoFixture;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.TestHelper;
using NSubstitute;

namespace Fanex.Athena.BetList.DataAccess.Tests
{
    public class PlayerSegmentationBetListDataAccessTests : BaseDataAccessHelperTest<PlayerSegmentationBetListDataAccess>
    {
        private IBalanceUpDataAccess _balanceUpDataAccessMock;
        private ICurrencyDataAccess _currencyDataAccessMock;

        private Fixture _fixture => new Fixture();

        protected override void SetUpDataAccess()
        {
            _balanceUpDataAccessMock = Substitute.For<IBalanceUpDataAccess>();
            _currencyDataAccessMock = Substitute.For<ICurrencyDataAccess>();
            DataAccess = Substitute.ForPartsOf<PlayerSegmentationBetListDataAccess>(_balanceUpDataAccessMock, _currencyDataAccessMock);
        }

        [Test]
        public async Task GetAbnormalBetListAsync_ShouldReturnTicketResult()
        {
            // Arrange
            var metaModel = new PlayerSegmentationBetListMetaModel
            {
                ListProductCode = new List<string> { "BA", "Product1", "Product2" },
                CustId = 123,
                FromDate = new DateTime(2022, 1, 1),
                ToDate = new DateTime(2022, 1, 31),
                AgentListXml = "<AgentList><Agent>Agent1</Agent><Agent>Agent2</Agent></AgentList>",
                BaseCurrency = "USD",
                PageSize = 10,
                PageIndex = 1
            };
            var balanceUp = _fixture.Create<BalanceUp>();
            _balanceUpDataAccessMock.GetBalanceUp().Returns(balanceUp);

            // Act
            var result = await DataAccess.GetAbnormalBetListAsync(metaModel);

            // Assert
            ObjectDb.Received(1).SetParameterValue("MoveBetsDay", balanceUp.MoveBetsDay);
            ObjectDb.Received(1).SetParameterValue("BalanceUpDay", balanceUp.BalanceUpDay);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.MainTicketList, Is.Not.Null);
            Assert.That(result.MainTicketList.Count, Is.EqualTo(0));
            Assert.That(result.TotalRows, Is.EqualTo(0));
        }
    }
}
