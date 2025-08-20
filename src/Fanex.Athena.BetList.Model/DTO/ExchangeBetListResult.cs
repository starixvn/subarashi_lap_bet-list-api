namespace Fanex.Athena.BetList.Model.DTO
{
    public class ExchangeBetListResult
    {
        public IEnumerable<ExchangeSumWinLoss> SumWinLossList { get; set; }

        public IEnumerable<ExchangeTicket> TicketList { get; set; }

        public decimal GrandTotal { get; set; }
    }
}