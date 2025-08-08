namespace Fanex.Athena.Common.DataAccess.UnitTests
{
    using System.Data;
    using Data;
    using Interfaces;
    using NSubstitute;
    using TestHelper;

    public class AuthorizationDataAccessTests
    {
        private IAuthorizationDataAccess _authorizationDataAccess;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        private IObjectDb _objectDb;
        private IDataReader _dataReader;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        [SetUp]
        public void SetUp()
        {
            _authorizationDataAccess = new AuthorizationDataAccess();
            _objectDb = Substitute.For<IObjectDb>();
            _dataReader = Substitute.For<IDataReader>();
            DBTestHelper.SetUpDBObjects(_objectDb);
        }

        [TearDown]
        public void TearDown()
        {
            _authorizationDataAccess = null;
            _objectDb = null;
            _dataReader = null;
        }

        [Test]
        public void IsCustomerBelonging_ShouldReturnCurrencyIdAndCustId()
        {
            // Arrange
            int custId = 1;
            long transId = 12345;
            _objectDb.ExecuteReader().Returns(_dataReader);
            _dataReader.Read().Returns(true, false);
            _dataReader["CurrencyId"].Returns(1);
            _dataReader["CustId"].Returns(2);

            // Act
            var result = _authorizationDataAccess.IsCustomerBelonging(custId, transId);

            // Assert
            Assert.That(result.CurrencyId, Is.EqualTo(1));
            Assert.That(result.CustId, Is.EqualTo(2));
        }

        [Test]
        public void IsCustomerBelonging_ShouldReturnCurrencyIdAndCustId_WhenDataReaderReadReturnsFalse()
        {
            // Arrange
            int custId = 1;
            long transId = 12345;
            _objectDb.ExecuteReader().Returns(_dataReader);
            _dataReader.Read().Returns(false);
            _dataReader["CurrencyId"].Returns(1);
            _dataReader["CustId"].Returns(2);

            // Act
            var result = _authorizationDataAccess.IsCustomerBelonging(custId, transId);

            // Assert
            Assert.That(result.CurrencyId, Is.EqualTo(0));
            Assert.That(result.CustId, Is.EqualTo(0));
        }
    }
}
