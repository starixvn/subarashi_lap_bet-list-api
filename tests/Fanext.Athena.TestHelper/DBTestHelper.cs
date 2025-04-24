using Fanex.Data;
using Microsoft.Data.SqlClient;

namespace Fanex.Athena.TestHelper
{
    public static class DBTestHelper
    {
        public static void SetUpDBObjects(IObjectDb objectDb)
        {
            DbSettingProviderManager.StartTestingSession();
            CreateObjectDbFunc createObjectDbFunc = (key, timeout, serverIp) => objectDb;
            ObjectDbFactory.Mock(createObjectDbFunc);
        }

        public static SqlException? MakeSqlException()
        {
            SqlException? exception = null;
            try
            {
                SqlConnection conn = new SqlConnection(@"Data Source=.;Database=GUARANTEED_TO_FAIL;Connection Timeout=1");
                conn.Open();
            }
            catch (SqlException ex)
            {
                exception = ex;
            }
            return exception;
        }
    }
}