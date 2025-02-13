using Fanex.Athena.Common.Services;
using Fanex.Athena.Common.Services.Interfaces;
using NSubstitute;

namespace Fanex.Athena.Common.Service.UnitTests
{
    public class ShardInfoAccessorTests
    {
        private ISiteDBAccessor _contextServiceMock;
        private ShardInfoAccessor _shardInfoAccessor;

        [SetUp]
        public void Setup()
        {
            _contextServiceMock = Substitute.For<ISiteDBAccessor>();
            _shardInfoAccessor = new ShardInfoAccessor(_contextServiceMock);
        }

        [TearDown]
        public void TearDown()
        {
            _contextServiceMock = null;
            _shardInfoAccessor = null;
        }

        [Test]
        public void TryUseShardDB_WhenKeyIsShardSPAndUseShardDBIsTrue_ShouldReturnTrueAndSetDbServer()
        {
            // Arrange
            string key = "shard_1";
            string expectedDbServer = "dbServer1";
            _contextServiceMock.SiteDBServer.Returns(expectedDbServer);

            // Act
            bool result = _shardInfoAccessor.TryUseShardDB(key, out string dbServer);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dbServer, Is.EqualTo(expectedDbServer));
        }

        [Test]
        public void TryUseShardDB_WhenKeyIsNotShardSP_ShouldReturnFalseAndSetDbServerNull()
        {
            // Arrange
            string key = "other_key";

            // Act
            bool result = _shardInfoAccessor.TryUseShardDB(key, out string dbServer);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(dbServer, Is.Null);
        }

        [Test]
        public void TryUseShardDB_Should_Return_False_And_Set_DbServer_Null_When_UseShardDB_Is_False()
        {
            // Arrange
            string key = "shard_1";
            _contextServiceMock.ForceIgnoreSharding.Returns(true);

            // Act
            bool result = _shardInfoAccessor.TryUseShardDB(key, out string dbServer);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(dbServer, Is.Null);
        }

        [Test]
        public void UseShardDB_Should_Return_True_When_ForceIgnoreSharding_Is_False_And_SiteDBServer_Is_Not_Empty()
        {
            // Arrange
            _contextServiceMock.ForceIgnoreSharding.Returns(false);
            _contextServiceMock.SiteDBServer.Returns("dbServer1");

            // Act
            bool result = _shardInfoAccessor.UseShardDB;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void UseShardDB_Should_Return_False_When_ForceIgnoreSharding_Is_True()
        {
            // Arrange
            _contextServiceMock.ForceIgnoreSharding.Returns(true);

            // Act
            bool result = _shardInfoAccessor.UseShardDB;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UseShardDB_Should_Return_False_When_SiteDBServer_Is_Empty()
        {
            // Arrange
            _contextServiceMock.ForceIgnoreSharding.Returns(false);
            _contextServiceMock.SiteDBServer.Returns("");

            // Act
            bool result = _shardInfoAccessor.UseShardDB;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetShardSPKey_WhenKeyStartsWithPrefixSiteDB_ShouldReturnKeyWithSiteDBPrefix()
        {
            // Arrange
            string key = "sitedb_key";
            string expectedShardSPKey = "SiteDB_key";

            // Act
            string result = _shardInfoAccessor.GetShardSPKey(key);

            // Assert
            Assert.That(result.ToUpper(), Is.EqualTo(expectedShardSPKey.ToUpper()));
        }

        [Test]
        public void GetShardSPKey_Should_Return_Key_With_SiteDB_Prefix_When_Key_Starts_With_PrefixSharding()
        {
            // Arrange
            string key = "shard_key";
            string expectedShardSPKey = "SiteDB_key";

            // Act
            string result = _shardInfoAccessor.GetShardSPKey(key);

            // Assert
            Assert.That(result, Is.EqualTo(expectedShardSPKey));
        }

        [Test]
        public void GetShardSPKey_Should_Return_Key_Without_Prefix_When_Key_Does_Not_Start_With_PrefixSiteDB_Or_PrefixSharding()
        {
            // Arrange
            string key = "shard_other_key";

            // Act
            string result = _shardInfoAccessor.GetShardSPKey(key);

            // Assert
            Assert.That(result.ToUpper(), Is.EqualTo($"SiteDB_other_key".ToUpper()));
        }
    }
}
