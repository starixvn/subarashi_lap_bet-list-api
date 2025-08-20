namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;

    public interface IIPDataAccess
    {
        Tuple<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>> GetVNIPs();

        string GetIpLocation(string IPAddress);
    }
}