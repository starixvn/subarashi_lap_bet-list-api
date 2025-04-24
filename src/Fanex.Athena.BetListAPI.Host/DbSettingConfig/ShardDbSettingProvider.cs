using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Data;
using Fanex.Data.SqlServer;

namespace Fanex.Athena.BetListAPI.Host.DbSettingConfig
{
    public class ShardDbSettingProvider
    {
        private readonly IShardInfoAccessor _shardInfoAccessor;

        public ShardDbSettingProvider(IShardInfoAccessor shardInfoAccessor)
        {
            _shardInfoAccessor = shardInfoAccessor;
        }

        public static string ProviderName => nameof(ShardDbSettingProvider);

        public IObjectDb CreateObjectDbFunc(string settingKey, int? commandTimeout, string serverIp)
        {
            var shardSPKey = _shardInfoAccessor.GetShardSPKey(settingKey);

            if (_shardInfoAccessor.TryUseShardDB(shardSPKey, out var dbServer))
            {
                serverIp = dbServer;
            }

            return new SqlObjectDb(settingKey, commandTimeout, serverIp);
        }
    }
}