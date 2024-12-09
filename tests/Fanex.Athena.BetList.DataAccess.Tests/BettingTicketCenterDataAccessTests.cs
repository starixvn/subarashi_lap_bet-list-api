using AutoFixture;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.TestHelper;
using NSubstitute;
using System.Collections;

namespace Fanex.Athena.BetList.DataAccess.Tests
{
    public class BettingTicketCenterDataAccessTests : BaseDataAccessHelperTest<BettingTicketCenterDataAccess>
    {
        private const string BetListByRefnoUspKey = "Lrf_BetList_RefNo_Mem";
        private const string RunningBetListUspKey = "Shard_Lrf_BetList_Running";
        private const string BetListBySingleCustomerUspKey = "Lrf_BetList_Mem";

        private static readonly BalanceUp BalanceUp = new BalanceUp
        {
            MoveBetsDay = new DateTime(2016, 1, 1),
            BalanceUpDay = new DateTime(2016, 2, 1)
        };

        private Fixture _fixture => new Fixture();

        protected override void SetUpDataAccess()
        {
            var balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            balanceUpDataAccess.GetBalanceUp().Returns(BalanceUp);
            var shardInfoAccessor = Substitute.For<IShardInfoAccessor>();
            shardInfoAccessor.UseShardDB.Returns(false);
            DataAccess = Substitute.ForPartsOf<BettingTicketCenterDataAccess>(
                 balanceUpDataAccess, CurrencyDataAccess, shardInfoAccessor);
        }

        [Test]
        public async Task GetBetListByRefnoAsync_WhenCall_ParameterIsPassesCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            long refno = 123456789;

            var metaModel = new RefNoBetListMetaModel
            {
                RefNo = refno.ToString()
            };

