namespace Fanex.Athena.BetList.Model.DTO
{
    public class ExchangeBetList
    {
        public ExchangeSumWinLoss Ticket { get; set; }

        public IEnumerable<ExchangeTicket> Details { get; set; }
    }
}