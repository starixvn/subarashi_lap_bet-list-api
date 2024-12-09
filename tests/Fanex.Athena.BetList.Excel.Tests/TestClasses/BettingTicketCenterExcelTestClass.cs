using BetListNPOI.SS.UserModel;
using Fanex.Athena.BetList.Excel.Builders;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Service.Interfaces;

namespace Fanex.Athena.BetList.Excel.Tests.TestClasses
{
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
