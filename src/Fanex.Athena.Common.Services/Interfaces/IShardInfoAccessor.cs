namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface IShardInfoAccessor
    {
        bool TryUseShardDB(string key, out string dbServer);

        string GetShardSPKey(string key);

        bool UseShardDB { get; }
    }
}