
using AutoFixture;
using Fanex.Athena.BetList.DataAccess.Util;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Athena.TestHelper.Extensions;
using NSubstitute;
using System.Data;

namespace Fanex.Athena.BetList.DataAccess.Tests
{
    public class CashOutBetListDataAccessTests : BaseDataAccessHelperTest<CashOutBetListDataAccess>
    {
        private const string UspName = "Lrf_BetList_CashOut_Detail";

        private IBalanceUpDataAccess _balanceUpDataAccess;
        private IShardInfoAccessor _shardInfoAccessor;

        private Fixture _fixture => new Fixture();

        private BalanceUp _balanceUp;

        protected override void SetUpDataAccess()
        {
            _balanceUp = _fixture.Create<BalanceUp>();
            _balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            _balanceUpDataAccess.GetBalanceUp().Returns(_balanceUp);

            _shardInfoAccessor = Substitute.For<IShardInfoAccessor>();
            _shardInfoAccessor.UseShardDB.Returns(false);

            DataAccess = Substitute.ForPartsOf<CashOutBetListDataAccess>(CurrencyDataAccess, _shardInfoAccessor, _balanceUpDataAccess);
        }

        [Test]
        public async Task GetDetail_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = _fixture.Create<CashOutBetListDetailMetaModel>();
            var expectedParam = new
            {
                metaModel.TransId,
                metaModel.RefNo,
                metaModel.BetType,
                OriginalTransId = metaModel.BetdaqId,
                AgentList = metaModel.AgentListXml,
                metaModel.BaseCurrency,
                _balanceUp.MoveBetsDay,
                _balanceUp.BalanceUpDay
            };

            object param = null;
            await ObjectDb.ExecuteReaderAsync(Arg.Do<object>(x => param = x));

            // Action
            await DataAccess.GetDetail(metaModel);

            // Assert
            Assert.That(param, Tobe.SameProperties(expectedParam));
        }

        [Test]
        public async Task GetDetail_WhenCall_OriginalTicketReturnCorrectly()
        {
            // Arrange
            const string UserName = "user1";
            const int PlayerComm = 999;

            var safeReader = Substitute.For<ISafeDataReader>();
            DataAccess.InvokeProtectedMethod("GetSafeDataReader", Arg.Any<IDataReader>(), Arg.Any<string[]>()).ReturnsForAnyArgs(safeReader);

            safeReader.Read().Returns(true, false);

            safeReader.GetString("username").Returns(UserName);
            safeReader.GetDecimal("playercomm").Returns(PlayerComm);

            // Action
            var result = await DataAccess.GetDetail(new CashOutBetListDetailMetaModel());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.OriginalTicket.UserName, Is.EqualTo(UserName));
                Assert.That(result.OriginalTicket.PlayerComm, Is.EqualTo(PlayerComm));
            });
        }

        [Test]
        public async Task GetDetail_WhenCall_CashOutTicketListReturnCorrectly()
        {
            // Arrange
            Reader.Read().Returns(false, true, false);
            Reader.NextResult().Returns(true);

            var dataReader = new
            {
                BetType = 1,
                Stake = 2
            };
            MapReaderData(dataReader);

            // Action
            var result = await DataAccess.GetDetail(new CashOutBetListDetailMetaModel());

            var firstCashoutTicket = result.CashOutTicketList.First();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(firstCashoutTicket.BetType, Is.EqualTo(dataReader.BetType));
                Assert.That(firstCashoutTicket.Stake, Is.EqualTo(dataReader.Stake));
            });
        }

        [Test]
        public async Task GetHistoryDetail_WhenUseSiteDB_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = _fixture.Create<CashOutHistoryDetailMetaModel>();
            _shardInfoAccessor.UseShardDB.Returns(true);
            _balanceUpDataAccess.GetBalanceUp().Returns(_balanceUp);
            metaModel.Date = _balanceUp.MoveBetsDay.AddDays(1);

            // Action
            await DataAccess.GetHistoryDetail(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue(DbColumn.TransId, metaModel.TransId);
                ObjectDb.Received(1).SetParameterValue(DbColumn.CustId, metaModel.CustId);
                ObjectDb.Received(1).SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
                ObjectDb.Received(1).SetParameterValue("Currency", metaModel.CurrencyId);
                ObjectDb.DidNotReceive().SetParameterValue(DbColumn.BalanceUpDay, Arg.Any<DateTime>());
            });
        }

        [Test]
        public async Task GetHistoryDetail_WhenNotUseSiteDB_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = _fixture.Create<CashOutHistoryDetailMetaModel>();
            _shardInfoAccessor.UseShardDB.Returns(false);
            _balanceUpDataAccess.GetBalanceUp().Returns(_balanceUp);
            metaModel.Date = _balanceUp.MoveBetsDay.AddDays(-1);

            // Action
            await DataAccess.GetHistoryDetail(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue(DbColumn.TransId, metaModel.TransId);
                ObjectDb.Received(1).SetParameterValue(DbColumn.CustId, metaModel.CustId);
                ObjectDb.Received(1).SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
                ObjectDb.Received(1).SetParameterValue("Currency", metaModel.CurrencyId);
                ObjectDb.Received(1).SetParameterValue(DbColumn.BalanceUpDay, Arg.Any<DateTime>());
            });
        }

        [Test]
        public async Task GetHistoryDetail_WhenCall_ResultReturnCorrectly()
        {
            // Arrange
            var metaModel = _fixture.Create<CashOutHistoryDetailMetaModel>();
            var expectedResult = _fixture.Create<CashOutHistoryDetailViewModel>();

            var readerObjectMapper = new Dictionary<string, object>
            {
                { "TransId", expectedResult.TransId },
                { "CashoutExtraInfo", expectedResult.CashoutExtraInfo },
                { "Rate", expectedResult.Rate }
            };

            MapReaderData(readerObjectMapper);

            Reader.Read().Returns(true, false);

            // Action
            var result = await DataAccess.GetHistoryDetail(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Rate, Is.EqualTo(expectedResult.Rate));
                Assert.That(result.TransId, Is.EqualTo(expectedResult.TransId));
                Assert.That(result.CashoutExtraInfo, Is.EqualTo(expectedResult.CashoutExtraInfo));
                Assert.That(result.Currency, Is.EqualTo(metaModel.Currency));
                Assert.That(result.BaseCurrency, Is.EqualTo(metaModel.BaseCurrency));
            });
        }
    }
}
