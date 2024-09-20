

namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface IAuthorizationService
    {
        bool HasPermissionOnData(IEnumerable<KeyValuePair<int, int[]>> currencyAgents, int custId, long transId);
    }
}
