using Fanex.Athena.BetList.Model.Constants;
using Fanex.Core.Utils;


namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class ExchangeRunningTicketMetaModel : IAgentMetaModel, ICurrencyIdListMetaModel
    {
        public string AgentListXml { get; set; }

        public string Username { get; set; } = string.Empty;

        public string BaseCurrency { get; set; }

        public string CurrencyIdList { get; set; }

        public bool IsConvertAllToBaseCurrency { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public SortByExchange SortBy { get; set; } = SortByExchange.BetPlaced;

        public SortOrderExchange SortOrder { get; set; } = SortOrderExchange.Desc;

        public string Sort => GetSort();

        private string GetSort()
        {
            var sortByName = SortBy == SortByExchange.BetPlaced ? ExchangeConstant.BetPlaced : ExchangeConstant.Stake;
            var sortOrderName = SortOrder == SortOrderExchange.Desc ? ExchangeConstant.Desc : ExchangeConstant.Asc;
            return $"{sortByName} {sortOrderName}";
        }
    }
}
