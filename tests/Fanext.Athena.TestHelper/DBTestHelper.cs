using Fanex.Data;

namespace Fanex.Athena.TestHelper
{
    public static class DBTestHelper
    {
        public static void SetUpDBObjects(IObjectDb objectDb)
        {
            DbSettingProviderManager.StartTestingSession();
            CreateObjectDbFunc createObjectDbFunc = (key, timeout) => objectDb;
            ObjectDbFactory.Mock(createObjectDbFunc);
        }
    }
}