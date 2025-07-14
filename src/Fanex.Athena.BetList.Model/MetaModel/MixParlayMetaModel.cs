using Fanex.Athena.Common.Model.MetaModel;
using Fanex.Core.Utils;

namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class MixParlayMetaModel : IValidationMetaModel, IAgentMetaModel, ICurrencyIdMetaModel,
                                      IMetaModelProductCodeList, ISubsidiarySearchMetaModel
    {
        public int Page { get; set; }

        public int Rows { get; set; }

        public int BetType { get; set; }

        public string BaseCurrency { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public byte SortType { get; set; }

        public byte SortOrder { get; set; }

        public string AgentListXml { get; set; }

        public int CurrencyId { get; set; }

        public string ProductCodeList { get; set; }

        public string OriginalProductCodeList { get; set; }

        public string SubsidiaryList { get; set; }

        public bool SubsidiarySearch { get; set; }

        public bool IsValid => FromDate <= ToDate && IsValidBaseCurrency;

        private bool IsValidBaseCurrency => string.IsNullOrEmpty(BaseCurrency) || Common.Constant.SystemConstants.BaseCurrencyList
            .Any(x => x.Equals(BaseCurrency, StringComparison.OrdinalIgnoreCase));
    }
}