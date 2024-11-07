using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Data;
using Fanext.Athena.TestHelper;
using NSubstitute;
using System.Data;

namespace Fanex.Athena.Common.DataAccess.UnitTests
{
    public class BalanceUpDataAccessTests
    {
        private BalanceUpDataAccess _balanceUpDataAccess;
        private IMemoryCacheService _cacheServiceMock;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        private IDataReader _dataReaderMock;
        private IObjectDb _objectDbMock;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        [SetUp]
        public void SetUp()
        {
            _cacheServiceMock = Substitute.For<IMemoryCacheService>();
            _balanceUpDataAccess = new BalanceUpDataAccess(_cacheServiceMock);
            _dataReaderMock = Substitute.For<IDataReader>();
            _objectDbMock = Substitute.For<IObjectDb>();
            TestHelper.SetUpDBObjects(_objectDbMock);
        }

        [TearDown]
        public void TearDown()
        {
            _balanceUpDataAccess = null;
            _cacheServiceMock = null;
            _dataReaderMock = null;
            _objectDbMock = null;
        }

        [Test]
        public void GetBalanceUp_WithCachedBalanceUp_ReturnsCachedBalanceUp()
        {
            // Arrange
            var cachedBalanceUp = new BalanceUp();
            _cacheServiceMock.Get<BalanceUp>(Arg.Any<string>()).Returns(cachedBalanceUp);

            // Act
            var result = _balanceUpDataAccess.GetBalanceUp();

            // Assert
            Assert.That(result, Is.EqualTo(cachedBalanceUp));
        }

        [Test]
        public void GetBalanceUp_WithoutCachedBalanceUp_ReturnsBalanceUpFromDbAndCachesIt()
        {
            // Arrange
            _cacheServiceMock.Get<BalanceUp>(Arg.Any<string>()).Returns((BalanceUp)null);
            _objectDbMock.ExecuteReader().Returns(_dataReaderMock);
            _dataReaderMock.Read().Returns(false);

            // Act
            var result = _balanceUpDataAccess.GetBalanceUp();

            // Assert
            _cacheServiceMock.Received(1).Set(Arg.Any<string>(), result, Arg.Any<int>());
        }

        [Test]
        public void GetBalanceUp_WithoutCachedBalanceUp_ReturnsBalanceUpFromDbAndCachesIt_WithDataReaderReadTrue()
        {
            // Arrange
            var balanceDate = DateTime.Today;
            _cacheServiceMock.Get<BalanceUp>(Arg.Any<string>()).Returns((BalanceUp)null);
            _objectDbMock.ExecuteReader().Returns(_dataReaderMock);
            _dataReaderMock.Read().Returns(true);
            _dataReaderMock["balanceupday"].Returns(balanceDate);
            _dataReaderMock["movebetsday"].Returns(balanceDate);

            // Act
            var result = _balanceUpDataAccess.GetBalanceUp();

            // Assert
            _cacheServiceMock.Received(1).Set(Arg.Any<string>(), result, Arg.Any<int>());
            Assert.That(result.BalanceUpDay, Is.EqualTo(balanceDate));
            Assert.That(result.MoveBetsDay, Is.EqualTo(balanceDate));
        }


    }
}
