namespace Fanex.Athena.BetList.DataAccess.Tests
{
    using Model.DTO;
    using NSubstitute;
    using TestHelper;

    [TestFixture]
    public class IPDataAccessTests : BaseDataAccessHelperTest<IPDataAccess>
    {
        private IPDataAccess _ipDataAccess;

        protected override void SetUpDataAccess()
        {
            _ipDataAccess = new IPDataAccess();
            base.SetUpDataAccess();
        }

        [Test]
        public void GetIpLocation_ValidIPAddress_ReturnsCountryShort()
        {
            // Arrange
            string ipAddress = "192.168.0.1";
            const string expectedCountryShort = "US";

            Reader.Read().Returns(true, false);
            Reader["countrySHORT"].Returns(expectedCountryShort);

            // Act
            string result = _ipDataAccess.GetIpLocation(ipAddress);

            // Assert
            ObjectDb.Received(1).SetParameterValue("IP", ipAddress);
            Assert.That(result, Is.EqualTo(expectedCountryShort));
        }

        [Test]
        public void GetIpLocation_InvalidIPAddress_ReturnsEmptyString()
        {
            // Arrange
            string ipAddress = "invalid_ip_address";

            Reader.Read().Returns(false);

            // Act
            string result = _ipDataAccess.GetIpLocation(ipAddress);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetVNIPs_ReturnsTupleOfIPV4RangeEntityAndIPV6RangeEntityLists()
        {
            // Arrange
            var ipVNList = new List<IPV4RangeEntity>
                {
                    new IPV4RangeEntity { IPFrom = 0, IPTo = 100 },
                    new IPV4RangeEntity { IPFrom = 200, IPTo = 300 }
                };

            var ipV6VNList = new List<IPV6RangeEntity>
                {
                    new IPV6RangeEntity { IPFrom = "2001:0db8:85a3:0000:0000:8a2e:0370:7334", IPTo = "2001:0db8:85a3:0000:0000:8a2e:0370:7335" },
                    new IPV6RangeEntity { IPFrom = "2001:0db8:85a3:0000:0000:8a2e:0370:7336", IPTo = "2001:0db8:85a3:0000:0000:8a2e:0370:7337" }
                };
            Reader.Read().Returns(true, true, false, true, true, false);
            Reader.NextResult().Returns(true, false);
            Reader["IPFrom"].Returns(0, 0, 200, 200, "2001:0db8:85a3:0000:0000:8a2e:0370:7334", "2001:0db8:85a3:0000:0000:8a2e:0370:7334", "2001:0db8:85a3:0000:0000:8a2e:0370:7336", "2001:0db8:85a3:0000:0000:8a2e:0370:7336");
            Reader["IPTo"].Returns(100, 100, 300, 300, "2001:0db8:85a3:0000:0000:8a2e:0370:7335", "2001:0db8:85a3:0000:0000:8a2e:0370:7335", "2001:0db8:85a3:0000:0000:8a2e:0370:7337", "2001:0db8:85a3:0000:0000:8a2e:0370:7337");

            // Act
            var result = _ipDataAccess.GetVNIPs();

            // Assert
            Assert.That(result.Item1, Is.EquivalentTo(ipVNList));
            Assert.That(result.Item2, Is.EquivalentTo(ipV6VNList));
        }
    }
}
