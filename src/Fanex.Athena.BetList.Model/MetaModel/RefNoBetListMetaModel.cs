namespace Fanex.Athena.BetList.Model.MetaModel
{
    using Fanex.Athena.Common.Model.MetaModel;

    public class RefNoBetListMetaModel : BaseBettingTicketMetaModel, IShardDBMetaModel
    {
        public long RefNo { get; set; }
        public string TxId { get; set; }
        public override bool IsValid => RefNo > 0 || !string.IsNullOrEmpty(TxId);

        public bool UseShardDB { get; set; }
    }
}