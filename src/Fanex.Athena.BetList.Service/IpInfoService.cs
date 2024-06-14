using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Service.Interfaces;

namespace Fanex.Athena.BetList.Service
{
    public class IpInfoService : IIpInfoService
    {
        private readonly IIPDataAccess _ipDataAccess;

        public IpInfoService(IIPDataAccess ipDataAccess)
        {
            _ipDataAccess = ipDataAccess;
        }

        public string GetIpLocation(string IPAddres)
        {
            return _ipDataAccess.GetIpLocation(IPAddres);
        }
    }
}