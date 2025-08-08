namespace Fanex.Athena.BetList.Model.ViewModel
{
    using DTO;

    public class BetListResultViewModel
    {
        public List<TicketText> Main { get; set; }

        public BetListTotalData Total { get; set; }

        public string LeagueName { get; set; }
    }
}