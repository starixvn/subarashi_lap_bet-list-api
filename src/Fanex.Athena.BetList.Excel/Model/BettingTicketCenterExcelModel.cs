namespace Fanex.Athena.BetList.Excel.Model
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;

    public class BettingTicketCenterExcelModel
    {
        public TicketResult Data { get; set; }

        public int LicenseeId { get; set; }

        public string TotalText { get; set; }

        public string Title { get; set; }

        public BettingTicketCenterOption Option { get; set; }
    }
}
