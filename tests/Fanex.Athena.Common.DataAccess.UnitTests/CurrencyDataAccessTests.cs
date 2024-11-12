using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Data;
using NSubstitute;
using System.Data;


namespace Fanex.Athena.Common.DataAccess.UnitTests
{
    internal class CurrencyDataAccessTests
    {
        private ICurrencyDataAccess _currencyDataAccess;
        private IMemoryCacheService _memoryCacheService;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        private IDataReader _dataReaderMock;
        private IObjectDb _objectDbMock;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        [SetUp]
        public void SetUp()
        {
            _memoryCacheService = Substitute.For<IMemoryCacheService>();
            _currencyDataAccess = new CurrencyDataAccess(_memoryCacheService);
            _dataReaderMock = Substitute.For<IDataReader>();
            _objectDbMock = Substitute.For<IObjectDb>();
            DBTestHelper.SetUpDBObjects(_objectDbMock);
            _objectDbMock.ExecuteReaderAsync().Returns(_dataReaderMock);
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCacheService = null;
            _currencyDataAccess = null;
            _dataReaderMock = null;
            _objectDbMock = null;
        }

        [Test]
        public async Task GetCurrencyListAsync_WithCachedData_ReturnsCachedData()
        {
            // Arrange
            var cachedCurrencyList = new Dictionary<int, string>
                    {
                        { 1, "USD" },
                        { 2, "EUR" },
                        { 3, "GBP" }
                    };
            _memoryCacheService.Get<IDictionary<int, string>>(Arg.Any<string>()).Returns(cachedCurrencyList);

            // Act
            var result = await _currencyDataAccess.GetCurrencyListAsync();

            // Assert
            Assert.That(result, Is.EqualTo(cachedCurrencyList));
        }

        [Test]
        public async Task GetCurrencyListAsync_WithNoCachedAndEmptyData_NoCacheEmptyData()
        {
            // Arrange
            _memoryCacheService.Get<IDictionary<int, string>>(Arg.Any<string>()).Returns((IDictionary<int, string>)null);
            _dataReaderMock.Read().Returns(false);

            // Act
            var result = await _currencyDataAccess.GetCurrencyListAsync();

            // Assert
            _memoryCacheService.DidNotReceive().Set(Arg.Any<string>(), result, Arg.Any<int>());
        }

        [Test]
        public async Task GetCurrencyListAsync_WhenNoCachedAndNotEmptyCurrencies_ReturnAndCachedData()
        {
            // Arrange
            var currencyList = new Dictionary<int, string>
            {
                { 1, "USD" },
            };
            _memoryCacheService.Get<IDictionary<int, string>>(Arg.Any<string>()).Returns((IDictionary<int, string>)null);
            _dataReaderMock.Read().Returns(true, false);
            _dataReaderMock["Id"].Returns(currencyList.Keys.First());
            _dataReaderMock["Name"].Returns(currencyList.Values.First());
            _objectDbMock.ExecuteReaderAsync().Returns(_dataReaderMock);

            // Act
            var result = await _currencyDataAccess.GetCurrencyListAsync();

            // Assert
            Assert.That(result.Keys.First(), Is.EqualTo(currencyList.Keys.First()));
            Assert.That(result.Values.First(), Is.EqualTo(currencyList.Values.First()));
            _memoryCacheService.Received().Set(Arg.Any<string>(), result, Arg.Any<int>());
        }
    }
}
