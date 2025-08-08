namespace Fanex.Athena.BetList.Excel.Tests
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using Excel.Builders;
    using NPOI.SS.UserModel;
    using NSubstitute;
    using Resources;
    using TestClasses;
    using TestHelper;
    using TestHelper.Extensions;

    public class ExchangeRunningExcelTests
    {
        private const int NumberOfColumn = 11;

        private IExcelBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = Substitute.For<IExcelBuilder>();
        }

        [TearDown]
        public void TearDown()
        {
            _builder = null;
        }

        [Test]
        public void ExportRunningExcel_HasData_CallRelevantMethods()
        {
            // Arrange
            var data = FakeHelper.Create<ExchangeResult<ExchangeRunningTicket>>();
            var metaModel = new ExchangeRunningTicketMetaModel();
            var excel = CreateExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.LoadTemplate(Arg.Any<string>()).Returns(workbook);
            _builder.CloneSheet(Arg.Any<IWorkbook>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Substitute.For<ISheet>());
            workbook.GetSheet(default).ReturnsForAnyArgs(Substitute.For<ISheet>());

            excel.When(x => x.InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeRunningTicketMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeResult<ExchangeRunningTicket>>(), Arg.Any<ExchangeRunningTicketMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();

            // Action
            excel.ExportRunningExcel("template", data, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(1).LoadTemplate("template");
                _builder.Received(1).CloneSheet(workbook, "Template", "Data");
                _builder.Received(1).RemoveSheet(workbook, "Template");
                excel.Received(1).InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>());
                excel.Received(1).InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeRunningTicketMetaModel>());
            });
        }

        [Test]
        public void GetWorkbook_WhenCallWithNoData_CallAssignNoInfoRow()
        {
            // Arrange
            var data = FakeHelper.Create<ExchangeResult<ExchangeRunningTicket>>();
            var metaModel = new ExchangeRunningTicketMetaModel();
            data.DataList = new List<ExchangeRunningTicket>();
            var excel = CreateExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.LoadTemplate(Arg.Any<string>()).Returns(workbook);
            _builder.CloneSheet(Arg.Any<IWorkbook>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Substitute.For<ISheet>());
            workbook.GetSheet(default).ReturnsForAnyArgs(Substitute.For<ISheet>());

            excel.When(x => x.InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeRunningTicketMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeResult<ExchangeRunningTicket>>(), Arg.Any<ExchangeRunningTicketMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();

            // Action
            excel.GetWorkbook("template", data, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(1).LoadTemplate("template");
                _builder.Received(1).CloneSheet(workbook, "Template", "Data");
                excel.Received(1).InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>());
                excel.Received(1).InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeRunningTicketMetaModel>());
                excel.Received(1).InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>());
            });
        }

        [Test]
        public void GetWorkbook_WhenCallWithMainData_CallAssignSheetData()
        {
            // Arrange
            var data = FakeHelper.Create<ExchangeResult<ExchangeRunningTicket>>();
            var metaModel = new ExchangeRunningTicketMetaModel();
            data.DataList = new List<ExchangeRunningTicket>{ FakeHelper.Create<ExchangeRunningTicket>() };
            var excel = CreateExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.LoadTemplate(Arg.Any<string>()).Returns(workbook);
            _builder.CloneSheet(Arg.Any<IWorkbook>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Substitute.For<ISheet>());
            workbook.GetSheet(default).ReturnsForAnyArgs(Substitute.For<ISheet>());

            excel.When(x => x.InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeRunningTicketMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeResult<ExchangeRunningTicket>>(), Arg.Any<ExchangeRunningTicketMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();

            // Action
            excel.GetWorkbook("template", data, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(1).LoadTemplate("template");
                _builder.Received(1).CloneSheet(workbook, "Template", "Data");
                excel.Received(1).InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>());
                excel.Received(1).InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeRunningTicketMetaModel>());
                excel.Received(1).InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeResult<ExchangeRunningTicket>>(), Arg.Any<ExchangeRunningTicketMetaModel>());
            });
        }

        [Test]
        public void AssignHeader_WhenCall_AssignHeader()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var excel = CreateExcel();
            var row = Substitute.For<IRow>();
            var cell = Substitute.For<ICell>();
            _builder.CopyRow(sheet, templateSheet, 0, 0).ReturnsForAnyArgs(row);
            row.Cells.Returns(new List<ICell>());
            for (var i = 0; i < NumberOfColumn; i++)
            {
                row.Cells.Add(cell);
            }

            // Action
            excel.ProtectedAssignHeader(sheet, templateSheet);

            // Assert
            Assert.Multiple(() =>
            {
                cell.Received().SetCellValue("#");
                cell.Received().SetCellValue(ViewResources.ExternalId);
                cell.Received().SetCellValue(ViewResources.SystemId);
                cell.Received().SetCellValue(ViewResources.BetID);
                cell.Received().SetCellValue(ViewResources.BetPlaced);
                cell.Received().SetCellValue(ViewResources.BetPlaced);
                cell.Received().SetCellValue(ViewResources.Market);
                cell.Received().SetCellValue(ViewResources.Selection);
                cell.Received().SetCellValue(ViewResources.Type);
                cell.Received().SetCellValue(ViewResources.OddsAvg);
                cell.Received().SetCellValue(ViewResources.Stake);
            });
        }

        [Test]
        public void AssignNoInfoRow_WhenCall_CallRelevantMethod()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var excel = CreateExcel();

            // Action
            excel.ProtectedAssignNoInfoRow(sheet, templateSheet);

            // Assert
            _builder.Received(1).SetRowValue(sheet, templateSheet, 4, 2, 10, ViewResources.NoInfo);
        }

        [Test]
        public void AssignTitle_WhenCall_CallRelevantMethod()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var metaModel = FakeHelper.Create<ExchangeRunningTicketMetaModel>();
            var excel = CreateExcel();

            // Action
            excel.ProtectedAssignTitle(sheet, templateSheet, metaModel);

            // Assert
            _builder.Received(1).SetRowValue(sheet, templateSheet, 0, 0, 10, Arg.Is<string>(x => x.Contains(ViewResources.BettingTicketCenter)));

        }

        [Test]
        public void AssignTotal_WhenCall_CallRelevantMethods()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var excel = CreateExcel();
            var data = FakeHelper.Create<ExchangeResult<ExchangeRunningTicket>>();
            var metaModel = FakeHelper.Create<ExchangeRunningTicketMetaModel>();
            var row = Substitute.For<IRow>();
            var cell = Substitute.For<ICell>();
            row.Cells.Returns(new List<ICell>());
            for (var i = 0; i < 10; i++)
            {
                row.Cells.Add(cell);
            }

            _builder.CopyRow(sheet, templateSheet, 0, 0).ReturnsForAnyArgs(row);

            // Action
            excel.ProctectedAssignTotal(sheet, templateSheet, data, 0, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                cell.Received().SetCellValue(Arg.Is<string>(_ => _.Contains(metaModel.BaseCurrency)));
                cell.Received().SetCellValue((double)data.SumStake);
                _builder.Received().MergeCols(sheet, 0, 8, 0);
            });
        }


        [Test]
        public void AssignSheetData_WhenCallWithOnlyOneDetails_CallRelevantMethod()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var data = FakeHelper.Create<ExchangeResult<ExchangeRunningTicket>>();
            var metaModel = FakeHelper.Create<ExchangeRunningTicketMetaModel>();
            var ticket = FakeHelper.Create<ExchangeRunningTicket>();
            data.DataList = new List<ExchangeRunningTicket> { ticket };
            var excel = CreateExcel();
            var row = Substitute.For<IRow>();
            var cell = Substitute.For<ICell>();
            row.Cells.Returns(new List<ICell>());
            for (var i = 0; i < NumberOfColumn; i++)
            {
                row.Cells.Add(cell);
            }

            _builder.CopyRow(sheet, templateSheet, 0, 0).ReturnsForAnyArgs(row);

            // Action
            excel.ProtectedAssignSheetData(sheet, templateSheet, data, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                cell.Received().SetCellValue(ticket.NickName);
                cell.Received().SetCellValue(ticket.Username);
                cell.Received().SetCellValue(ticket.BetId);
                cell.Received().SetCellValue(ticket.BetPlacedString);
                cell.Received().SetCellValue($"{ticket.EventTypeName} > {ticket.EventName} > {ticket.MarketName}");
                cell.Received().SetCellValue(ticket.SelectionName);
                cell.Received().SetCellValue(ticket.Type);
                cell.Received().SetCellValue((double)ticket.Stake);
            });
        }

        private ExchangeRunningExcelTestClass CreateExcel()
            => Substitute.ForPartsOf<ExchangeRunningExcelTestClass>(_builder);
    }
}
