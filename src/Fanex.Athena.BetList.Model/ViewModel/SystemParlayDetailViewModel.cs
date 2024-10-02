namespace Fanex.Athena.BetList.Model.ViewModel
{
    using Fanex.Athena.BetList.Model.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SystemParlayDetailViewModel
    {
        public IEnumerable<IGrouping<DateTime, SystemParlayDetailItem>> DataList { get; set; }

        public decimal Outstanding { get; set; }

        public decimal Total { get; set; }

        public bool ShowPlayerComm { get; set; }
    }
}