namespace Fanex.Athena.BetList.Model.ViewModel
{
    using DTO;

    public class SystemParlayDetailViewModel
    {
        public IEnumerable<IGrouping<DateTime, SystemParlayDetailItem>> DataList { get; set; }

        public decimal Outstanding { get; set; }

        public decimal Total { get; set; }

        public bool ShowPlayerComm { get; set; }
    }
}