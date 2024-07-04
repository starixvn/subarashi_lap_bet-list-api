namespace Fanex.Athena.BetList.Model.MetaModel
{
    using Fanex.Athena.Common.Model.MetaModel;
    using System;

    public class MixParlayDetailMetaModel : IBalanceUpMetaModel, IShardDBMetaModel
    {
        public long TransId { get; set; }

        public int CustId { get; set; }

        public bool IsShowIpvn { get; set; }

        public bool IsShowIp { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public bool UseShardDB { get; set; }

        public bool IsHideSaba { get; set; }
    }
}