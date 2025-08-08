namespace Fanex.Athena.BetListAPI.Host.DbSettingConfig;

using Common.Services.Interfaces;
using Data;
using Data.SqlServer;

public class ShardDbSettingProvider(IShardInfoAccessor shardInfoAccessor)
{
    public static string ProviderName => nameof(ShardDbSettingProvider);

    public IObjectDb CreateObjectDbFunc(string settingKey, int? commandTimeout, string serverIp)
    {
        var server = serverIp;

        if (shardInfoAccessor.TryUseShardDB(settingKey, out var dbServer))
        {
            server = dbServer;
            settingKey = shardInfoAccessor.GetShardSPKey(settingKey);
        }

        return new SqlObjectDb(settingKey, commandTimeout, server);
    }
}