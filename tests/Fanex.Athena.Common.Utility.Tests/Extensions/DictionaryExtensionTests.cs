
namespace Fanex.Athena.Common.Utility.Tests.Extensions
{
    public class DictionaryExtensionTests
    {
        [Test]
        public void GetValueOrDefault_KeyExists_ReturnsValue()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
                    {
                        { "key1", 1 },
                        { "key2", 2 },
                        { "key3", 3 }
                    };

            // Act
            var result = dictionary.GetValueOrDefault("key2", 0);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GetValueOrDefault_KeyDoesNotExist_ReturnsDefaultValue()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
                    {
                        { "key1", 1 },
                        { "key2", 2 },
                        { "key3", 3 }
                    };

            // Act
            var result = dictionary.GetValueOrDefault("key4", 0);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }
    }
}
