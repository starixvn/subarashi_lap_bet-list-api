namespace Fanex.Athena.BetList.DataAccess.Tests
{
    using Model.DTO;
    using Model.MetaModel;
    using NSubstitute;
    using TestHelper;

    public class LastBetsMonitoringBetListDataAccessTests : BaseDataAccessHelperTest<LastBetsMonitoringBetListDataAccess>
    {
        protected override void SetUpDataAccess()
        {
            DataAccess = Substitute.ForPartsOf<LastBetsMonitoringBetListDataAccess>(
                CurrencyDataAccess);
        }

        [Test]
        public async Task GetLastBetsMonitoringData_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = FakeHelper.Create<LastBetsMonitoringMetaModel>();

            // Action
            await DataAccess.GetLastBetsMonitoringData(metaModel).ConfigureAwait(false);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("BetType", metaModel.BetType);
                ObjectDb.Received(1).SetParameterValue("Currency", metaModel.Currency);
                ObjectDb.Received(1).SetParameterValue("SpecialLevel", metaModel.Level);
                ObjectDb.Received(1).SetParameterValue("TransId", metaModel.TransId);
                ObjectDb.Received(1).SetParameterValue("TransLogId", metaModel.TransLogId);
                ObjectDb.Received(1).SetParameterValue("LimitRowCount", metaModel.PageSize);
                ObjectDb.Received(1).SetParameterValue("MatchId", metaModel.MatchId);
                ObjectDb.Received(1).SetParameterValue("MinStake", metaModel.StakeFrom);
                ObjectDb.Received(1).SetParameterValue("MaxStake", metaModel.StakeTo);
                ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BasedCurrency);
                ObjectDb.Received(1).SetParameterValue("BetFrom", metaModel.BetFrom);
                ObjectDb.Received(1).SetParameterValue("CustomerClass", metaModel.CustomerClass);
                ObjectDb.Received(1).SetParameterValue("LeagueList", metaModel.LeagueList);
                ObjectDb.Received(1).SetParameterValue("SportTypeList", metaModel.SportTypeList);
                ObjectDb.Received(1).SetParameterValue("ProductList", metaModel.ProductCodeList);
            });
        }

        [Test]
        public async Task GetMatchList_WhenCall_DataReturnedCorrectly()
        {
            // Arrange
            var expected = new BetListMatch { MatchId = 1, LeagueId = 2, LeagueGroupId = 1 };
            ObjectDb.QueryAsync<BetListMatch>().Returns(new List<BetListMatch> { expected });

            // Action
            var result = (await DataAccess.GetMatchList("SPNameTest").ConfigureAwait(false))[0];

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetMatchById_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            var expected = new BetListMatch { MatchId = 1, LeagueId = 2, LeagueGroupId = 1 };
            ObjectDb.QueryEntityAsync<BetListMatch>().Returns(expected);

            // Action
            var result = await DataAccess.GetMatchById(1);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
