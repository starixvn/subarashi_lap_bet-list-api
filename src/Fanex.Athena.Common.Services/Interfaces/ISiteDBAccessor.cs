namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface ISiteDBAccessor
    {
        string SiteDBServer { get; }

        bool ForceIgnoreSharding { get; }
    }
}
