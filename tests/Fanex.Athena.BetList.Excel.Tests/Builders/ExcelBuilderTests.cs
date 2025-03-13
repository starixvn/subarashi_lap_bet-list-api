using Fanex.Athena.BetList.Excel.Builders;
using NPOI.SS.UserModel;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Fanex.Athena.BetList.Excel.Tests.Builders
{
    public class ExcelBuilderTests
    {
        private IWorkbook _workbook;

        [SetUp]
        public void SetUp()
        {
            _workbook = Substitute.For<IWorkbook>();
        }

        [TearDown]
        public void TearDown()
        {
            _workbook.Dispose();
            _workbook = null;
        }

        [Test]
        public void CreateWorkbook_WhenCall_ReturnWorkbook()
        {
            // Arrange
            var builder = new ExcelBuilder();

            // Action
            using var result = builder.CreateWorkbook();

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CloneSheet_WhenCall_ReturnCorrectSheet()
        {
            // Arrange
            var builder = new ExcelBuilder();
            var cloneSheet = Substitute.For<ISheet>();
            _workbook.CloneSheet(default).ReturnsForAnyArgs(cloneSheet);

            // Action
            var result = builder.CloneSheet(_workbook, "sheet", "sheetClone");

            // Assert
            Assert.That(result, Is.EqualTo(cloneSheet));
            _workbook.Received().SetSheetName(Arg.Any<int>(), "sheetClone");
        }

        [Test]
        public void RemoveSheet_WhenSheetNotExist_NotCallAnyMethod()
        {
            //Arrange
            var builder = new ExcelBuilder();
            _workbook.GetSheet("sheet").ReturnsNull();

            //Action
            builder.RemoveSheet(_workbook, "sheet");

            //Assert
            _workbook.DidNotReceive().RemoveSheetAt(Arg.Any<int>());
        }

        [Test]
        public void RemoveSheet_WhenSheetExist_RemoveSheet()
        {
            //Arrange
            var builder = new ExcelBuilder();
            var sheet = Substitute.For<ISheet>();
            _workbook.GetSheet("sheet").Returns(sheet);

            //Action
            builder.RemoveSheet(_workbook, "sheet");

            //Assert
            _workbook.Received().RemoveSheetAt(Arg.Any<int>());
        }

        [TestCase(CellType.Unknown)]
        [TestCase(CellType.Numeric)]
        [TestCase(CellType.String)]
        [TestCase(CellType.Formula)]
        [TestCase(CellType.Blank)]
        [TestCase(CellType.Boolean)]
        [TestCase(CellType.Error)]
        public void SetRowValue_WhenCall_ReturnCorrectRow(CellType cellType)
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sourceSheet = Substitute.For<ISheet>();
            var destSheet = Substitute.For<ISheet>();
            var sourceRow = Substitute.For<IRow>();
            var destRow = Substitute.For<IRow>();
            var sourceCell = Substitute.For<ICell>();
            var destCell = Substitute.For<ICell>();
            sourceSheet.LastRowNum.Returns(0);

            destSheet.GetRow(0).Returns(destRow);
            sourceSheet.GetRow(0).Returns(sourceRow);

            sourceRow.LastCellNum.Returns((short)1);

            sourceRow.GetCell(0).Returns(sourceCell);
            destRow.CreateCell(0).Returns(destCell);
            sourceCell.CellType.Returns(cellType);
            sourceCell.Hyperlink.Returns((IHyperlink)null);
            sourceCell.CellComment.Returns((IComment)null);


            // Action
            var result = builder.CopyRow(destSheet, sourceSheet, 0, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(destRow));
                destCell.Received().SetCellType(cellType);
            });
        }

        [TestCase(CellType.Error)]
        public void SetRowValue_WhenCallWithSourceCellIsNull_ReturnCorrectRow(CellType cellType)
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sourceSheet = Substitute.For<ISheet>();
            var destSheet = Substitute.For<ISheet>();
            var sourceRow = Substitute.For<IRow>();
            var destRow = Substitute.For<IRow>();

            var destCell = Substitute.For<ICell>();
            sourceSheet.LastRowNum.Returns(0);

            destSheet.GetRow(0).Returns(destRow);
            sourceSheet.GetRow(0).Returns(sourceRow);

            sourceRow.LastCellNum.Returns((short)1);

            sourceRow.GetCell(0).Returns((ICell)null);
            destRow.CreateCell(0).Returns(destCell);


            // Action
            var result = builder.CopyRow(destSheet, sourceSheet, 0, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(destRow));
                destCell.DidNotReceiveWithAnyArgs().SetCellType(cellType);
            });
        }

        [TestCase(CellType.Error)]
        public void SetRowValue_WhenCallWithSourceCellHaveCellCommentAndHyperLink_SetDestCellCorrectValue(CellType cellType)
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sourceSheet = Substitute.For<ISheet>();
            var destSheet = Substitute.For<ISheet>();
            var sourceRow = Substitute.For<IRow>();
            var destRow = Substitute.For<IRow>();
            var sourceCell = Substitute.For<ICell>();
            var destCell = Substitute.For<ICell>();
            sourceSheet.LastRowNum.Returns(0);

            destSheet.GetRow(0).Returns(destRow);
            sourceSheet.GetRow(0).Returns(sourceRow);

            sourceRow.LastCellNum.Returns((short)1);

            sourceRow.GetCell(0).Returns(sourceCell);
            destRow.CreateCell(0).Returns(destCell);
            var comment = Substitute.For<IComment>();
            var hyperlink = Substitute.For<IHyperlink>();
            sourceCell.CellComment.Returns(comment);
            sourceCell.Hyperlink.Returns(hyperlink);

            // Action
            var result = builder.CopyRow(destSheet, sourceSheet, 0, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(destRow));
                Assert.That(destCell.CellComment, Is.EqualTo(comment));
                Assert.That(destCell.Hyperlink, Is.EqualTo(hyperlink));
            });
        }

        [Test]
        public void ClearTemplate_WhenCall_CallRelevantMethod()
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sheet = Substitute.For<ISheet>();
            sheet.LastRowNum.Returns(0);

            // Action
            builder.ClearTemplate(sheet, 0);

            // Assert
            sheet.Received(1).RemoveRow(Arg.Any<IRow>());
        }

        [Test]
        public void MergeCells_WhenCall_ReturnCorrectValue()
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sheet = Substitute.For<ISheet>();

            // Action
            var result = builder.MergeCells(sheet, 0, 1, 0, 1);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void MergeCCols_WhenCall_ReturnCorrectValue()
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sheet = Substitute.For<ISheet>();

            // Action
            var result = builder.MergeCols(sheet, 0, 1, 0);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void HideRow_WhenCall_SetCorrectValue()
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sheet = Substitute.For<ISheet>();
            var cell = Substitute.For<ICell>();
            var row = Substitute.For<IRow>();
            sheet.GetRow(0).ReturnsForAnyArgs(row);
            row.GetCell(0).ReturnsForAnyArgs(cell);
            row.ZeroHeight = false;

            // Action
            builder.HideRow(sheet, 0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(row.ZeroHeight, Is.True);
                cell.Received().SetCellValue(string.Empty);
            });
        }

        [Test]
        public void HideColumn_WhenCall_SetCorrectValue()
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sheet = Substitute.For<ISheet>();
            var cell = Substitute.For<ICell>();
            var row = Substitute.For<IRow>();
            sheet.GetRow(0).ReturnsForAnyArgs(row);
            row.GetCell(0).ReturnsForAnyArgs(cell);
            sheet.FirstRowNum.Returns(0);
            sheet.LastRowNum.Returns(0);

            // Action
            builder.HideColumn(sheet, 0);

            // Assert
            Assert.Multiple(() =>
                {
                    cell.Received().SetCellValue(string.Empty);
                    sheet.Received().SetColumnWidth(0, 0);
                    sheet.Received().SetColumnHidden(0, true);
                });
        }

        [Test]
        public void SetRowValue_WhenCall_CallRelevantMethods()
        {
            // Arrange
            var builder = new ExcelBuilder();
            var sheet = Substitute.For<ISheet>();
            var templateSheet = Substitute.For<ISheet>();
            var sourceRow = Substitute.For<IRow>();
            var destRow = Substitute.For<IRow>();
            var destCell = Substitute.For<ICell>();
            sourceRow.LastCellNum.Returns((short)0);
            sheet.GetRow(0).ReturnsForAnyArgs(destRow);
            destRow.Cells.Returns([destCell]);

            // Action
            builder.SetRowValue(sheet, templateSheet, 0, 1, 1, "value");

            // Assert
            Assert.Multiple(() =>
                {
                    destCell.Received().SetCellValue("value");
                });
        }
    }
}
