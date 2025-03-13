namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class SingleCustomersBetListMetaModel : BaseBettingTicketMetaModel
    {
        public string Platform { get; set; }

        public string LiveStatus { get; set; }

        public string SportIds { get; set; }

        public string BetTypeIds { get; set; }

        public string SportListXML { get; set; }

        public int CashOutMode { get; set; }

        public override bool IsValid => base.IsValid && !string.IsNullOrWhiteSpace(UserName);
    }
}