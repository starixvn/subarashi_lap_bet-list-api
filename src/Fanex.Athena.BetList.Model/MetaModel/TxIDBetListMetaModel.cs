namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class TxIDBetListMetaModel : BaseBettingTicketMetaModel
    {
        public string TxID { get; set; }

        public override bool IsValid => !string.IsNullOrWhiteSpace(TxID);
    }
}