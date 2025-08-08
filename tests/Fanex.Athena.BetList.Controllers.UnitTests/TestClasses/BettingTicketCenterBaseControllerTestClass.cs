namespace Fanex.Athena.BetList.Controllers.UnitTests.TestClasses
{
    using CustomActionResult;
    using Excel;
    using Excel.Model;
    using Model.DTO;
    using Model.MetaModel;
    using Service.Interfaces;

    public class BettingTicketCenterBaseControllerTestClass(
        ITicketBuilderService ticketBuilderService,
        IBettingTicketCenterExcel bettingTicketCenterExcel)
        : BettingTicketCenterBaseController(ticketBuilderService, bettingTicketCenterExcel)
    {
        public new string GetTitle(BaseBettingTicketMetaModel metaModel)
            => BettingTicketCenterBaseController.GetTitle(metaModel);

        public new ExcelResult ExportExcel(BettingTicketCenterExcelModel excelModel)
            => base.ExportExcel(excelModel);

        public new object ReturnData(TicketResult ticket,
            BettingTicketCenterOption option,
            ICommonValueMetaModel metaModel)
            => base.ReturnData(ticket, option, metaModel);
    }
}