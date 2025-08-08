namespace Fanex.Athena.BetList.Model.MetaModel
{
    using Common.Constant;
    using Core.Utils;

    public class ExchangeRunningTicketMetaModel : IAgentMetaModel, ICurrencyIdListMetaModel
    {
        public ExchangeRunningTicketMetaModel()
        {
            SortType = (byte)SortTypes.BetPlaced;
            SortOrder = (byte)SortOrders.Descending;
        }

        public string AgentListXml { get; set; }

        public string Username { get; set; } = string.Empty;

        public string BaseCurrency { get; set; }

        public string CurrencyIdList { get; set; }

        public bool IsConvertAllToBaseCurrency { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public byte SortType { get; set; }

        public byte SortOrder { get; set; }
    }
}