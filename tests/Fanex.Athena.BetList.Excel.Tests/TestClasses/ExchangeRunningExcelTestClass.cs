namespace Fanex.Athena.BetList.Excel.Tests.TestClasses
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using Excel.Builders;
    using NPOI.SS.UserModel;

    public class ExchangeRunningExcelTestClass(IExcelBuilder builder) : ExchangeRunningExcel(builder)
    {
        public void ProtectedAssignHeader(ISheet sheet, ISheet templateSheet)
            => base.AssignHeader(sheet, templateSheet);

        public void ProtectedAssignTitle(ISheet sheet, ISheet templateSheet, ExchangeRunningTicketMetaModel metaModel)
            => base.AssignTitle(sheet, templateSheet, metaModel);

        public void ProtectedAssignSheetData(ISheet sheet, ISheet templateSheet, ExchangeResult<ExchangeRunningTicket> data, ExchangeRunningTicketMetaModel metaModel)
            => base.AssignSheetData(sheet, templateSheet, data, metaModel);

        public void ProtectedAssignNoInfoRow(ISheet sheet, ISheet templateSheet)
            => base.AssignNoInfoRow(sheet, templateSheet);

        public void ProctectedAssignTotal(ISheet sheet, ISheet templateSheet, ExchangeResult<ExchangeRunningTicket> data, int index, ExchangeRunningTicketMetaModel metaModel)
            => base.AssignTotal(sheet, templateSheet, data, index, metaModel);

    }
}
