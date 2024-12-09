using Fanex.Athena.BetList.Controllers.CustomActionResult;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;

namespace Fanex.Athena.BetList.Controllers.UnitTests.TestClasses
{
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