            // Action
            await DataAccess.GetBetListByRefnoAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
                ObjectDb.Received(1).SetParameterValue("RefNo", refno);
                ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
                ObjectDb.Received(1).SetParameterValue("BalanceUpDay", BalanceUp.BalanceUpDay);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            });
        }

        [TestCase("", "", false)]
        [TestCase("P1,SB,P2", "P1,SB,P2", false)]
        [TestCase("BA,P1,P2", "P1,P2", true)]
        [TestCase("P1,BA,P2,SB", "P1,P2,SB", true)]
        public async Task GetBetListForSingleCustomerAsync_WhenCall_ParametersPassedCorrectly(string productCode, string productList, bool showBA)
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = new SingleCustomersBetListMetaModel { ProductCodeList = productCode };

            // Action
            await DataAccess.GetBetListForSingleCustomerAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("showBA", showBA);
                ObjectDb.Received(1).SetParameterValue("ProductList", productList);
                ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
                ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
                ObjectDb.Received(1).SetParameterValue("StatusIdList", metaModel.Status);
                ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
                ObjectDb.Received(1).SetParameterValue("Platform", metaModel.Platform);
                ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
                ObjectDb.Received(1).SetParameterValue("BalanceUpDay", BalanceUp.BalanceUpDay);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
                ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
                ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
                ObjectDb.Received(1).SetParameterValue("TotalRows", null);
                ObjectDb.Received(1).SetParameterValue("SumStake", null);
                ObjectDb.Received(1).SetParameterValue("SumWinLost", null);
                ObjectDb.Received(1).SetParameterValue("SumLicenseeWL", null);
                ObjectDb.Received(1).SetParameterValue("SumComm", null);
                ObjectDb.Received(1).SetParameterValue("SumPlayerComm", null);
            });
        }

        [Test]
        public async Task GetBetListForSingleCustomerAsync_Export_PagingParametersPassedCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = new SingleCustomersBetListMetaModel { ProductCodeList = string.Empty, IsExport = true };

            // Action
            await DataAccess.GetBetListForSingleCustomerAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("PageSize", int.MaxValue - 1);
                ObjectDb.Received(1).SetParameterValue("PageIndex", 1);
            });
        }

        [Test]
        public async Task GetBetListForSingleCustomerAsync_Sort_SortParametersPassedCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = new SingleCustomersBetListMetaModel { ProductCodeList = string.Empty, SortBy = "Colum1", SortOrder = "asc" };

            // Action
            await DataAccess.GetBetListForSingleCustomerAsync(metaModel);

            // Assert
            ObjectDb.Received(1).SetParameterValue("Sort", metaModel.SortBy + " " + metaModel.SortOrder);
        }

        [Test]
        public async Task GetBetListForSingleCustomerAsync_WhenCall_ReturnTotalDataCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);

            var outPut = new Hashtable
            {
                { "SumStake", _fixture.Create<decimal>() },
                { "TotalRows", _fixture.Create<int>() },
                { "SumWinLost", _fixture.Create<decimal>() },
                { "SumLicenseeWL", _fixture.Create<int>() },
                { "SumComm", _fixture.Create<decimal>() },
                { "SumPlayerComm", _fixture.Create<int>() }
            };
            ObjectDb.GetOutputParams().Returns(outPut);

            var metaModel = new SingleCustomersBetListMetaModel { ProductCodeList = string.Empty };

            // Action
            var result = await DataAccess.GetBetListForSingleCustomerAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SumStake, Is.EqualTo(outPut["SumStake"]));
                Assert.That(result.TotalRows, Is.EqualTo(outPut["TotalRows"]));
                Assert.That(result.SumWinLoss, Is.EqualTo(outPut["SumWinLost"]));
                Assert.That(result.SumLicenseeWL, Is.EqualTo(outPut["SumLicenseeWL"]));
                Assert.That(result.SumComm, Is.EqualTo(outPut["SumComm"]));
                Assert.That(result.SumPlayerComm, Is.EqualTo(outPut["SumPlayerComm"]));
            });
        }

        [TestCase("", "", false)]
        [TestCase("P1,SB,P2", "P1,SB,P2", false)]
        [TestCase("BA,P1,P2", "P1,P2", true)]
        [TestCase("P1,BA,P2,SB", "P1,P2,SB", true)]
        public async Task GetRunningBetListAsync_WhenCall_ParametersPassedCorrectly(string productCode, string productList, bool showBA)
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = new RunningBetListMetaModel { ProductCodeList = productCode };

            // Action
            await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
                ObjectDb.Received(1).SetParameterValue("ProductList", productList);
                ObjectDb.Received(1).SetParameterValue("showBA", showBA);
                ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
                ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
                ObjectDb.Received(1).SetParameterValue("TotalRows", null);
                ObjectDb.Received(1).SetParameterValue("SumStake", null);
            });
        }

        [Test]
        public async Task GetRunningBetListAsync_Export_PagingParametersPassedCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = new RunningBetListMetaModel { ProductCodeList = string.Empty, IsExport = true };

            // Action
            await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("PageSize", int.MaxValue - 1);
                ObjectDb.Received(1).SetParameterValue("PageIndex", 1);
            });
        }

        [Test]
        public async Task GetRunningBetListAsync_Sort_SortParametersPassedCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = new RunningBetListMetaModel { ProductCodeList = string.Empty, SortBy = "Colum1", SortOrder = "asc" };

            // Action
            await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            ObjectDb.Received(1).SetParameterValue("Sort", metaModel.SortBy + " " + metaModel.SortOrder);
        }

        [Test]
        public async Task GetRunningBetListAsync_WhenCall_ReturnTotalDataCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);

            var outPut = new Hashtable
            {
                { "SumStake", _fixture.Create<decimal>() },
                { "TotalRows", _fixture.Create<int>() }
            };
            ObjectDb.GetOutputParams().Returns(outPut);

            var metaModel = new RunningBetListMetaModel { ProductCodeList = string.Empty };

            // Action
            var result = await DataAccess.GetRunningBetListAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SumStake, Is.EqualTo(outPut["SumStake"]));
                Assert.That(result.TotalRows, Is.EqualTo(outPut["TotalRows"]));
            });
        }

        [Test]
        public async Task GetBetListByTxIDAsync_WhenCall_ParameterIsPassesCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);

            var metaModel = _fixture.Create<TxIDBetListMetaModel>();

            // Action
            await DataAccess.GetBetListByTxIDAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("UserName", SqlHelper.ToSqlTextSearch(metaModel.UserName));
                ObjectDb.Received(1).SetParameterValue("TxId", metaModel.TxID);
                ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
                ObjectDb.Received(1).SetParameterValue("BalanceUpDay", BalanceUp.BalanceUpDay);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            });
        }

        [Test]
        public async Task GetBetListForBookedBetAsync_WhenNotSearchUserNameAndSearchTextNotNumber_NotPassAnyParamToDB()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);

            var outPut = new Hashtable
            {
                { DbColumn.SumStake, _fixture.Create<decimal>() },
                { DbColumn.TotalRows, _fixture.Create<int>() },
            };
            ObjectDb.GetOutputParams().Returns(outPut);
            var metaModel = _fixture.Create<BookedBetBetListMetaModel>();
            metaModel.SearchText = "abc";
            metaModel.SearchOption = 1;

            // Action
            var result = await DataAccess.GetBetListForBookedBetAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.DidNotReceiveWithAnyArgs().SetParameterValue(Arg.Any<string>(), Arg.Any<object>());
            });
        }


        [Test]
        public async Task GetBetListForBookedBetAsync_WhenSearchUserName_ReturnCorrectValue()
        {
            // Arrange
            const int UserNameOption = 3;
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);

            var outPut = new Hashtable
            {
                { DbColumn.SumStake, _fixture.Create<decimal>() },
                { DbColumn.TotalRows, _fixture.Create<int>() },
            };
            ObjectDb.GetOutputParams().Returns(outPut);
            var metaModel = _fixture.Create<BookedBetBetListMetaModel>();
            metaModel.SearchText = "abc";
            metaModel.SearchOption = UserNameOption;

            // Action
            var result = await DataAccess.GetBetListForBookedBetAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SumStake, Is.EqualTo(outPut[DbColumn.SumStake]));
                Assert.That(result.TotalRows, Is.EqualTo(outPut[DbColumn.TotalRows]));
            });
        }

        [Test]
        public async Task GetBetListForBookedBetAsync_WhenSearchTextIsNumber_ReturnCorrectValue()
        {
            // Arrange
            const int NotUserNameOption = 1;
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);

            var outPut = new Hashtable
            {
                { DbColumn.SumStake, _fixture.Create<decimal>() },
                { DbColumn.TotalRows, _fixture.Create<int>() },
            };
            ObjectDb.GetOutputParams().Returns(outPut);
            var metaModel = _fixture.Create<BookedBetBetListMetaModel>();
            metaModel.SearchText = "10000";
            metaModel.SearchOption = NotUserNameOption;

            // Action
            var result = await DataAccess.GetBetListForBookedBetAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.SumStake, Is.EqualTo(outPut[DbColumn.SumStake]));
                Assert.That(result.TotalRows, Is.EqualTo(outPut[DbColumn.TotalRows]));
            });
        }
    }
}
