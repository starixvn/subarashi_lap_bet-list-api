using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Data;
using NSubstitute;
using System.Data;

namespace Fanex.Athena.TestHelper
{
    public class BaseDataAccessHelperTest<T> where T : class
    {
        protected const int CurrencyId = 123;

        protected const string CurrencyName = "aaaaaaaaa";
        protected ICurrencyDataAccess CurrencyDataAccess;

        protected IObjectDb ObjectDb { get; set; }

        protected IDataReader Reader { get; set; }

        protected T DataAccess { get; set; }

        protected IDictionary<int, string> UserCurrencyList { get; } = new Dictionary<int, string>
        {
            { CurrencyId, CurrencyName }, { 1, "USD" }
        };

        [SetUp]
        public virtual void SetUp()
        {
            CurrencyDataAccess = Substitute.For<ICurrencyDataAccess>();
            CurrencyDataAccess.GetCurrencyListAsync().Returns(Task.FromResult(UserCurrencyList));

            ObjectDb = Substitute.For<IObjectDb>();
            DBTestHelper.SetUpDBObjects(ObjectDb);
            SetUpDataAccess();
            ObjectDb.ClearReceivedCalls();
            Reader = Substitute.For<IDataReader>();
            ObjectDb.ExecuteReader().Returns(Reader);
            ObjectDb.ExecuteReader(Arg.Any<object>()).Returns(Reader);
            ObjectDb.ExecuteReaderAsync().Returns(Reader);
            ObjectDb.ExecuteReaderAsync(Arg.Any<object>()).Returns(Reader);
        }

        [TearDown]
        public virtual void TearDown()
        {
            ObjectDb?.Dispose();
            Reader?.Dispose();
            DataAccess.ClearReceivedCalls();
            Reader = null;
            CurrencyDataAccess = null;
            ObjectDb = null;
            DataAccess = null;
        }

        protected virtual void SetUpDataAccess()
        {
            DataAccess = Substitute.ForPartsOf<T>();
        }

        protected void MapReaderData(object readerDataMapping)
        {
            DataAccessTestHelper.MapReaderData(Reader, readerDataMapping);
        }
    }
}