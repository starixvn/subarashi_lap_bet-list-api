namespace Fanex.Athena.BetListAPI.Host.DbSettingConfig;

using Common.Services.Interfaces;
using Data;
using Data.SqlServer;

public class ShardDbSettingProvider(IShardInfoAccessor shardInfoAccessor)
{
    public static string ProviderName => nameof(ShardDbSettingProvider);

    public IObjectDb CreateObjectDbFunc(string settingKey, int? commandTimeout, string serverIp)
    {
        var shardSpKey = shardInfoAccessor.GetShardSPKey(settingKey);
        var server = serverIp;

        if (shardInfoAccessor.TryUseShardDB(shardSpKey, out var dbServer))
        {
            server = dbServer;
        }

        return new SqlObjectDb(settingKey, commandTimeout, server);
    }
}