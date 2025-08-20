namespace Fanex.Athena.BetList.Service
{
    using DataAccess.Interfaces;
    using Interfaces;

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