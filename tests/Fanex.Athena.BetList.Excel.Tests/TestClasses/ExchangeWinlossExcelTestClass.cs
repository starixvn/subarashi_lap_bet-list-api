namespace Fanex.Athena.BetList.Excel.Tests.TestClasses
{
    using BetList.Model.MetaModel;
    using BetList.Model.ViewModel;
    using Excel.Builders;
    using NPOI.SS.UserModel;

    public class ExchangeWinlossExcelTestClass(IExcelBuilder builder) : ExchangeWinLossExcel(builder)
    {
        public void ProtectedAssignHeader(ISheet sheet, ISheet templateSheet)
            => base.AssignHeader(sheet, templateSheet);

        public void ProtectedAssignTitle(ISheet sheet, ISheet templateSheet, ExchangeBetListMetaModel metaModel)
            => base.AssignTitle(sheet, templateSheet, metaModel);

        public void ProtectedAssignSheetData(ISheet sheet, ISheet templateSheet, ExchangeBetListViewModel data)
            => base.AssignSheetData(sheet, templateSheet, data);

        public void ProtectedAssignNoInfoRow(ISheet sheet, ISheet templateSheet)
            => base.AssignNoInfoRow(sheet, templateSheet);

        public void ProctectedAssignTotal(ISheet sheet, ISheet templateSheet, ExchangeBetListViewModel data, int index)
            => base.AssignTotal(sheet, templateSheet, data, index);

    }
}
