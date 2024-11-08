namespace Fanex.Athena.BetList.Excel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fanex.Athena.BetList.Excel.Builders;
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.BetList.Model.ViewModel;
    using Fanex.Athena.Common.Constant;
    using Fanex.Athena.Resources;
    using NPOI.SS.UserModel;

    public class ExchangeWinLossExcel : IExchangeWinLossExcel
    {
        private const string TemplateSheetName = "Template";
        private const string SheetName = "Data";

        private const int ColumnIndex = 0;
        private const int ColumnMarket = 1;
        private const int ColumnBetId = 2;
        private const int ColumnSelection = 3;
        private const int ColumnType = 4;
        private const int ColumnBetPlaced = 5;
        private const int ColumnOddsAvg = 6;
        private const int ColumnStake = 7;
        private const int ColumnWL = 8;
        private const int ColumnWL_Comm = 9;

        private const int RowNoInfoTemplate = 4;
        private const int ColumnTemplateCount = 10;

        private const int RowTitleTemplate = 0;
        private const int RowHeaderTemplate = 1;
        private const int RowContentTemplate = 2;
        private const int RowTotalTemplate = 3;

        private readonly IExcelBuilder _builder;

        public ExchangeWinLossExcel(IExcelBuilder builder)
        {
            _builder = builder;
        }

        public byte[] ExportExcel(string template, ExchangeBetListViewModel data, ExchangeBetListMetaModel metaModel)
        {
            var workBook = GetWorkbook(template, data, metaModel);

            using (var stream = new MemoryStream())
            {
                workBook.Write(stream);
                workBook.Close();

                return stream.ToArray();
            }
        }

        public IWorkbook GetWorkbook(string template, ExchangeBetListViewModel data, ExchangeBetListMetaModel metaModel)
        {
            var workBook = _builder.LoadTemplate(template);
            var sheet = _builder.CloneSheet(workBook, TemplateSheetName, SheetName);
            var templateSheet = workBook.GetSheet(TemplateSheetName);

            AssignHeader(sheet, templateSheet);
            AssignTitle(sheet, templateSheet, metaModel);

            if (data.MainData.Any())
            {
                AssignSheetData(sheet, templateSheet, data);
            }
            else
            {
                AssignNoInfoRow(sheet, templateSheet);
            }

            _builder.RemoveSheet(workBook, TemplateSheetName);

            return workBook;
        }

        protected void AssignHeader(ISheet sheet, ISheet templateSheet)
        {
            var row = _builder.CopyRow(sheet, templateSheet, sourceRowIndex: RowHeaderTemplate, destRowIndex: RowHeaderTemplate);

            var headerTextList = new[]
            {
                "#",
                ViewResources.Market,
                ViewResources.BetID,
                ViewResources.Selection,
                ViewResources.Type,
                ViewResources.BetPlaced,
                ViewResources.OddsAvg,
                ViewResources.Stake,
                ViewResources.WinLoss,
                $"{ViewResources.WL}/{ViewResources.Comm}"
            };

            SetTextRowHeader(row, headerTextList);
        }

        private static void SetTextRowHeader(IRow row, IEnumerable<string> headerTextList)
        {
            for (int i = 0; i < headerTextList.Count(); i++)
            {
                row.Cells[i].SetCellValue(headerTextList.ElementAt(i));
            }
        }

        private void AssignNoInfoRow(ISheet sheet, ISheet templateSheet)
        {
            _builder.SetRowValue(sheet, templateSheet, RowNoInfoTemplate, RowContentTemplate, ColumnTemplateCount, ViewResources.NoInfo);
        }

        private void AssignSheetData(ISheet sheet, ISheet templateSheet, ExchangeBetListViewModel data)
        {
            const float rowHeight = 25.5F;
            var index = 0;
            var rowIndex = 0;

            foreach (var item in data.MainData)
            {
                var isFirstRow = true;

                foreach (var detail in item.Details)
                {
                    var row = _builder.CopyRow(sheet, templateSheet, RowContentTemplate, RowContentTemplate + index);
                    row.HeightInPoints = rowHeight;

                    if (isFirstRow)
                    {
                        row.Cells[ColumnIndex].SetCellValue(++rowIndex);
                        row.Cells[ColumnMarket].SetCellValue($"{item.Ticket.EventTypeName} > {item.Ticket.EventName} > {item.Ticket.MarketName}");
                        row.Cells[ColumnWL_Comm].SetCellValue($"{item.Ticket.WinLost.ToString("#,0.00")}\n{item.Ticket.Commission.ToString("#,0.00")}");
                        isFirstRow = false;
                    }

                    AssignCellData(row, detail);
                    index++;
                }

                if (item.Details.Count() > 1)
                {
                    MergeCellData(sheet, item.Details.Count(), index);
                }
            }
            AssignTotal(sheet, templateSheet, data, RowContentTemplate + index);
        }

        private void MergeCellData(ISheet sheet, int rowCount, int index)
        {
            var firstRow = RowContentTemplate + index - rowCount;
            var lastRow = RowContentTemplate + index - 1;

            _builder.MergeCells(sheet, firstRow, lastRow, ColumnIndex, ColumnIndex);
            _builder.MergeCells(sheet, firstRow, lastRow, ColumnMarket, ColumnMarket);
            _builder.MergeCells(sheet, firstRow, lastRow, ColumnWL_Comm, ColumnWL_Comm);
        }

        private static void AssignCellData(IRow row, ExchangeTicket item)
        {
            var isFancyBet = item.CategoryType == 2;
            var oddsAvgStrings = isFancyBet ? $"{item.FancyBetRuns.ToString("0")}/{item.OddsAvg.ToString("0")}" : item.OddsAvg.ToString("0.00");
            row.Cells[ColumnBetId].SetCellValue(item.BetId);
            row.Cells[ColumnSelection].SetCellValue(item.SelectionName);
            row.Cells[ColumnType].SetCellValue(item.Type);
            row.Cells[ColumnBetPlaced].SetCellValue(item.BetPlacedDateString);
            row.Cells[ColumnOddsAvg].SetCellValue(oddsAvgStrings);
            row.Cells[ColumnStake].SetCellValue((double)item.Stake);
            row.Cells[ColumnWL].SetCellValue((double)item.WinLost);
        }

        private void AssignTitle(ISheet sheet, ISheet templateSheet, ExchangeBetListMetaModel metaModel)
        {
            var baseCurrency = metaModel.BaseCurrency.ToUpperInvariant();
            var displayedCurrency = !string.IsNullOrEmpty(baseCurrency) ? baseCurrency : metaModel.CurrencyName;
            var baseCurrencyText = string.Format(ViewResources.Viewin, displayedCurrency);
            var dateText = $"{metaModel.FromDate.ToString(FormatKey.DateFormat)} -> {metaModel.ToDate.ToString(FormatKey.DateFormat)}";
            var reportTitle = $"{ViewResources.PlayerBetList} {dateText} - (GMT+8) - {metaModel.ExternalId}({metaModel.SystemId}) - {baseCurrencyText}";

            _builder.SetRowValue(
                sheet,
                templateSheet,
                sourceRowIndex: RowTitleTemplate,
                destRowIndex: RowTitleTemplate,
                columnCount: ColumnTemplateCount,
                value: reportTitle);

            sheet.CreateFreezePane(0, RowContentTemplate);

            _builder.ClearTemplate(sheet, RowContentTemplate);
        }

        private void AssignTotal(ISheet sheet, ISheet templateSheet, ExchangeBetListViewModel data, int index)
        {
            var row = _builder.CopyRow(sheet, templateSheet, RowTotalTemplate, index);
            row.Cells[0].SetCellValue(ViewResources.GrandTotal);
            row.Cells[ColumnWL_Comm].SetCellValue((double)data.GrandTotal);

            _builder.MergeCols(sheet, 0, ColumnWL, index);
        }
    }
}