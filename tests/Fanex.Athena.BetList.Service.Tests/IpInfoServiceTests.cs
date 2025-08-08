namespace Fanex.Athena.BetList.Service.Tests
{
    using DataAccess.Interfaces;
    using NSubstitute;
    using TestHelper;

    public class IpInfoServiceTests
    {
        private IIPDataAccess _dataAccess;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dataAccess = Substitute.For<IIPDataAccess>();
        }

        [Test]
        public void GetIpLocation_WhenCall_ReturnCorrectValue()
        {
            // Arrange
            var service = new IpInfoService(_dataAccess);
            var expectedLocation = "American";
            var ipAddress = FakeHelper.Create<string>();

            _dataAccess.GetIpLocation(ipAddress).Returns(expectedLocation);

            // Action
            var result = service.GetIpLocation(ipAddress);

            // Assert
            Assert.That(result, Is.EqualTo(expectedLocation));
        }
    }
}
