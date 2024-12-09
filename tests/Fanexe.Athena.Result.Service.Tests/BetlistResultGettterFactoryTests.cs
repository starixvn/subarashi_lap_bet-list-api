using Fanex.Athena.Result.Service;
using Fanex.Result.Core.ThirdParty;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;

namespace Fanexe.Athena.Result.Service.Tests
{
    public class BetlistResultGettterFactoryTests
    {
        private IConfiguration _configuration;
        private IConfigurationSection _configurationSection;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfiguration>();
            _configurationSection = Substitute.For<IConfigurationSection>();
        }

        [TearDown]
        public void TearDown()
        {
            _configuration = null;
            _configurationSection = null;
        }

        [Test]
        public void CreateThirdPartyResultGetter_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            var service = CreateService();
            var expectedResult = new ThirdPartyConfigs();
            _configuration.GetSection(string.Empty).ReturnsForAnyArgs(_configurationSection);
            _configurationSection.Value.Returns(JsonConvert.SerializeObject(expectedResult));

            // Action
            var result = service.CreateThirdPartyResultGetter(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        private BetlistResultGettterFactory CreateService() => new(_configuration);
    }
}