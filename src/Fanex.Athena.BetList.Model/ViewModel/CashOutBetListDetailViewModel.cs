namespace Fanex.Athena.BetList.Model.ViewModel
{
    using DTO;
    using System.Collections.Generic;

    public class CashOutBetListDetailViewModel
    {
        public TicketText OriginalTicketText { get; set; }

        public LrfTicket OriginalTicket { get; set; }

        public IEnumerable<CashOutTicket> CashOutTicketList { get; set; }

        public IEnumerable<CashOutTicketText> CashOutTicketTextList { get; set; }

        public bool IsCashOut { get; set; }

        public bool IsHighlightOriginalTicket { get; set; }
    }
}