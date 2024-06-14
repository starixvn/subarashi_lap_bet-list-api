using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Data;

namespace Fanex.Athena.BetListAPI.Site.DbSettingConfig
{
    public static class DbSettingProviderManagerExtension
    {
        public static DbSettingProviderManager UseShardDbSettingProvider(this DbSettingProviderManager dbSettingProviderManager, IShardInfoAccessor accessor, string spNameSource)
            => dbSettingProviderManager.Use(new ShardDbSettingProvider(accessor,
                    resourcePath: spNameSource));
    }
}