using Fanex.Core.Utils;

namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class CashOutBetListDetailMetaModel : IPlayerComMetaModel
    {
        public string BaseCurrency { get; set; }

        public long TransId { get; set; }

        public long RefNo { get; set; }

        public int BetType { get; set; }

        public long BetdaqId { get; set; }

        public string AgentListXml { get; set; }

        public bool IsCashOut { get; set; }

        public bool HasPlayerComm { get; set; }

        public bool IsMobileView { get; set; }

        public bool IsShowIpvn { get; set; }

        public bool IsShowIp { get; set; }

        public bool HasPlayerCom { get; set; }
    }
}
