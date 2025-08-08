namespace Fanex.Athena.BetList.Excel.Tests.TestClasses
{
    using BetListNPOI.SS.UserModel;
    using Excel.Builders;
    using Model;
    using Service.Interfaces;

    public class BettingTicketCenterExcelTestClass : BettingTicketCenterExcel
    {
        public BettingTicketCenterExcelTestClass(IBettingTicketCenterExcelBuilder builder, IIPService ipService) : base(builder, ipService)
        {
        }

        public void ProtectedBuildSheets(BettingTicketCenterExcelModel excelModel)
            => base.BuildSheets(excelModel);

        public void ProtectedAssignSheetData(BettingTicketCenterExcelModel excelModel, int sheetIndex, int endIndex)
            => base.AssignSheetData(excelModel, sheetIndex, endIndex);

        public void ProtectedAddFooter(BettingTicketCenterExcelModel excelModel)
            => base.AddFooter(excelModel);

        public void ProtectedFixBugIOSMobile(IWorkbook workbook)
            => base.FixBugIOSMobile(workbook);
    }
}
