using Fanex.Data;

namespace Fanext.Athena.TestHelper
{
    public static class TestHelper
    {
        public static void SetUpDBObjects(IObjectDb objectDb)
        {
            DbSettingProviderManager.StartTestingSession();
            CreateObjectDbFunc createObjectDbFunc = (key, timeout) => objectDb;
            ObjectDbFactory.Mock(createObjectDbFunc);
        }
    }
}