
namespace Fanex.Athena.Common.DataAccess.Interfaces
{
    public interface IAuthorizationDataAccess
    {
        (int CurrencyId, int CustId) IsCustomerBelonging(int custId, long transId);
    }
}
