namespace Fanex.Athena.BetList.Model.ViewModel
{
    using DTO;

    public class ExchangeBetListViewModel
    {
        public IEnumerable<ExchangeBetList> MainData { get; set; }

        public decimal GrandTotal { get; set; }
    }
}
