namespace Fanex.Athena.Common.Services
{
    using System.Collections.ObjectModel;
    using System.Security.Claims;
    using Constant;
    using Interfaces;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class ContextService : IContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISiteDBAccessor _siteDBAccessor;

        protected HttpContext Context => _httpContextAccessor.HttpContext;

        protected ClaimsPrincipal User => Context.User;

        public ContextService(IHttpContextAccessor httpContextAccessor,
            ISiteDBAccessor siteDBAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _siteDBAccessor = siteDBAccessor;
        }

        public bool IsAllowedCustomerComm => GetClaimValue<bool>("IsAllowedCustomerComm");

        public bool IsEnableBA => GetClaimValue<bool>("IsEnableBA");

        public bool IsEnableSingleWallet => GetClaimValue<bool>("IsEnableSingleWallet");

        public string SiteName => GetClaimValue("SiteName");

        public int LicenseeId => GetClaimValue<int>("LicenseeId");

        public int SpecialPlatform => GetClaimValue<int>("SpecialPlatform");

        public int SpecialPlatformType => GetClaimValue<int>("SpecialPlatformType");

        public string UserName => GetClaimValue("UserID");

        public int UserID => GetClaimValue<int>("UserSEQ");

        public bool IsFakeLicensee => GetClaimValue<bool>("IsFakeLicensee");

        public bool IsAdministrator => GetClaimValue<bool>("Administrator");

        public string SiteDBServer => _siteDBAccessor.SiteDBServer;

        public IEnumerable<string> OfferedSubProductList
        {
            get
            {
                var offeredSubProductList = GetClaimValue("OfferedSubProducts");
                return string.IsNullOrEmpty(offeredSubProductList) ? Enumerable.Empty<string>() : offeredSubProductList.Split(',');
            }
        }

        public bool IsInRoles(params string[] roles) => roles != null && Array.Exists(roles, _ => User.IsInRole(_));

        public string ClientIP => Context.Connection.RemoteIpAddress.ToString();

        private IReadOnlyDictionary<int, int[]> _agentList;

        public IReadOnlyDictionary<int, int[]> AgentList
        {
            get
            {
                if (_agentList is null)
                {
                    var currencyAgentList = RawCurrencyAgentList;
                    var dicClaims = currencyAgentList.ToDictionary(_ => _.CurrencyId, _ => _.AgentId);
                    _agentList = new ReadOnlyDictionary<int, int[]>(dicClaims);
                }
                return _agentList;
            }
        }

        private IReadOnlyDictionary<int, string> _currencyList;

        public IReadOnlyDictionary<int, string> CurrencyList
        {
            get
            {
                if (_currencyList is null)
                {
                    var currencyAgentList = RawCurrencyAgentList;
                    var dicClaims = currencyAgentList.ToDictionary(_ => _.CurrencyId, _ => _.Currency);
                    _currencyList = new ReadOnlyDictionary<int, string>(dicClaims);
                }
                return _currencyList;
            }
        }

        private IEnumerable<CurrencyAgentInfo> _rawCurrencyAgentList;

        protected internal IEnumerable<CurrencyAgentInfo> RawCurrencyAgentList
        {
            get
            {
                if (_rawCurrencyAgentList is null)
                {
                    var agentClaims = User.Claims.Where(_ => _.Type.Equals("CurrencyAgent", StringComparison.OrdinalIgnoreCase));
                    _rawCurrencyAgentList = agentClaims.Select(agent => JsonConvert.DeserializeObject<CurrencyAgentInfo>(agent.Value));
                }

                return _rawCurrencyAgentList;
            }
        }

        public bool ForceIgnoreSharding => _siteDBAccessor.ForceIgnoreSharding;

        public byte SubsidiaryViewType
        {
            get
            {
                if (string.IsNullOrEmpty(GetClaimValue("VendorID")))
                {
                    return Constant.SubsidiaryViewType.NotManagement;
                }

                int? GetWhiteLabelID()
                {
                    var value = GetClaimValue<int>("WhiteLabelID");
                    if (value < 0)
                    {
                        return null;
                    }
                    return value;
                }

                const int SubsidiaryLevel = 2;

                if (GetClaimValue<bool>("IsEnabledSubsidiary") && GetClaimValue<int>("UserLevel") < SubsidiaryLevel)
                {
                    if (GetClaimValue<bool>("IsMasterLicensee") && !GetWhiteLabelID().HasValue)
                    {
                        return Constant.SubsidiaryViewType.MasterManagement;
                    }

                    return Constant.SubsidiaryViewType.NormalManagement;
                }

                return Constant.SubsidiaryViewType.NotManagement;
            }
        }

        public bool IsShowIP => GetClaimValue<bool>("IsShowIP");

#pragma warning disable S1067 // Expressions should not be too complex
        public bool IsShowIpvn => IsShowIP
            && !IsLrfUser 
            && (IsAdministrator
            || IsInRoles(Roles.BetlistViewIpvn, Roles.PortalSuperAdmin)
            || IsHaveConfigShowIpVnInStatementService);
#pragma warning restore S1067 // Expressions should not be too complex

        private bool IsHaveConfigShowIpVnInStatementService => (User.IsInRole(Roles.StatementServiceRoleUser) && GetClaimValue<bool>("IsShowIPVN"));

        private bool IsLrfUser => GetClaimValue<string>("name").StartsWith("lrf_");

        private T GetClaimValue<T>(string claimType)
        {
            var value = GetClaimValue(claimType);
            return value is null ? default : (T)Convert.ChangeType(value, typeof(T));
        }

        private string GetClaimValue(string claimType)
            => User.FindFirst(claimType)?.Value;

        protected internal class CurrencyAgentInfo
        {
            public int[] AgentId { get; set; }

            public int CurrencyId { get; set; }

            public string Currency { get; set; }
        }

    }

}
