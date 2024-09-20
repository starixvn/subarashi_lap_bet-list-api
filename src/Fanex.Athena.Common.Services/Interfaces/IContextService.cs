namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface IContextService
    {
        string SiteName { get; }

        int LicenseeId { get; }

        string UserName { get; }

        int UserID { get; }

        string SiteDBServer { get; }

        IReadOnlyDictionary<int, int[]> AgentList { get; }

        IReadOnlyDictionary<int, string> CurrencyList { get; }

        bool IsAllowedCustomerComm { get; }

        bool IsEnableBA { get; }

        bool IsEnableSingleWallet { get; }

        bool IsFakeLicensee { get; }

        IEnumerable<string> OfferedSubProductList { get; }

        bool IsInRoles(params string[] roles);

        bool ForceIgnoreSharding { get; }

        int SpecialPlatform { get; }

        int SpecialPlatformType { get; }

        byte SubsidiaryViewType { get; }

        bool IsAdministrator { get; }

        bool IsShowIP { get; }

        bool IsShowIpvn { get; }
    }

}
