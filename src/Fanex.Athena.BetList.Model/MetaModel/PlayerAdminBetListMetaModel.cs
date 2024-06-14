namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class PlayerAdminBetListMetaModel : ICommonValueMetaModel
    {
        public int CustId { get; set; }

        public string SortBy { get; set; }

        public string SortOrder { get; set; }

        public string AgentListXml { get; set; }

        public string Currency { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string BaseCurrency { get; set; } = string.Empty;

        public bool IsAllowTxIDColumn { get; set; }

        public bool IsShowIpvn { get; set; }

        public bool IsShowIp { get; set; }

        public int SpecialPlatform { get; set; }
    }
}