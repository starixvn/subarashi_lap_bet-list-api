using Fanex.Core.Utils;

namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class ExchangeBetListDetailMetaModel : BaseExchangeBetListMetaModel, IValidationMetaModel
    {
        public long BetId { get; set; }

        public string Type { get; set; }

        public override bool IsValid => base.IsValid && BetId > 0;
    }
}