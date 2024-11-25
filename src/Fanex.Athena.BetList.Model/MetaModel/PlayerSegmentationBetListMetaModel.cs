
using Fanex.Core.Utils;

namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class PlayerSegmentationBetListMetaModel : ICommonValueMetaModel, IPlayerComMetaModel, IAgentMetaModel
    {
        public int CustId { get; set; }

        public string AgentListXml { get; set; }

        public IEnumerable<string> ListProductCode { get; set; }

        public bool IsAllowedCustomerComm { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string BaseCurrency { get; set; } = string.Empty;

        public bool IsShowIpvn { get; set; }

        public bool IsShowIp { get; set; }

        public int SpecialPlatform { get; set; }

        public bool HasPlayerCom { get; set; }
    }
}