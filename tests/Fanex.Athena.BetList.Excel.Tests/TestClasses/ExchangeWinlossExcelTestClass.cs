using Fanex.Athena.BetList.Excel.Builders;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using NPOI.SS.UserModel;

namespace Fanex.Athena.BetList.Excel.Tests.TestClasses
{
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
