namespace Fanex.Athena.BetList.Model.DTO
{
    using Common.Utility.Collections;
    using Fanex.BetList.Core.Entities;
    using System.Collections.Generic;

    public class TicketResult
    {
        public SortableList<LrfTicket> MainTicketList { get; set; } = new SortableList<LrfTicket>();

        public List<ITicketData> TicketDataList { get; set; } = new List<ITicketData>();

        // Sum value
        public int TotalRows { get; set; }

        public decimal SumWinLoss { get; set; }

        public decimal SumBetEffective { get; set; }

        public decimal SumLicenseeWL { get; set; }

        public decimal SumComm { get; set; }

        public decimal SumPlayerComm { get; set; }

        public decimal SumStake { get; set; }
    }
}