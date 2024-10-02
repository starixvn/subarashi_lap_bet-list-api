namespace Fanex.Athena.BetList.Model.ViewModel
{
    using Fanex.Athena.BetList.Model.DTO;
    using System.Collections.Generic;

    public class BetListResultViewModel
    {
        public List<TicketText> Main { get; set; }

        public BetListTotalData Total { get; set; }

        public string LeagueName { get; set; }
    }
}