namespace Fanex.Athena.BetList.Model.DTO
{
    public class SystemParlayDetailData
    {
        public decimal Outstanding { get; set; }

        public IEnumerable<SystemParlayDetailSerial> SerialList { get; set; }

        public IEnumerable<SystemParlayDetailTicket> TicketList { get; set; }
    }
}