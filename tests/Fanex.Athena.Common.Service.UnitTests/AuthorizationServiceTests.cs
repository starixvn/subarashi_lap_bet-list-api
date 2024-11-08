using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services;
using NSubstitute;

namespace Fanex.Athena.Common.Service.UnitTests
{
    [TestFixture]
    internal class AuthorizationServiceTests
    {
        private IAuthorizationDataAccess _authorizationDataAccessMock;
        private AuthorizationService _authorizationService;

        [SetUp]
        public void SetUp()
        {
            _authorizationDataAccessMock = Substitute.For<IAuthorizationDataAccess>();
            _authorizationService = new AuthorizationService(_authorizationDataAccessMock);
        }

        [Test]
        public void HasPermissionOnData_ShouldReturnTrue_WhenCustIdAndTransIdAreZero()
        {
            // Arrange
            var currencyAgents = new List<KeyValuePair<int, int[]>>();
            int custId = 0;
            long transId = 0;

            // Act
            var result = _authorizationService.HasPermissionOnData(currencyAgents, custId, transId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasPermissionOnData_ShouldCallIsCustomerBelonging_WhenCustIdAndTransIdAreNotZero()
        {
            // Arrange
            var currencyAgents = new List<KeyValuePair<int, int[]>>();
            int custId = 1;
            long transId = 2;

            // Act
            _authorizationService.HasPermissionOnData(currencyAgents, custId, transId);

            // Assert
            _authorizationDataAccessMock.Received(1).IsCustomerBelonging(custId, transId);
        }

        [Test]
        public void HasPermissionOnData_ShouldReturnTrue_WhenCustomerBelongingIsFoundInCurrencyAgents()
        {
            // Arrange
            var currencyAgents = new List<KeyValuePair<int, int[]>>();
            int custId = 1;
            long transId = 2;
            var customerBelonging = new KeyValuePair<int, int[]>(3, new int[] { 4, 5 });
            currencyAgents.Add(customerBelonging);
            _authorizationDataAccessMock.IsCustomerBelonging(custId, transId).Returns((3, 4));

            // Act
            var result = _authorizationService.HasPermissionOnData(currencyAgents, custId, transId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HasPermissionOnData_ShouldReturnFalse_WhenCustomerBelongingIsNotFoundInCurrencyAgents()
        {
            // Arrange
            var currencyAgents = new List<KeyValuePair<int, int[]>>();
            int custId = 1;
            long transId = 2;
            var customerBelonging = new KeyValuePair<int, int[]>(3, new int[] { 4, 5 });
            currencyAgents.Add(customerBelonging);
            _authorizationDataAccessMock.IsCustomerBelonging(custId, transId).Returns((7, 8));

            // Act
            var result = _authorizationService.HasPermissionOnData(currencyAgents, custId, transId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
