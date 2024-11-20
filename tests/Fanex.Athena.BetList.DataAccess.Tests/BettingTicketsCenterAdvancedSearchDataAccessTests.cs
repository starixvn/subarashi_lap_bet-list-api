using AutoFixture;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.TestHelper;
using NSubstitute;
using System.Collections;

namespace Fanex.Athena.BetList.DataAccess.Tests
{
    public class BettingTicketsCenterAdvancedSearchDataAccessTest : BaseDataAccessHelperTest<BettingTicketCenterAdvancedSearchDataAccess>
    {
        private const string UspKey = "Lrf_BetList_Licensee";
        private const string TotalRowUspKey = "Lrf_BetList_Licensee_TotalRows";

        private static readonly BalanceUp BalanceUp = new BalanceUp
        {
            MoveBetsDay = new DateTime(2016, 1, 1),
            BalanceUpDay = new DateTime(2016, 2, 1)
        };

        private readonly ITempFileService _tempFileService = Substitute.For<ITempFileService>();
        private Fixture _fixture => new Fixture();

        protected override void SetUpDataAccess()
        {
            var balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
            balanceUpDataAccess.GetBalanceUp().Returns(BalanceUp);
            DataAccess = Substitute.ForPartsOf<BettingTicketCenterAdvancedSearchDataAccess>(
                balanceUpDataAccess,
                _tempFileService, CurrencyDataAccess);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            _tempFileService.ClearReceivedCalls();
        }

        [Test]
        public async Task GetBetListForMultipleCustomersAsync_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
            metaModel.IsExport = false;

            // Action
            await DataAccess.GetBetListForMultipleCustomersAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
                ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
                ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
                ObjectDb.Received(1).SetParameterValue("SportType", metaModel.SportId);
                ObjectDb.Received(1).SetParameterValue("BetType", metaModel.BetTypeId);
                ObjectDb.Received(1).SetParameterValue("LeagueId", metaModel.LeagueId);
                ObjectDb.Received(1).SetParameterValue("MatchId", metaModel.MatchId);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
                ObjectDb.Received(1).SetParameterValue("StatusIdList", metaModel.Status);
                ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
                ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
                ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
                ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
                ObjectDb.Received(1).SetParameterValue("Product", metaModel.ProductCodeList);

                ObjectDb.Received(1).SetParameterValue("TotalRows", null);
                ObjectDb.Received(1).SetParameterValue("SumStake", null);
                ObjectDb.Received(1).SetParameterValue("SumWinLost", null);
                ObjectDb.Received(1).SetParameterValue("SumLicenseeWL", null);
                ObjectDb.Received(1).SetParameterValue("SumComm", null);
                ObjectDb.Received(1).SetParameterValue("SumPlayerComm", null);
            });
        }

        [Test]
        public async Task GetTotalRowForMultipleCustomers_WhenCall_ParametersPassedCorrectly()
        {
            // Arrange
            var totalRow = _fixture.Create<int>();
            ObjectDb.GetOutputParams().Returns(new Hashtable { { "TotalRows", totalRow } });

            var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();

            // Action
            var result = await DataAccess.GetTotalRowForMultipleCustomersAsync(metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
                ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
                ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
                ObjectDb.Received(1).SetParameterValue("SportType", metaModel.SportId);
                ObjectDb.Received(1).SetParameterValue("BetType", metaModel.BetTypeId);
                ObjectDb.Received(1).SetParameterValue("LeagueId", metaModel.LeagueId);
                ObjectDb.Received(1).SetParameterValue("MatchId", metaModel.MatchId);
                ObjectDb.Received(1).SetParameterValue("StatusIdList", metaModel.Status);
                ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
                ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
                ObjectDb.Received(1).SetParameterValue("Product", metaModel.ProductCodeList);

                ObjectDb.Received(1).SetParameterValue("TotalRows", null);

                Assert.That(result, Is.EqualTo(totalRow));
            });
        }

        [Test]
        public async Task GetBetListForMultipleCustomersAsync_ExportExcel_ParametersPassedCorrectly()
        {
            // Arrange
            const int RowPerGroup = 1000;

            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
            metaModel.IsExport = true;
            metaModel.SortBy = string.Empty;

            // Action
            await DataAccess.GetBetListForMultipleCustomersAsync(metaModel, RowPerGroup);

            // Assert
            Assert.Multiple(() =>
            {
                ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
                ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
                ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
                ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
                ObjectDb.Received(1).SetParameterValue("SportType", metaModel.SportId);
                ObjectDb.Received(1).SetParameterValue("BetType", metaModel.BetTypeId);
                ObjectDb.Received(1).SetParameterValue("LeagueId", metaModel.LeagueId);
                ObjectDb.Received(1).SetParameterValue("MatchId", metaModel.MatchId);
                ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
                ObjectDb.Received(1).SetParameterValue("StatusIdList", metaModel.Status);
                ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
                ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
                ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
                ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
                ObjectDb.Received(1).SetParameterValue("Product", metaModel.ProductCodeList);

                ObjectDb.Received(1).SetParameterValue("TotalRows", null);
                ObjectDb.Received(1).SetParameterValue("SumStake", null);
                ObjectDb.Received(1).SetParameterValue("SumWinLost", null);
                ObjectDb.Received(1).SetParameterValue("SumLicenseeWL", null);
                ObjectDb.Received(1).SetParameterValue("SumComm", null);
                ObjectDb.Received(1).SetParameterValue("SumPlayerComm", null);

                ObjectDb.DidNotReceive().SetParameterValue("Sort", Arg.Any<object>());
            });
        }

        [Test]
        public async Task GetBetListForMultipleCustomersAsync_ExportExcel_HaveOneSheet_SaveOneFileToMemory()
        {
            // Arrange
            const int RowPerGroup = 1000;

            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(true, false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
            metaModel.IsExport = true;

            var ticket = _fixture.Create<LrfTicket>();
            MapReaderData(ticket);

            // Action
            await DataAccess.GetBetListForMultipleCustomersAsync(metaModel, RowPerGroup);

            // Assert
            _tempFileService.Received(1).Save(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task GetBetListForMultipleCustomersAsync_ExportExcel_HaveTwoSheets_SaveTwoFilesToMemory()
        {
            // Arrange
            const int RowPerGroup = 1;
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            Reader.Read().Returns(true, true, false);
            ObjectDb.GetOutputParams().Returns(new Hashtable());

            var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
            metaModel.IsExport = true;

            var ticket = _fixture.Create<LrfTicket>();
            MapReaderData(ticket);

            // Action
            await DataAccess.GetBetListForMultipleCustomersAsync(metaModel, RowPerGroup);

            // Assert
            _tempFileService.Received(2).Save(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
