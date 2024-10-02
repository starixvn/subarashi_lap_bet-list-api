namespace Fanex.Athena.BetList.Model.ViewModel
{
    using Fanex.Athena.BetList.Model.DTO;
    using System.Collections.Generic;

    public class ExchangeBetListViewModel
    {
        public IEnumerable<ExchangeBetList> MainData { get; set; }

        public decimal GrandTotal { get; set; }
    }
}
