namespace Fanex.Athena.BetList.Excel.Tests
{
    using BetList.Model.DTO;
    using BetListNPOI.SS.UserModel;
    using BetListNPOI.SS.Util;
    using Common.Constant;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using NSubstitute;
    using Resources;
    using CellAddress = Fanex.BetList.Core.Entities.CellAddress;

    public class BetListExcelBuilderExtensionTests
    {
        private IBetListExcelBuilder _builder;
        private IFont _font;

        [SetUp]
        public void Setup()
        {
            _builder = Substitute.For<IBetListExcelBuilder>();
            _font = Substitute.For<IFont>();
        }

        [TestCase(false, "1,000.00")]
        [TestCase(true, "1,000.00\n2.99")]
        public void AddCustomerWL_ShowPlayerComm_CallAddCell(bool isShowPlayerComm, string expectedValue)
        {
            // Arrange
            var ticket = new Ticket
            {
                Winlost = 1000,
                PlayerComm = 2.99m
            };

            // Action
            _builder.AddCustomerWL(ticket, isShowPlayerComm, _font);

            // Assert
            _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == expectedValue));
        }

        [TestCase("running", "-")]
        [TestCase("abc", "1,000.00\n2.99")]
        public void AddCustomerWL_Status_CallAddCell(string status, string expectedValue)
        {
            // Arrange
            var ticket = new Ticket
            {
                Status = status,
                Winlost = 1000,
                PlayerComm = 2.99m
            };

            // Action
            _builder.AddCustomerWL(ticket, true, _font);

            // Assert
            _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == expectedValue));
        }

        [Test]
        public void AddBetEffective_WhenCall_AddCell()
        {
            // Arrange
            var ticket = new LrfTicket
            {
                BetEffective = 12345.67m
            };

            // Action
            _builder.AddBetEffective(ticket, _font);

            // Assert
            _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == "12,345.67\n"));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void AddExternalId_WhenCall_CallAddCell(int index)
        {
            // Arrange
            const string ExternalId = "aaaaaaaaaaaaaaaaaa";
            var ticketHelper = Substitute.For<ITicketHelper>();
            ticketHelper.Index = index;
            var oddStyle = Substitute.For<ICellStyle>();
            var evenStyle = Substitute.For<ICellStyle>();

            // Action
            _builder.AddExternalId(ticketHelper, ExternalId, oddStyle, evenStyle);

            // Assert
            _builder.Received(1).AddCell(
                Arg.Is<IRichTextString>(x => x.String == ExternalId),
                index % 2 == 0 ? oddStyle : evenStyle);
        }

        [Test]
        public void AddFooterForSumStake_WhenCall_CallAddCell()
        {
            // Arrange
            const decimal SumStake = 12345.67m;
            const int LastRowIndex = 999;
            _builder.LastContentRow.Returns(LastRowIndex);

            // Action
            _builder.AddFooterForSumStake(SumStake);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(6).AddCell(
                    Arg.Is<IRichTextString>(x => x.String == ViewResources.GrandTotal),
                    Arg.Is<ICellStyle>(x =>
                        x.Alignment == HorizontalAlignment.Right &&
                        x.BorderTop == BorderStyle.None &&
                        x.BorderLeft == BorderStyle.None &&
                        x.BorderBottom == BorderStyle.None));

                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == "12,345.67"), Arg.Any<ICellStyle>());

                _builder.Received(4).AddCell(Arg.Is<IRichTextString>(x => x.String == string.Empty), Arg.Any<ICellStyle>());

                _builder.Received(1).MergeCell(
                    Arg.Is<CellRangeAddress>(x =>
                        x.FirstRow == LastRowIndex && x.LastRow == LastRowIndex &&
                        x.FirstColumn == 0 && x.LastColumn == 5),
                    Arg.Any<ICellStyle>());
            });
        }

        [Test]
        public void AddFooter_WhenCall_CallAddCell()
        {
            // Arrange
            const decimal SumCustomerWL = 12345.67m;
            const decimal SumBetEffective = 22345.67m;
            const decimal SumLicenseeWLComm = 32345.67m;
            const int LastRowIndex = 999;
            _builder.LastContentRow.Returns(LastRowIndex);
            var redFont = Substitute.For<IFont>();
            var blackFont = Substitute.For<IFont>();

            // Action
            _builder.AddFooter(SumCustomerWL, SumBetEffective, SumLicenseeWLComm, redFont, blackFont);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(7).AddCell(
                    Arg.Is<IRichTextString>(x => x.String == ViewResources.GrandTotal),
                    Arg.Any<ICellStyle>());

                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == "12,345.67"), Arg.Any<ICellStyle>());
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == "22,345.67"), Arg.Any<ICellStyle>());
                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == "32,345.67"), Arg.Any<ICellStyle>());

                _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == string.Empty), Arg.Any<ICellStyle>());

                _builder.Received(1).MergeCell(
                    Arg.Is<CellRangeAddress>(x =>
                        x.FirstRow == LastRowIndex && x.LastRow == LastRowIndex &&
                        x.FirstColumn == 0 && x.LastColumn == 6),
                    Arg.Any<ICellStyle>());
            });
        }

        [Test]
        public void AddHeader_WhenCall_CallAddCell()
        {
            // Arrange
            const string CustomerWLHeader = "aaaaaaaaaaaaaaaaaa";
            var captions = new[]
            {
                "#", ViewResources.ExternalId, ViewResources.SystemId, ViewResources.TransTime, ViewResources.Choice, ViewResources.Odds, ViewResources.Stake,
                CustomerWLHeader, ViewResources.EffectiveBettingValue, ViewResources.LicenseeWL + "/" + ViewResources.Comm, ViewResources.Status
            };
            var columWidths = new short[] { 1500, 4800, 5800, 5400, 15500, 3400, 7800, 7800, 5400, 9000, 4000 };
            var cellList = Enumerable.Range(0, captions.Length).Select(x => new CellAddress(0, x)).ToArray();
            _builder.AddCell(null, null).ReturnsForAnyArgs(cellList[0], cellList.Skip(1).ToArray());

            // Action
            _builder.AddHeader(CustomerWLHeader);

            // Assert
            Assert.Multiple(() =>
            {
                for (int i = 0; i < captions.Length; i++)
                {
                    _builder.Received(1).AddCell(
                        Arg.Is<IRichTextString>(x => x.String == captions[i]),
                        Arg.Is<ICellStyle>(x => x.Alignment == HorizontalAlignment.Center));
                    _builder.Received(1).SetColumnWidth(i, columWidths[i]);
                }
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        public void AddIndex_WhenCall_CallAddCell(int index)
        {
            // Arrange
            const int Index = 999;
            var ticketHelper = Substitute.For<ITicketHelper>();
            ticketHelper.Index = index;
            _builder.OddCellStyleCenterAligned = Substitute.For<ICellStyle>();
            _builder.EvenCellStyleCenterAligned = Substitute.For<ICellStyle>();

            // Action
            _builder.AddIndex(ticketHelper, Index);

            // Assert
            _builder.Received(1).AddCell(
                Arg.Is<IRichTextString>(x => x.String == "999"),
                index % 2 == 0 ? _builder.OddCellStyleCenterAligned : _builder.EvenCellStyleCenterAligned);
        }

        [TestCase(BetType.CashOutBought, false, "10,000.55\n1.44")]
        [TestCase(BetType.BetTradeBought, true, "-\n-")]
        [TestCase(BetType.CorrectScore, true, "-")]
        [TestCase(BetType.CorrectScore, false, "10,001.99")]
        public void AddLicenseeWL_WhenCall_AddCellWithCorrectValue(int bettype, bool isRunning, string expectedValue)
        {
            // Arrange
            var lrfTicket = new LrfTicket
            {
                Status = isRunning ? "running" : "abc",
                BetTypeId = bettype,
                LicenseeWL = 10000.55m,
                Comm = 1.44m
            };

            var redFont = Substitute.For<IFont>();
            var blackFont = Substitute.For<IFont>();

            // Action
            _builder.AddLicenseeWL(lrfTicket, true, redFont, blackFont);

            // Assert
            _builder.Received(1).AddCell(Arg.Is<IRichTextString>(x => x.String == expectedValue));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void AddSystemId_WhenCall_CallAddCell(int index)
        {
            // Arrange
            const string SystemId = "bbbbbbbbbbbbb";
            var ticketHelper = Substitute.For<ITicketHelper>();
            ticketHelper.Index = index;
            var oddStyle = Substitute.For<ICellStyle>();
            var evenStyle = Substitute.For<ICellStyle>();

            // Action
            _builder.AddSystemId(ticketHelper, SystemId, oddStyle, evenStyle);

            // Assert
            _builder.Received(1).AddCell(
                Arg.Is<IRichTextString>(x => x.String == SystemId),
                index % 2 == 0 ? oddStyle : evenStyle);
        }
    }
}
