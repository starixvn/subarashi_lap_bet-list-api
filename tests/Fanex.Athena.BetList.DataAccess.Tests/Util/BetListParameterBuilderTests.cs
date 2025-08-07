using Fanex.Athena.BetList.DataAccess.Util;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.TestHelper;
using Fanex.Data;
using NSubstitute;

namespace Fanex.Athena.BetList.DataAccess.Tests.Util
{
    public class BetListParameterBuilderTests
    {
        private BetListMetaModel _metaModel;
        private IObjectDb _objectDb;

        [SetUp]
        public void SetUp()
        {
            _metaModel = FakeHelper.Create<BetListMetaModel>();
            _metaModel.ProductCodeList = "SB,NG";

            _objectDb = Substitute.For<IObjectDb>();
        }

        [TearDown]
        public void TearDown()
        {
            _objectDb.Dispose();
            _metaModel = null;
            _objectDb = null;
        }

        [Test]
        public void CreateBetListParameters_CalledByCorectScoreBetlist_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_TotalBets_Sel430";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                AssertPagingParameters(_objectDb, _metaModel);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByCustomerWlBetlist_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Shard_Lrf_BetList_CustomerWL_Mem";
            _metaModel.UspKey = UspKey;
            _metaModel.ProductCodeList = "SB,NG";

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("CustId", _metaModel.CustId);
                _objectDb.Received(1).SetParameterValue("ProductList", "SB,NG");
                AssertMoveBetParams(_objectDb, _metaModel);
                AssertDateRangeParameters(_objectDb, _metaModel);
                AssertPagingParameters(_objectDb, _metaModel);
                AssertSummaryParameters(_objectDb);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByLicenseeWLBetlist_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Shard_Lrf_BetList_LicenseeWL_Mem";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("CustId", _metaModel.CustId);
                AssertDateRangeParameters(_objectDb, _metaModel);
                AssertMoveBetParams(_objectDb, _metaModel);
                _objectDb.Received(1).SetParameterValue("ProductList", "SB,NG");
                AssertPagingParameters(_objectDb, _metaModel);
                AssertSummaryParameters(_objectDb);
                _objectDb.Received(1).SetParameterValue("DrawFilter", _metaModel.DrawFilter);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByRunningBetListByBettype_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_SelRunningByBettype";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("BetType", _metaModel.BetType);
                AssertPagingParameters(_objectDb, _metaModel);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByRunningBetListByMultipleBettype_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_SelRunningByBettypeList";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("BetType", _metaModel.MultipleBettype);
                AssertPagingParameters(_objectDb, _metaModel);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByPlatformWinLossBetList_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_PlatformWinLoss_Mem";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("CustId", _metaModel.CustId);
                AssertDateRangeParameters(_objectDb, _metaModel);
                AssertMoveBetParams(_objectDb, _metaModel);
                _objectDb.Received(1).SetParameterValue(DbColumn.ProductList, "SB,NG");
                AssertPagingParameters(_objectDb, _metaModel);
                AssertSummaryParameters(_objectDb);
                _objectDb.Received(1).SetParameterValue("DrawFilter", _metaModel.DrawFilter);
                _objectDb.Received(1).SetParameterValue("PlatformType", _metaModel.PlatformType);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByRunningBetList_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Shard_Lrf_BetList_SelRunningByDate_Mem";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("CustId", _metaModel.CustId);
                _objectDb.Received(1).SetParameterValue("ProductList", _metaModel.ProductCodeList);
                AssertDateRangeParameters(_objectDb, _metaModel);
                AssertPagingParameters(_objectDb, _metaModel);
            });
        }

        [TestCase(2, 2)]
        [TestCase(null, -1)]
        public void CreateBetListParameters_CalledByMatchRunningBetLitByBettype_ParametersPassedCorrectly(int? liveIndicator, int expectedLiveIndicator)
        {
            // Arrange
            const string UspKey = "Shard_Lrf_BetList_SelRunningByBettypeMatch";

            _metaModel.LiveIndicator = liveIndicator;
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("BetType", _metaModel.BetType);
                _objectDb.Received(1).SetParameterValue("MatchId", _metaModel.MatchId);
                AssertPagingParameters(_objectDb, _metaModel);
                _objectDb.Received(1).SetParameterValue("LiveIndicator", expectedLiveIndicator);
                _objectDb.Received(1).SetParameterValue("BetTeam", _metaModel.BetTeam);
            });
        }

        [TestCase("Test", "Test", 1, 1)]
        [TestCase(null, "", null, -1)]
        public void CreateBetListParameters_CalledByMatchRunningBetLitByMultipleBettypeOfMatch_ParametersPassedCorrectly(
            string multipleBettype,
            string expectedMultipleBettype,
            int? liveIndicator,
            int expectedLiveIndicator)
        {
            // Arrange
            const string UspKey = "Lrf_BetList_SelRunningByMultipleBettypeMatch";

            _metaModel.MultipleBettype = multipleBettype;
            _metaModel.LiveIndicator = liveIndicator;
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("BetType", expectedMultipleBettype);
                _objectDb.Received(1).SetParameterValue("MatchId", _metaModel.MatchId);
                AssertPagingParameters(_objectDb, _metaModel);
                _objectDb.Received(1).SetParameterValue("LiveIndicator", expectedLiveIndicator);
                _objectDb.Received(1).SetParameterValue("BetTeam", _metaModel.BetTeam);
            });
        }

        [TestCase(2, 2)]
        [TestCase(null, -1)]
        public void CreateBetListParameters_CalledByRunningBetListByForeCastMatch_ParametersPassedCorrectly(int? liveIndicator, int expectedLiveIndicator)
        {
            // Arrange
            const string UspKey = "Shard_Lrf_BetList_Forecast_SelByMatch";

            _metaModel.LiveIndicator = liveIndicator;
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("BetType", _metaModel.BetType);
                _objectDb.Received(1).SetParameterValue("MatchId", _metaModel.MatchId);
                AssertPagingParameters(_objectDb, _metaModel);
                _objectDb.Received(1).SetParameterValue("LiveIndicator", expectedLiveIndicator);
                _objectDb.Received(1).SetParameterValue("BetTeam", _metaModel.BetTeam);
                _objectDb.Received(1).SetParameterValue("BetChoice", _metaModel.BetChoice);
                _objectDb.Received(1).SetParameterValue("Handicap", _metaModel.Handicap);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByMicroGamingRngDetail_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_RNG_Detail";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            AssertCasinoDetailParameters(_objectDb, _metaModel);
        }

        [Test]
        public void CreateBetListParameters_CalledByAGCasinoDetail_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LC_Detail";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            AssertCasinoDetailParameters(_objectDb, _metaModel);
        }

        [TestCase(false, 2, 50, 2, 50)]
        public void CreateBetListParameters_CalledByLiveReportNumberGameBetlist_ParametersPassedPagingInfoCorrectly(
            bool isExportExcel,
            int pageIndex,
            int pageSize,
            int expectedIndex,
            int expectedSize)
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LiveReport_SelWinlossNumberGame";
            var metaModel = new LiveReportBetListMetaModel
            {
                IsExportExcel = isExportExcel,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UspKey = UspKey
            };

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("PageSize", expectedSize);
                _objectDb.Received(1).SetParameterValue("PageIndex", expectedIndex);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByBonusDetail_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LicenseeBonus_Detail";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("TransId", _metaModel.TransId);
                _objectDb.Received(1).SetParameterValue("RefCode", _metaModel.RefCode);
                _objectDb.Received(1).SetParameterValue("BaseCurrency", _metaModel.BaseCurrency);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByGodOfFortuneWL_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_GodOfFortuneWL_Mem";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("BaseCurrency", _metaModel.BaseCurrency);
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("CustId", _metaModel.CustId);
                _objectDb.Received(1).SetParameterValue("DrawFilter", _metaModel.DrawFilter);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledBySabaVirtualSportsWL_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_SabaVirtualSports_Mem";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue("BaseCurrency", _metaModel.BaseCurrency);
                _objectDb.Received(1).SetParameterValue("AgentList", _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue("CustId", _metaModel.CustId);
                _objectDb.Received(1).SetParameterValue("DrawFilter", _metaModel.DrawFilter);
                _objectDb.Received(1).SetParameterValue("LeagueId", _metaModel.LeagueId);
                _objectDb.Received(1).SetParameterValue("SportType", _metaModel.SportType);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByMatchWinloss_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_MatchWL";
            _metaModel.UspKey = UspKey;
            _metaModel.ProductCodeList = "SB,NG";

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue(DbColumn.MoveBetsDay, _metaModel.MoveBetsDay);
                _objectDb.Received(1).SetParameterValue(DbColumn.AgentList, _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue(DbColumn.Date, _metaModel.FromDate);
                _objectDb.Received(1).SetParameterValue(DbColumn.MatchId, _metaModel.MatchId);
                _objectDb.Received(1).SetParameterValue(DbColumn.BetType, _metaModel.BetType);
                _objectDb.Received(1).SetParameterValue(DbColumn.ProductList, "SB,NG");
                AssertPagingParameters(_objectDb, _metaModel);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByMatchWinlossOld_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_MatchWL_Old";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue(DbColumn.AgentList, _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue(DbColumn.Date, _metaModel.FromDate);
                _objectDb.Received(1).SetParameterValue(DbColumn.MatchId, _metaModel.MatchId);
                _objectDb.Received(1).SetParameterValue(DbColumn.BetType, _metaModel.BetType);
                AssertPagingParameters(_objectDb, _metaModel);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledByLiveReport_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_LiveReport";
            var metaModel = FakeHelper.Create<LiveReportBetListMetaModel>();
            metaModel.UspKey = UspKey;
            metaModel.ProductCodeList = "SB,NG,BA";

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
                _objectDb.Received(1).SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
                _objectDb.Received(1).SetParameterValue(DbColumn.SportType, metaModel.SportType);
                _objectDb.Received(1).SetParameterValue(DbColumn.ProductList, metaModel.ProductCodeList);
                _objectDb.Received(1).SetParameterValue(DbColumn.MatchId, metaModel.MatchId);
                AssertDateRangeParameters(_objectDb, metaModel);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledCasinoDetail_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "SiteDB_Lrf_BetList_RNG_Detail";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue(DbColumn.AgentList, _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue(nameof(_metaModel.BaseCurrency), _metaModel.BaseCurrency);
                _objectDb.Received(1).SetParameterValue(DbColumn.RefNo, _metaModel.TransId);
                _objectDb.Received(1).SetParameterValue(DbColumn.CustId, _metaModel.CustId);
                _objectDb.Received(1).SetParameterValue(DbColumn.TransType, _metaModel.TransType);
                AssertMoveBetParams(_objectDb, _metaModel);
            });
        }

        [Test]
        public void CreateBetListParameters_CalledJackPotWonDetail_ParametersPassedCorrectly()
        {
            // Arrange
            const string UspKey = "Lrf_BetList_Jackpot_Detail";
            _metaModel.UspKey = UspKey;

            // Action
            BetListParameterBuilder.CreateBetListParameters(_objectDb, _metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _objectDb.Received(1).SetParameterValue(DbColumn.AgentList, _metaModel.AgentListXml);
                _objectDb.Received(1).SetParameterValue(DbColumn.BaseCurrency, _metaModel.BaseCurrency);
                _objectDb.Received(1).SetParameterValue(DbColumn.TransId, _metaModel.TransId);
                _objectDb.Received(1).SetParameterValue(DbColumn.CustId, _metaModel.CustId);
                _objectDb.Received(1).SetParameterValue(DbColumn.ExcludeBettrans, _metaModel.UseShardDB);
            });
        }

        private static void AssertSummaryParameters(IObjectDb objectDb)
        {
            var summaryParams = new[] { "SumWinLost", "SumLicenseeWL", "SumComm", "SumPlayerComm" };

            foreach (var param in summaryParams)
            {
                objectDb.Received(1).SetParameterValue(param, null);
            }
        }

        private static void AssertPagingParameters(IObjectDb objectDb, BetListMetaModel metaModel)
        {
            objectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
            objectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
            objectDb.Received(1).SetParameterValue("TotalRows", null);
        }

        private static void AssertDateRangeParameters(IObjectDb objectDb, BetListMetaModel metaModel)
        {
            objectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
            objectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
        }

        private static void AssertMoveBetParams(IObjectDb objectDb, BetListMetaModel metaModel)
        {
            objectDb.Received(1).SetParameterValue("MoveBetsDay", metaModel.MoveBetsDay);
            objectDb.Received(1).SetParameterValue("BalanceUpDay", metaModel.BalanceUpDay);
        }

        private static void AssertDailyStatementBetList(IObjectDb objectDb, BetListMetaModel metaModel)
        {
            objectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
            objectDb.Received(1).SetParameterValue("CustId", metaModel.CustId);
            objectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
            objectDb.Received(1).SetParameterValue("TransType", metaModel.TransType);
            AssertPagingParameters(objectDb, metaModel);
            AssertSummaryParameters(objectDb);
        }

        private static void AssertCasinoDetailParameters(IObjectDb objectDb, BetListMetaModel metaModel)
        {
            objectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
            objectDb.Received(1).SetParameterValue("RefNo", metaModel.TransId);
            objectDb.Received(1).SetParameterValue("CustId", metaModel.CustId);
            objectDb.Received(1).SetParameterValue("TransType", metaModel.TransType);
        }
    }
}