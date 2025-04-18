namespace Fanex.Athena.BetList.DataAccess.Tests;

using System.Collections;
using AutoFixture;
using Common.DataAccess.Interfaces;
using Common.Model.DTO;
using Common.Utility.Services.Interfaces;
using Model.DTO;
using Model.MetaModel;
using NSubstitute;
using TestHelper;

public class BettingTicketsCenterAdvancedSearchDataAccessTest : BaseDataAccessHelperTest<BettingTicketCenterAdvancedSearchDataAccess>
{
    private const string UspKey = "Lrf_BetList_Licensee";
    private const string TotalRowUspKey = "Lrf_BetList_Licensee_TotalRows";

    private static readonly BalanceUp BalanceUp = new()
    {
        MoveBetsDay = new DateTime(2016, 1, 1),
        BalanceUpDay = new DateTime(2016, 2, 1)
    };

    private readonly ITempFileService _tempFileService = Substitute.For<ITempFileService>();
    private Fixture _fixture => new();

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
        var cancellationToken = new CancellationToken();
        ObjectDb.ExecuteReaderAsync().Returns(Reader);
        Reader.Read().Returns(false);
        ObjectDb.GetOutputParams().Returns(new Hashtable());

        var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
        metaModel.IsExport = false;

        // Action
        await DataAccess.GetBetListForMultipleCustomersAsync(metaModel, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
            ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
            ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
            ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
            ObjectDb.Received(1).SetParameterValue("SportTypeList", metaModel.SportListXML);
            ObjectDb.Received(1).SetParameterValue("BetTypeList", metaModel.BetTypeIds);
            ObjectDb.Received(1).SetParameterValue("CashOutMode", metaModel.CashOutMode);
            ObjectDb.Received(1).SetParameterValue("LeagueId", metaModel.LeagueId);
            ObjectDb.Received(1).SetParameterValue("MatchId", metaModel.MatchId);
            ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            ObjectDb.Received(1).SetParameterValue("StatusIdList", metaModel.Status);
            ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
            ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
            ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
            ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
            ObjectDb.Received(1).SetParameterValue("ProductList", metaModel.ProductCodeList);

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
        var cancellationToken = new CancellationToken();
        var totalRow = _fixture.Create<int>();
        ObjectDb.GetOutputParams().Returns(new Hashtable { { "TotalRows", totalRow } });

        var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
        var maxTotalRows = FakeHelper.Create<int>();

        // Action
        var result = await DataAccess.GetTotalRowForMultipleCustomersAsync(metaModel, maxTotalRows, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
            ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
            ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
            ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
            ObjectDb.Received(1).SetParameterValue("SportTypeList", metaModel.SportListXML);
            ObjectDb.Received(1).SetParameterValue("BetTypeList", metaModel.BetTypeIds);
            ObjectDb.Received(1).SetParameterValue("CashOutMode", metaModel.CashOutMode);
            ObjectDb.Received(1).SetParameterValue("LeagueId", metaModel.LeagueId);
            ObjectDb.Received(1).SetParameterValue("MatchId", metaModel.MatchId);
            ObjectDb.Received(1).SetParameterValue("StatusIdList", metaModel.Status);
            ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
            ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
            ObjectDb.Received(1).SetParameterValue("ProductList", metaModel.ProductCodeList);
            ObjectDb.Received(1).SetParameterValue("MaxTotalRows", maxTotalRows);

            ObjectDb.Received(1).SetParameterValue("TotalRows", null);

            Assert.That(result, Is.EqualTo(totalRow));
        });
    }

    [Test]
    public async Task GetBetListForMultipleCustomersAsync_ExportExcel_ParametersPassedCorrectly()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        const int RowPerGroup = 1000;

        ObjectDb.ExecuteReaderAsync().Returns(Reader);
        Reader.Read().Returns(false);
        ObjectDb.GetOutputParams().Returns(new Hashtable());

        var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
        metaModel.IsExport = true;
        metaModel.SortBy = string.Empty;

        // Action
        await DataAccess.GetBetListForMultipleCustomersAsync(metaModel, RowPerGroup, cancellationToken);

        // Assert
        Assert.Multiple(() =>
        {
            ObjectDb.Received(1).SetParameterValue("AgentList", metaModel.AgentListXml);
            ObjectDb.Received(1).SetParameterValue("FromDate", metaModel.FromDate);
            ObjectDb.Received(1).SetParameterValue("ToDate", metaModel.ToDate);
            ObjectDb.Received(1).SetParameterValue("MoveBetsDay", BalanceUp.MoveBetsDay);
            ObjectDb.Received(1).SetParameterValue("SportTypeList", metaModel.SportListXML);
            ObjectDb.Received(1).SetParameterValue("BetTypeList", metaModel.BetTypeIds);
            ObjectDb.Received(1).SetParameterValue("CashOutMode", metaModel.CashOutMode);
            ObjectDb.Received(1).SetParameterValue("LeagueId", metaModel.LeagueId);
            ObjectDb.Received(1).SetParameterValue("MatchId", metaModel.MatchId);
            ObjectDb.Received(1).SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            ObjectDb.Received(1).SetParameterValue("StatusIdList", metaModel.Status);
            ObjectDb.Received(1).SetParameterValue("UserName", metaModel.UserName);
            ObjectDb.Received(1).SetParameterValue("LiveIndicator", metaModel.LiveStatus);
            ObjectDb.Received(1).SetParameterValue("PageSize", metaModel.PageSize);
            ObjectDb.Received(1).SetParameterValue("PageIndex", metaModel.PageIndex);
            ObjectDb.Received(1).SetParameterValue("ProductList", metaModel.ProductCodeList);

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
        var cancellationToken = new CancellationToken();

        ObjectDb.ExecuteReaderAsync().Returns(Reader);
        Reader.Read().Returns(true, false);
        ObjectDb.GetOutputParams().Returns(new Hashtable());

        var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
        metaModel.IsExport = true;

        var ticket = _fixture.Create<LrfTicket>();
        MapReaderData(ticket);

        // Action
        await DataAccess.GetBetListForMultipleCustomersAsync(metaModel, RowPerGroup, cancellationToken);

        // Assert
        _tempFileService.Received(1).Save(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task GetBetListForMultipleCustomersAsync_ExportExcel_HaveTwoSheets_SaveTwoFilesToMemory()
    {
        // Arrange
        const int RowPerGroup = 1;
        var cancellationToken = new CancellationToken();
        ObjectDb.ExecuteReaderAsync().Returns(Reader);
        Reader.Read().Returns(true, true, false);
        ObjectDb.GetOutputParams().Returns(new Hashtable());

        var metaModel = _fixture.Create<AdvancedSearchBetListMetaModel>();
        metaModel.IsExport = true;

        var ticket = _fixture.Create<LrfTicket>();
        MapReaderData(ticket);

        // Action
        await DataAccess.GetBetListForMultipleCustomersAsync(metaModel, RowPerGroup, cancellationToken);

        // Assert
        _tempFileService.Received(2).Save(Arg.Any<string>(), Arg.Any<string>());
    }

    protected override void SetUpDataAccess()
    {
        var balanceUpDataAccess = Substitute.For<IBalanceUpDataAccess>();
        balanceUpDataAccess.GetBalanceUp().Returns(BalanceUp);
        DataAccess = Substitute.ForPartsOf<BettingTicketCenterAdvancedSearchDataAccess>(
            balanceUpDataAccess,
            _tempFileService, CurrencyDataAccess);
    }
}