namespace Fanex.Athena.BetList.Excel
{
    using BetListNPOI.SS.UserModel;
    using Model;

    public interface IBettingTicketCenterExcel : IDisposable
    {
        IWorkbook Export(BettingTicketCenterExcelModel excelModel);
    }
}