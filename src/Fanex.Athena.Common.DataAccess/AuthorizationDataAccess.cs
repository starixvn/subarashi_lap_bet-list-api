using Fanex.Athena.Common.DataAccess.Interfaces;

namespace Fanex.Athena.Common.DataAccess
{
    public class AuthorizationDataAccess : BaseDataAccess, IAuthorizationDataAccess
    {
        public (int CurrencyId, int CustId) IsCustomerBelonging(int custId, long transId)
        {
            using (var db = GetObjectDb("Lrf_CheckBelonging_GetAgentInfo"))
            {
                db.SetParameterValue("transid", transId);
                db.SetParameterValue("custid", custId);
                using (var reader = db.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (Convert.ToInt32(reader["CurrencyId"]), Convert.ToInt32(reader["CustId"]));
                    }
                }
            }
            return (0, 0);
        }
    }
}
