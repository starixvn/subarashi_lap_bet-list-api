namespace Fanex.Athena.Common.DataAccess
{
    using Data;

    public class BaseDataAccess
    {
        protected virtual IObjectDb GetObjectDb(string spname, int timeout = 120)
        {
            return ObjectDbFactory.CreateInstance(spname, timeout);
        }
    }
}