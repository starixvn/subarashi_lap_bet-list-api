namespace Fanex.Athena.BetList.Excel.Tests
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using BetListNPOI.SS.UserModel;
    using Common.Constant;
    using Common.Utility.Services.Interfaces;
    using Fanex.BetList.Core;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.TicketHelper;
    using NSubstitute;
    using Resources;
    using Service.Extensions;
    using Service.Interfaces;
    using TestClasses;
    using TestHelper.Extensions;

    public class BetListExcelTests
    {
        private const int LicenseeId = 9999;
        private IAppSettings _appSettings;
        private ITicketHelper _ticketHelper;
        private IBetListExcelBuilder _builder;
        private IIPService _ipService;
        private ICacheRef _cacheRef;

        [SetUp]
        public void Setup()
        {
            _appSettings = Substitute.For<IAppSettings>();
            _ticketHelper = Substitute.For<ITicketHelper>();
            _builder = Substitute.For<IBetListExcelBuilder>();
            _ipService = Substitute.For<IIPService>();
            _cacheRef = Substitute.For<ICacheRef>();
            BetListManager.RegisterRemotingRef(_cacheRef);
        }

        [TearDown]
        public void TearDown()
        {
            _appSettings = null;
            _ticketHelper = null;
            _builder = null;
            _ipService = null;
            _cacheRef = null;
        }

        [Test]
        public void Export_WhenCall_CallRelevantMethods()
        {
            // Arrange
            const string Title = "aaaa";
            const int startIndex = 0;
            var betListExcel = CreateBetListExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.GetBetList().Returns(workbook);
            var ticketResult = new TicketResult();
            betListExcel.When(x => x.InvokeProtectedMethod("Build", _builder, ticketResult, Title, startIndex)).DoNotCallBase();
            betListExcel.When(x => x.InvokeProtectedMethod("ProcessUi", workbook)).DoNotCallBase();
            betListExcel.When(x => x.InvokeProtectedMethod("FixBugIOSMobile", workbook)).DoNotCallBase();

            // Action
            var result = betListExcel.Export(ticketResult, Title, startIndex);

            // Assert
            Assert.Multiple(() =>
            {
                betListExcel.Received(1).InvokeProtectedMethod("Build", _builder, ticketResult, Title, startIndex);
                betListExcel.Received(1).InvokeProtectedMethod("ProcessUi", workbook);
                Assert.That(result, Is.EqualTo(workbook));
            });
        }

        [Test]
        public void Build_NoData_CallRelevantMethods()
        {
            // Arrange
            const string Title = "aaaa";
            var betListExcel = CreateBetListExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.GetBetList().Returns(workbook);
            var ticketResult = new TicketResult();

            // Action
            betListExcel.ProtectedBuild(_builder, ticketResult, Title);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(1).BuildWorksheet(ViewResources.BetList, 11);
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == ViewResources.NoInfo), Arg.Any<ICellStyle>());
            });
        }

        [Test]
        public void Build_HasData_CallAssignSheetsData()
        {
            // Arrange
            const string Title = "aaaa";
            var betListExcel = CreateBetListExcel();
            var ticketResult = new TicketResult();
            var ticket = new LrfTicket();
            ticketResult.MainTicketList.Add(ticket);
            betListExcel.When(x => x.InvokeProtectedMethod("AssignSheetsData", _builder, ticketResult, Title, Arg.Any<int>())).DoNotCallBase();

            // Action
            betListExcel.ProtectedBuild(_builder, ticketResult, Title);

            // Assert
            betListExcel.Received(1).InvokeProtectedMethod("AssignSheetsData",_builder, ticketResult, Title, Arg.Any<int>());
        }

        [Test]
        public void ProcessUi_WhenCall_CallRelevantMethods()
        {
            // Arrange
            const int HiddenColumnIndex = 100;
            var betListExcel = CreateBetListExcel(hiddenColumns: new[] { HiddenColumnIndex });
            var sheet = Substitute.For<ISheet>();
            var workbook = Substitute.For<IWorkbook>();
            workbook.NumberOfSheets.Returns(1);
            workbook.GetSheetAt(0).Returns(sheet);
            _builder.GetBetList().Returns(workbook);

            // Action
            betListExcel.ProtectedProcessUi(workbook);

            // Assert
            Assert.Multiple(() =>
            {
                sheet.Received(1).SetColumnHidden(HiddenColumnIndex, true);
            });
        }

        [Test]
        public void ProcessUi_NotHaveHidenColumns_DoNothing()
        {
            // Arrange
            var betListExcel = CreateBetListExcel(hiddenColumns: null);
            var sheet = Substitute.For<ISheet>();
            var workbook = Substitute.For<IWorkbook>();
            workbook.GetSheetAt(0).Returns(sheet);

            // Action
            betListExcel.ProtectedProcessUi(workbook);

            // Assert
            Assert.Multiple(() =>
            {
                sheet.DidNotReceiveWithAnyArgs().SetColumnHidden(Arg.Any<int>(), Arg.Any<bool>());
            });
        }

        [Test]
        public void ProcessUi_FakeLicensee_HideSystemIdColumn()
        {
            // Arrange
            const int HiddenColumnIndex = 100;
            const int ColumnSystemIdIndex = 2;
            _appSettings.FakeLicenseeList.Returns([LicenseeId]);
            var betListExcel = CreateBetListExcel(hiddenColumns: [HiddenColumnIndex]);
            var sheet = Substitute.For<ISheet>();
            var workbook = Substitute.For<IWorkbook>();
            workbook.NumberOfSheets.Returns(1);
            workbook.GetSheetAt(0).Returns(sheet);
            _builder.GetBetList().Returns(workbook);

            // Action
            betListExcel.ProtectedProcessUi(workbook);

            // Assert
            Assert.Multiple(() =>
            {
                sheet.Received(1).SetColumnHidden(HiddenColumnIndex, true);
                sheet.Received(1).SetColumnHidden(ColumnSystemIdIndex, true);
            });
        }

        [Test]
        public void AssignSheetsData_ShowSumStake_AddFooterSumStake()
        {
            // Arrange
            const string Title = "aaaa";
            var betListExcel = CreateBetListExcel(new BetListShowingOption{IsShowSumStake = true});
            betListExcel.When(x => x.InvokeProtectedMethod("BuildTicket", Arg.Any<IBetListExcelBuilder>(), Arg.Any<TicketResult>(), Arg.Any<LrfTicket>())).DoNotCallBase();
            var ticketResult = new TicketResult();
            ticketResult.MainTicketList.Add(new LrfTicket { Stake = 99 });
            ticketResult.MainTicketList.Add(new LrfTicket { Stake = 100 });

            // Action
            betListExcel.ProtectedAssignSheetsData(_builder, ticketResult, Title, 0);

            // Assert
            _builder.Received(1).AddCell(
                    Arg.Is<IRichTextString>(x => x.String == "199.00"),
                    Arg.Any<ICellStyle>());
        }

        [Test]
        public void AssignSheetsData_ShowWinLoss_AddFooterWinLoss()
        {
            // Arrange
            const string Title = "aaaa";
            var betListExcel = CreateBetListExcel(showingOption: new BetListShowingOption { GrandTotalVisible = true, CustomerCommVisible = true, IsShowIp = true});
            betListExcel.When(x => x.InvokeProtectedMethod("BuildTicket", Arg.Any<IBetListExcelBuilder>(), Arg.Any<TicketResult>(), Arg.Any<LrfTicket>())).DoNotCallBase();
            var ticketResult = new TicketResult
            {
                SumWinLoss = 100,
                SumPlayerComm = 0.88m,
                SumLicenseeWL = 200,
                SumComm = 0.99m,
                SumBetEffective = 999
            };
            ticketResult.MainTicketList.Add(new LrfTicket { Stake = 0 });

            // Action
            betListExcel.ProtectedAssignSheetsData(_builder, ticketResult, Title, 0);

            // Assert
            Assert.Multiple(() =>
            {
                // Sum customerWL
                _builder.Received(1).AddCell(
                        Arg.Is<IRichTextString>(x => x.String == "100.88"),
                        Arg.Any<ICellStyle>());

                // Sum LicenseeWL
                _builder.Received(1).AddCell(
                        Arg.Is<IRichTextString>(x => x.String == "200.99"),
                        Arg.Any<ICellStyle>());

                // SumBetEffective
                _builder.Received(1).AddCell(
                        Arg.Is<IRichTextString>(x => x.String == "999.00"),
                        Arg.Any<ICellStyle>());
            });
        }

        [Test]
        public void AssignSheetsData_NotShowSumStakeAndWinLoss_AddFooterWinLoss()
        {
            // Arrange
            const string Title = "aaaa";
            var betListExcel = CreateBetListExcel(showingOption: new BetListShowingOption { GrandTotalVisible = false });
            betListExcel.When(x => x.InvokeProtectedMethod("BuildTicket", Arg.Any<IBetListExcelBuilder>(), Arg.Any<TicketResult>(), Arg.Any<LrfTicket>())).DoNotCallBase();
            var ticketResult = CreateTicketResult();

            // Action
            betListExcel.ProtectedAssignSheetsData(_builder, ticketResult, Title, 0);

            // Assert
            Assert.Multiple(() =>
            {
                // Sum customerWL
                _builder.DidNotReceive().AddCell(
                        Arg.Is<IRichTextString>(x => x.String == "100.88"),
                        Arg.Any<ICellStyle>());

                // Sum LicenseeWL
                _builder.DidNotReceive().AddCell(
                        Arg.Is<IRichTextString>(x => x.String == "200.99"),
                        Arg.Any<ICellStyle>());

                // SumBetEffective
                _builder.DidNotReceive().AddCell(
                        Arg.Is<IRichTextString>(x => x.String == "999.00"),
                        Arg.Any<ICellStyle>());
            });
        }

        [Test]
        public void AssignSheetsData_NotShowCustomerComm_AddCorrectHeader()
        {
            // Arrange
            const string Title = "aaaa";
            var betListExcel = CreateBetListExcel(showingOption: new BetListShowingOption { GrandTotalVisible = false, CustomerCommVisible = false });
            betListExcel.When(x => x.InvokeProtectedMethod("BuildTicket", Arg.Any<IBetListExcelBuilder>(), Arg.Any<TicketResult>(), Arg.Any<LrfTicket>())).DoNotCallBase();
            var ticketResult = CreateTicketResult();

            // Action
            betListExcel.ProtectedAssignSheetsData(_builder, ticketResult, Title, 0);

            // Assert
            _builder.Received(1).AddCell(
                Arg.Is<IRichTextString>(x => x.String == ViewResources.WinLoss),
                Arg.Any<ICellStyle>());
        }

        [Test]
        public void BuildTicket_WhenCall_CallReleventMethods()
        {
            // Arrange
            const int Index = 9000;
            _ticketHelper.Index.Returns(Index);
            var betListExcel = CreateBetListExcel();
            var ticketResult = new TicketResult();
            var ticket = new LrfTicket
            {
                ExternalId = "ExternalId",
                UserName = "UserName",
                Winlost = 1,
                BetEffective = 2,
                LicenseeWL = 3,
                Comm = 4,
                Status = BetStatus.Won,
                BetTypeId = (int)BetType.Handicap
            };
            ticketResult.MainTicketList.Add(ticket);

            // Action
            betListExcel.ProtectedBuildTicket(_builder, ticketResult, ticket);

            // Assert
            Assert.Multiple(() =>
            {
                // AddIndex
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == Index.ToString()), Arg.Any<ICellStyle>());

                // AddExternalId
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == ticket.ExternalId), Arg.Any<ICellStyle>());

                // AddSystemId
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == ticket.UserName), Arg.Any<ICellStyle>());

                // AddCustomerWL
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == string.Format("{0:#,##0.00}", ticket.Winlost)));

                // AddBetEffective
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == string.Format("{0:#,##0.00}\n", ticket.BetEffective)));

                // AddLicenseeWL
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == string.Format("{0:#,##0.00}", ticket.LicenseeWL + ticket.Comm)));

                _builder.Received(1).AddTrans(ticket, _ticketHelper);
                _builder.Received(1).AddChoice(ticket, _ticketHelper, ticketResult.TicketDataList);
                _builder.Received(1).AddOdds(ticket, ticketResult.TicketDataList, _ticketHelper.GetValidOddsTypeNameById);
                _builder.Received(1).AddStake(ticket, _ticketHelper);
                _builder.Received(1).AddStatus(ticket, _ticketHelper, ticketResult.TicketDataList, false);
            });
        }

        [Test]
        public void FixBugIOSMobile_WhenCall_CallRelevanMethods()
        {
            // Arrange
            var betListExcel = CreateBetListExcel();
            var workbook = Substitute.For<IWorkbook>();
            var sheet = Substitute.For<ISheet>();
            workbook.NumberOfSheets.Returns(1);
            workbook.GetSheetAt(0).Returns(sheet);
            sheet.LastRowNum.Returns(0);

            // Action
            betListExcel.ProtectedFixBugIOSMobile(workbook);

            // Assert
            sheet.Received(1000).CreateRow(Arg.Any<int>());
        }

        private static TicketResult CreateTicketResult()
        {
            var ticketResult = new TicketResult
            {
                SumWinLoss = 100,
                SumPlayerComm = 0.88m,
                SumLicenseeWL = 200,
                SumComm = 0.99m,
                SumBetEffective = 999
            };
            ticketResult.MainTicketList.Add(new LrfTicket { Stake = 0 });
            return ticketResult;
        }

        private BetListExcelTestClass CreateBetListExcel(BetListShowingOption showingOption = null, int[] hiddenColumns = null)
        {
            var betListExcel = Substitute.ForPartsOf<BetListExcelTestClass>(
                showingOption ?? new BetListShowingOption(),
                LicenseeId,
                hiddenColumns ?? [],
                _appSettings,
                _ipService,
                _ticketHelper);
            betListExcel.InvokeProtectedMethod("CreateExcelBuilder").Returns(_builder);
            betListExcel.ProtectedInitializeFonts(_builder);
            return betListExcel;
        }
    }
}
