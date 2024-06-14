namespace Fanex.Athena.BetList.Service.Interfaces
{
    public interface IIPService
    {
        bool IsVNIP(string ipAddress);

        bool IsAlwaysIpvn(string ipAddress);
    }
}