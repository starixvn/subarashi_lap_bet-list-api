using Fanex.Athena.BetList.Model.DTO;

namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    public interface IIPDataAccess
    {
        Tuple<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>> GetVNIPs();

        string GetIpLocation(string IPAddress);
    }
}