namespace Fanex.Athena.BetList.Service.Tests
{
    using DataAccess.Interfaces;
    using Interfaces;
    using Model.DTO;
    using NSubstitute;
    using TestHelper;

    public class IPServiceTests
    {
        private IIPDataAccess _ipDataAccess;
        private IIPService _ipService;

        [SetUp]
        public void Setup()
        {
            _ipDataAccess = Substitute.For<IIPDataAccess>();
            _ipService = new IPService(_ipDataAccess);
        }

        [TearDown]
        public void TearDown()
        {
            _ipDataAccess = null;
        }

        [Test]
        public void IsVNIP_WhenCall_IPNullOrEmpty_ReturnFalse()
        {
            // Arrange
            var ip = string.Empty;

            // Action
            var result = _ipService.IsVNIP(ip);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsVNIP_WhenCall_ListIPVNIsEmpty_ReturnFalse()
        {
            // Arrange
            var ip = "1.1.1.1";
            var expectedResult = Tuple.Create<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>>(new List<IPV4RangeEntity>(), new List<IPV6RangeEntity>());
            _ipDataAccess.GetVNIPs().Returns(expectedResult);

            // Action
            var result = _ipService.IsVNIP(ip);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsVNIP_WhenCall_IpIsNotInRangeIP_ReturnFalse()
        {
            // Arrange
            var ip = "1.1.1.1";
            var expectedResult = Tuple.Create<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>>(new List<IPV4RangeEntity> { new IPV4RangeEntity { IPFrom = 1234, IPTo = 5678 } },
                new List<IPV6RangeEntity> { new IPV6RangeEntity { IPFrom = "aaaa", IPTo = "ffff" } });
            _ipDataAccess.GetVNIPs().Returns(expectedResult);

            // Action
            var result = _ipService.IsVNIP(ip);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsVNIP_WhenCall_IsIPVN_ReturnTrue()
        {
            // Arrange
            var ip = "1.1.1.1";
            var expectedResult = Tuple.Create<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>>(new List<IPV4RangeEntity> { new IPV4RangeEntity { IPFrom = 1234, IPTo = 16843009 } }, new List<IPV6RangeEntity>());
            _ipDataAccess.GetVNIPs().Returns(expectedResult);

            // Action
            var result = _ipService.IsVNIP(ip);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsAlwaysIpvn_WhenCall_ReturnTrue()
        {
            // Arrange
            var ip = FakeHelper.Create<string>();

            // Action
            var result = _ipService.IsAlwaysIpvn(ip);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidIpV4_WhenCallWithEmptyString_ReturnFalse()
        {
            //Arrange
            var ip = string.Empty;

            //Action
            var result = IPService.IsValidIpV4(ip);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsVNIP_WhenCallWithIpV6AndNotExistIPV6VNRange_ReturnFalse()
        {
            //Arrange
            var ip = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            var expectedResult = Tuple.Create<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>>(new List<IPV4RangeEntity>(), null);
            _ipDataAccess.GetVNIPs().Returns(expectedResult);

            //Action
            var result = _ipService.IsVNIP(ip);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsVNIP_WhenCallWithIpV6AndExistIpInVnRange_ReturnTrue()
        {
            //Arrange
            var ip = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            var expectedResult = Tuple.Create<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>>
                (null, new List<IPV6RangeEntity> { new() { IPFrom = "20010db885a3000000008a2e03707334", IPTo = "20010db885a3000000008a2e03707336" } });
            _ipDataAccess.GetVNIPs().Returns(expectedResult);

            //Action
            var result = _ipService.IsVNIP(ip);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsVNIP_WhenCallWithIpV4AndNotExistIPV4VNRange_ReturnFalse()
        {
            //Arrange
            var ip = "127.0.0.1";
            var expectedResult = Tuple.Create<IList<IPV4RangeEntity>, IList<IPV6RangeEntity>>(null, new List<IPV6RangeEntity>());
            _ipDataAccess.GetVNIPs().Returns(expectedResult);

            //Action
            var result = _ipService.IsVNIP(ip);

            //Assert
            Assert.That(result, Is.False);

        }
    }
}
