using Fanex.Athena.Common.Services.Interfaces;

namespace Fanex.Athena.Common.Services
{
    public class ShardInfoAccessor : IShardInfoAccessor
    {
        const string PrefixSharding = "shard_";
        const string PrefixSiteDB = "sitedb_";

        private readonly IContextService _contextService;

        public ShardInfoAccessor(IContextService contextService)
        {
            _contextService = contextService;
        }

        public bool TryUseShardDB(string key, out string dbServer)
        {
            if (IsShardSP(key) && UseShardDB)
            {
                dbServer = SiteDBServer;
                return true;
            }
            dbServer = null;
            return false;
        }

        public bool UseShardDB => !ForceIgnoreSharding && !string.IsNullOrEmpty(SiteDBServer);

        public string GetShardSPKey(string key) => key.StartsWith(PrefixSiteDB, StringComparison.OrdinalIgnoreCase)
            ? key : $"SiteDB_{key.Substring(PrefixSharding.Length)}";

        private bool IsShardSP(string key) => key.StartsWith(PrefixSharding, StringComparison.OrdinalIgnoreCase)
            || key.StartsWith(PrefixSiteDB, StringComparison.OrdinalIgnoreCase);

        private string SiteDBServer => _contextService.SiteDBServer;

        private bool ForceIgnoreSharding => _contextService.ForceIgnoreSharding;
    }
}
