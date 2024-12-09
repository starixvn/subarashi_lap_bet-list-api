using Fanex.Athena.BetList.Excel.Builders;
using Fanex.Athena.BetList.Excel.Tests.TestClasses;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.Resources;
using Fanex.Athena.TestHelper;
using Fanex.Athena.TestHelper.Extensions;
using NPOI.SS.UserModel;
using NSubstitute;

namespace Fanex.Athena.BetList.Excel.Tests
{
    public class ExchangeWinLossExcelTests
    {
        private const int NumberOfColumn = 10;

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
        public void ExportExcel_HasData_CallRelevantMethods()
        {
            // Arrange
            var data = new ExchangeBetListViewModel();
            data.MainData = new List<ExchangeBetList>();
            var metaModel = new ExchangeBetListMetaModel();
            var excel = CreateExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.LoadTemplate(Arg.Any<string>()).Returns(workbook);
            _builder.CloneSheet(Arg.Any<IWorkbook>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Substitute.For<ISheet>());
            workbook.GetSheet(default).ReturnsForAnyArgs(Substitute.For<ISheet>());

            excel.When(x => x.InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListViewModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();

            // Action
            excel.ExportExcel("template", data, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(1).LoadTemplate("template");
                _builder.Received(1).CloneSheet(workbook, "Template", "Data");
                excel.Received(1).InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>());
                excel.Received(1).InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListMetaModel>());
            });
        }

        [Test]
        public void GetWorkbook_WhenCallWithNoData_CallAssignNoInfoRow()
        {
            // Arrange
            var data = new ExchangeBetListViewModel();
            data.MainData = new List<ExchangeBetList>();
            var metaModel = new ExchangeBetListMetaModel();
            var excel = CreateExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.LoadTemplate(Arg.Any<string>()).Returns(workbook);
            _builder.CloneSheet(Arg.Any<IWorkbook>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Substitute.For<ISheet>());
            workbook.GetSheet(default).ReturnsForAnyArgs(Substitute.For<ISheet>());

            excel.When(x => x.InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListViewModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();

            // Action
            excel.GetWorkbook("template", data, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(1).LoadTemplate("template");
                _builder.Received(1).CloneSheet(workbook, "Template", "Data");
                excel.Received(1).InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>());
                excel.Received(1).InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListMetaModel>());
                excel.Received(1).InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>());
            });
        }

        [Test]
        public void GetWorkbook_WhenCallWithMainData_CallAssignSheetData()
        {
            // Arrange
            var data = new ExchangeBetListViewModel();
            data.MainData = new List<ExchangeBetList>{FakeHelper.Create<ExchangeBetList>()};
            var metaModel = new ExchangeBetListMetaModel();
            var excel = CreateExcel();
            var workbook = Substitute.For<IWorkbook>();
            _builder.LoadTemplate(Arg.Any<string>()).Returns(workbook);
            _builder.CloneSheet(Arg.Any<IWorkbook>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Substitute.For<ISheet>());
            workbook.GetSheet(default).ReturnsForAnyArgs(Substitute.For<ISheet>());

            excel.When(x => x.InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListMetaModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListViewModel>())).DoNotCallBase();
            excel.When(x => x.InvokeProtectedMethod("AssignNoInfoRow", Arg.Any<ISheet>(), Arg.Any<ISheet>())).DoNotCallBase();

            // Action
            excel.GetWorkbook("template", data, metaModel);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.Received(1).LoadTemplate("template");
                _builder.Received(1).CloneSheet(workbook, "Template", "Data");
                excel.Received(1).InvokeProtectedMethod("AssignHeader", Arg.Any<ISheet>(), Arg.Any<ISheet>());
                excel.Received(1).InvokeProtectedMethod("AssignTitle", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListMetaModel>());
                excel.Received(1).InvokeProtectedMethod("AssignSheetData", Arg.Any<ISheet>(), Arg.Any<ISheet>(), Arg.Any<ExchangeBetListViewModel>());
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
            excel.ProtectedAssignHeader( sheet, templateSheet);

            // Assert
            Assert.Multiple(() =>
            {
                cell.Received().SetCellValue("#");
                cell.Received().SetCellValue(ViewResources.Market);
                cell.Received().SetCellValue(ViewResources.BetID);
                cell.Received().SetCellValue(ViewResources.Selection);
                cell.Received().SetCellValue(ViewResources.Type);
                cell.Received().SetCellValue(ViewResources.BetPlaced);
                cell.Received().SetCellValue(ViewResources.OddsAvg);
                cell.Received().SetCellValue(ViewResources.Stake);
                cell.Received().SetCellValue(ViewResources.WinLoss);
                cell.Received().SetCellValue($"{ViewResources.WL}/{ViewResources.Comm}");
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
            var metaModel = FakeHelper.Create<ExchangeBetListMetaModel>();
            var excel = CreateExcel();

            // Action
            excel.ProtectedAssignTitle(sheet, templateSheet, metaModel);

            // Assert
            _builder.Received(1).SetRowValue(sheet, templateSheet, 0, 0, 10, Arg.Is<string>(x => x.Contains(metaModel.SystemId)));

        }

        [Test]
        public void AssignTotal_WhenCall_CallRelevantMethods()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var excel = CreateExcel();
            var data = new ExchangeBetListViewModel();
            var row = Substitute.For<IRow>();
            var cell = Substitute.For<ICell>();
            row.Cells.Returns(new List<ICell>());
            for (var i = 0; i < 10; i++)
            {
                row.Cells.Add(cell);
            }

            _builder.CopyRow(sheet, templateSheet, 0, 0).ReturnsForAnyArgs(row);

            // Action
            excel.ProctectedAssignTotal(sheet, templateSheet, data, 0);

            // Assert
            Assert.Multiple(() =>
            {
                cell.Received().SetCellValue(ViewResources.GrandTotal);
            });
        }

        [Test]
        public void AssignSheetData_WhenCallWithNoDetails_DoesNotCallRelevantMethod()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var data = new ExchangeBetListViewModel();
            data.MainData = new List<ExchangeBetList>{ new ExchangeBetList{Details = new List<ExchangeTicket>()}};
            var excel = CreateExcel();

            excel.When(x => x.InvokeProtectedMethod("AssignTotal", Arg.Any<ISheet>(), Arg.Any<ISheet>(), data, Arg.Any<int>())).DoNotCallBase();

            // Action
            excel.ProtectedAssignSheetData(sheet, templateSheet, data);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.DidNotReceiveWithAnyArgs().CopyRow(sheet, templateSheet, 0, 0);
                _builder.DidNotReceiveWithAnyArgs().MergeCells(sheet, default, default, default, default);
            });
        }

        [Test]
        public void AssignSheetData_WhenCallWithOnlyOneDetails_CallRelevantMethod()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var data = new ExchangeBetListViewModel();
            var detailTicket = FakeHelper.Create<ExchangeTicket>();
            var ticket = FakeHelper.Create<ExchangeBetList>(x => x.Details, new List<ExchangeTicket>{detailTicket});
            data.MainData = new List<ExchangeBetList> { ticket };
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
            excel.ProtectedAssignSheetData(sheet, templateSheet, data);

            // Assert
            Assert.Multiple(() =>
            {
                cell.Received().SetCellValue(1);
                cell.Received().SetCellValue($"{ticket.Ticket.EventTypeName} > {ticket.Ticket.EventName} > {ticket.Ticket.MarketName}");
                cell.Received().SetCellValue($"{ticket.Ticket.WinLost.ToString("#,0.00")}\n{ticket.Ticket.Commission.ToString("#,0.00")}");
                _builder.DidNotReceiveWithAnyArgs().MergeCells(default, default, default, default, default);
            });
        }

        [Test]
        public void AssignSheetData_WhenCallWithTwoDetails_CallMergeMethod()
        {
            // Arrange
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var data = new ExchangeBetListViewModel();
            var detailTicket = FakeHelper.Create<ExchangeTicket>();
            var ticket = FakeHelper.Create<ExchangeBetList>(x => x.Details, new List<ExchangeTicket> { detailTicket, detailTicket });
            data.MainData = new List<ExchangeBetList> { ticket };
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
            excel.ProtectedAssignSheetData(sheet, templateSheet, data);

            // Assert
            Assert.Multiple(() =>
            {
                _builder.ReceivedWithAnyArgs().MergeCells(default, default, default, default, default);
            });
        }

        private ExchangeWinlossExcelTestClass CreateExcel()
            => Substitute.ForPartsOf<ExchangeWinlossExcelTestClass>(_builder);
    }
}
