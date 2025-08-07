using Fanex.Athena.Common.Services.Interfaces;

namespace Fanex.Athena.Common.Services
{
    public class ShardInfoAccessor : IShardInfoAccessor
    {
        private const string PrefixSharding = "shard_";
        private const string PrefixSiteDB = "sitedb_";

        private readonly ISiteDBAccessor _contextService;

        public ShardInfoAccessor(ISiteDBAccessor contextService)
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

        public string GetShardSPKey(string key) => key.StartsWith(PrefixSharding, StringComparison.OrdinalIgnoreCase) ? $"SiteDB_{key.Substring(PrefixSharding.Length)}" : key;

        private bool IsShardSP(string key) => key.StartsWith(PrefixSharding, StringComparison.OrdinalIgnoreCase)
            || key.StartsWith(PrefixSiteDB, StringComparison.OrdinalIgnoreCase);

        private string SiteDBServer => _contextService.SiteDBServer;

        private bool ForceIgnoreSharding => _contextService.ForceIgnoreSharding;
    }
}