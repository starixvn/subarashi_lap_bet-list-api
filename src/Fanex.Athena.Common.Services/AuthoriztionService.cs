using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;

namespace Fanex.Athena.Common.Services
{
    public class AuthoriztionService : IAuthorizationService
    {
        private readonly IAuthorizationDataAccess _authorizationDataAccess;

        public AuthoriztionService(IAuthorizationDataAccess authorizationDataAccess)
        {
            _authorizationDataAccess = authorizationDataAccess;
        }

        public bool HasPermissionOnData(IEnumerable<KeyValuePair<int, int[]>> currencyAgents, int custId, long transId)
        {
            if (custId == 0 && transId == 0)
            {
                return true;
            }

            var item = _authorizationDataAccess.IsCustomerBelonging(custId, transId);

            return currencyAgents.Any(i => i.Key == item.Item1 && i.Value.Contains(item.Item2));
        }
    }
}
