using AutoFixture;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.TestHelper;
using NSubstitute;
using System.Collections;

namespace Fanex.Athena.BetList.DataAccess.Tests
{
    public class BetListByProductDataAccessTests : BaseDataAccessHelperTest<BetListByProductDataAccess>
    {
        private IBalanceUpDataAccess? _balanceUpDataAccess;
        private Fixture _fixture => new Fixture();

        private readonly BalanceUp _balanceUpInfo = new BalanceUp
        {
            BalanceUpDay = DateTime.Now,
            MoveBetsDay = DateTime.Now
        };

        protected override void SetUpDataAccess()
        {
            _balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            _balanceUpDataAccess.GetBalanceUp().Returns(_balanceUpInfo);
            DataAccess = Substitute.ForPartsOf<BetListByProductDataAccess>(_balanceUpDataAccess,
                CurrencyDataAccess);
        }

        [Test]
        public async Task QueryAsync_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            var metaModel = _fixture.Create<BetListByProductMetaModel>();

            // Action
            await DataAccess.QueryAsync(metaModel);

            // Assert
            ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
            ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
            ObjectDb.Received(1).SetParameterValue("CustId", metaModel.CustId);
            ObjectDb.Received(1).SetParameterValue("@Date", metaModel.WinlostDate.Date);
            ObjectDb.Received(1).SetParameterValue("@TransType", metaModel.TransType);
            ObjectDb.Received(1).SetParameterValue("@ProductCode", metaModel.ProductCode);
            ObjectDb.Received(1).SetParameterValue("@AgentList", metaModel.AgentListXml);
            ObjectDb.Received(1).SetParameterValue("@Sort", metaModel.Sort);
            ObjectDb.Received(1).SetParameterValue("@BalanceUpDay", _balanceUpInfo.BalanceUpDay);
            ObjectDb.Received(1).SetParameterValue("@MoveBetsDay", _balanceUpInfo.MoveBetsDay);

            ObjectDb.Received(1).SetParameterValue("@Sort", metaModel.Sort);

            ObjectDb.Received(1).SetParameterValue("@TotalRows", null);
            ObjectDb.Received(1).SetParameterValue("@SumStake", null);
            ObjectDb.Received(1).SetParameterValue("@SumWinLost", null);
            ObjectDb.Received(1).SetParameterValue("@SumLicenseeWL", null);
            ObjectDb.Received(1).SetParameterValue("@SumComm", null);
            ObjectDb.Received(1).SetParameterValue("@SumPlayerComm", null);
        }

        [Test]
        public async Task QueryAsync_WhenCall_CallReadBetListTicket()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            var metaModel = _fixture.Create<BetListByProductMetaModel>();

            // Action
            var result = await DataAccess.QueryAsync(metaModel);

            // Assert
            Assert.That(result.MainTicketList, Is.Empty);
        }

        [Test]
        public async Task QueryAsync_WhenCall_ReadCorrectOutputResult()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            var outPut = new Hashtable
            {
                { "totalrows", _fixture.Create<int>() },
                { "SumStake", _fixture.Create<int>() },
                { "SumWinLost", _fixture.Create<int>() },
                { "SumLicenseeWL", _fixture.Create<int>() },
                { "SumComm", _fixture.Create<int>() },
                { "SumPlayerComm", _fixture.Create<int>() }
            };

            ObjectDb.GetOutputParams().Returns(outPut);

            var metaModel = _fixture.Create<BetListByProductMetaModel>();

            // Action
            var result = await DataAccess.QueryAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalRows, Is.EqualTo(outPut["totalrows"]));
                Assert.That(result.SumStake, Is.EqualTo(outPut["SumStake"]));
                Assert.That(result.SumWinLoss, Is.EqualTo(outPut["SumWinLost"]));
                Assert.That(result.SumLicenseeWL, Is.EqualTo(outPut["SumLicenseeWL"]));
                Assert.That(result.SumComm, Is.EqualTo(outPut["SumComm"]));
                Assert.That(result.SumPlayerComm, Is.EqualTo(outPut["SumPlayerComm"]));
            });
        }
    }
}