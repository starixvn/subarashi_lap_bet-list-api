namespace Fanex.Athena.BetList.Excel
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using Builders;
    using NPOI.SS.UserModel;
    using Resources;

    public class ExchangeRunningExcel : IExchangeRunningExcel
    {
        private const string TemplateSheetName = "Template";
        private const string SheetName = "Data";

        private const int ColumnIndex = 0;
        private const int ColumnExternalID = 1;
        private const int ColumnSystemID = 2;
        private const int ColumnBetId = 3;
        private const int ColumnBetPlaced = 4;
        private const int ColumnMarket = 5;
        private const int ColumnSelection = 6;
        private const int ColumnType = 7;
        private const int ColumnOddsAvg = 8;
        private const int ColumnStake = 9;


        private const int RowNoInfoTemplate = 4;
        private const int ColumnTemplateCount = 10;

        private const int RowTitleTemplate = 0;
        private const int RowHeaderTemplate = 1;
        private const int RowContentTemplate = 2;
        private const int RowTotalTemplate = 3;

        private readonly IExcelBuilder _builder;

        public ExchangeRunningExcel(IExcelBuilder builder)
        {
            _builder = builder;
        }

        public byte[] ExportRunningExcel(string template, ExchangeResult<ExchangeRunningTicket> data, ExchangeRunningTicketMetaModel metaModel)
        {
            var workBook = GetWorkbook(template, data, metaModel);

            using (var stream = new MemoryStream())
            {
                workBook.Write(stream);
                workBook.Close();

                return stream.ToArray();
            }
        }

        public IWorkbook GetWorkbook(string template, ExchangeResult<ExchangeRunningTicket> data, ExchangeRunningTicketMetaModel metaModel)
        {
            var workBook = _builder.LoadTemplate(template);
            var sheet = _builder.CloneSheet(workBook, TemplateSheetName, SheetName);
            var templateSheet = workBook.GetSheet(TemplateSheetName);

            AssignHeader(sheet, templateSheet);
            AssignTitle(sheet, templateSheet, metaModel);

            if (data.DataList.Any())
            {
                AssignSheetData(sheet, templateSheet, data, metaModel);
            }
            else
            {
                AssignNoInfoRow(sheet, templateSheet);
            }

            _builder.RemoveSheet(workBook, TemplateSheetName);

            return workBook;
        }

        protected virtual void AssignHeader(ISheet sheet, ISheet templateSheet)
        {
            var row = _builder.CopyRow(sheet, templateSheet, sourceRowIndex: RowHeaderTemplate, destRowIndex: RowHeaderTemplate);

            var headerTextList = new[]
            {
                "#",
                ViewResources.ExternalId,
                ViewResources.SystemId,
                ViewResources.BetID,
                ViewResources.BetPlaced,
                ViewResources.Market,
                ViewResources.Selection,
                ViewResources.Type,
                ViewResources.OddsAvg,
                ViewResources.Stake,
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

        protected virtual void AssignNoInfoRow(ISheet sheet, ISheet templateSheet)
        {
            _builder.SetRowValue(sheet, templateSheet, RowNoInfoTemplate, RowContentTemplate, ColumnTemplateCount, ViewResources.NoInfo);
        }

        protected virtual void AssignSheetData(ISheet sheet, ISheet templateSheet, ExchangeResult<ExchangeRunningTicket> data, ExchangeRunningTicketMetaModel metaModel)
        {
            int index = 0;

            foreach (var item in data.DataList)
            {
                var row = _builder.CopyRow(sheet, templateSheet, RowContentTemplate, RowContentTemplate + index);
                row.Cells[ColumnIndex].SetCellValue(++index);
                AssignCellData(row, item);
            }
            AssignTotal(sheet, templateSheet, data, RowContentTemplate + index, metaModel);
        }

        private static void AssignCellData(IRow row, ExchangeRunningTicket item)
        {
            var isFancyBet = item.CategoryType == 2;
            var oddsAvgStrings = isFancyBet ? $"{item.FancyBetRuns.ToString("0")} / {item.OddsAvg.ToString("0")}" : item.OddsAvg.ToString("0.00");
            row.Cells[ColumnExternalID].SetCellValue(item.NickName);
            row.Cells[ColumnSystemID].SetCellValue(item.Username);
            row.Cells[ColumnBetId].SetCellValue(item.BetId);
            row.Cells[ColumnBetPlaced].SetCellValue(item.BetPlacedString);
            row.Cells[ColumnMarket].SetCellValue($"{item.EventTypeName} > {item.EventName} > {item.MarketName}");
            row.Cells[ColumnSelection].SetCellValue(item.SelectionName);
            row.Cells[ColumnType].SetCellValue(item.Type);
            row.Cells[ColumnOddsAvg].SetCellValue(oddsAvgStrings);
            row.Cells[ColumnStake].SetCellValue((double)item.Stake);
        }

        protected virtual void AssignTitle(ISheet sheet, ISheet templateSheet, ExchangeRunningTicketMetaModel metaModel)
        {
            var baseCurrency = metaModel.BaseCurrency.ToUpperInvariant();
            var baseCurrencyText = metaModel.IsConvertAllToBaseCurrency ? $", {ViewResources.BaseCurrency}: {baseCurrency}" : "";
            var reportTitle = $"{ViewResources.BettingTicketCenter}, GMT+8{baseCurrencyText}";

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

        protected virtual void AssignTotal(ISheet sheet, ISheet templateSheet, ExchangeResult<ExchangeRunningTicket> data, int index, ExchangeRunningTicketMetaModel metaModel)
        {
            var row = _builder.CopyRow(sheet, templateSheet, RowTotalTemplate, index);
            row.Cells[0].SetCellValue(string.Format(ViewResources.TotalInWithParam, metaModel.BaseCurrency));
            row.Cells[ColumnStake].SetCellValue((double)data.SumStake);

            _builder.MergeCols(sheet, 0, ColumnStake - 1, index);
        }
    }
}
