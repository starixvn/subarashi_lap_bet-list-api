namespace Fanex.Athena.BetList.Service.Optiions
{
    public record class BuildTicketOption
    {
        public bool IsAllowedCustomerComm { get; init; }

        public bool IsHideSaba { get; init; }

        public bool IsMobileView { get; init; }

        public bool IsShowIp { get; init; } = true;

        public bool IsShowIpvn { get; init; }

        public bool ShowOdds { get; init; }

        public void Deconstruct(
            out bool isAllowedCustomerComm,
            out bool isHideSaba,
            out bool isMobileView,
            out bool isShowIp,
            out bool isShowIpvn,
            out bool showOdds)
        {
            isAllowedCustomerComm = IsAllowedCustomerComm;
            isHideSaba = IsHideSaba;
            isMobileView = IsMobileView;
            isShowIp = IsShowIp;
            isShowIpvn = IsShowIpvn;
            showOdds = ShowOdds;
        }
    }
}
