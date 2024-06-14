namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class BettingTicketCenterOption
    {
        public bool ShowTotal { get; set; }

        public bool ShowSumStake { get; set; }

        public bool ShowWinLoss { get; set; }

        public bool ShowLicenseeWl { get; set; }

        public bool IsAllowedCustomerComm { get; set; }

        public bool ShowSystemId { get; set; }

        public bool ShowExternalId { get; set; }

        public bool ShowTxId { get; set; }

        public bool IsShowIpvn { get; set; }

        public bool IsShowIp { get; set; }

        public int SpecialPlatform { get; set; }
    }
}