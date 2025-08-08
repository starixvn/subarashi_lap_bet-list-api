namespace Fanexe.Athena.Result.Service.Tests
{
    using Fanex.Athena.Result.Model.Const;
    using Fanex.Athena.Result.Service;
    using Fanex.Result.Core.KenoLotteryTG;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using NSubstitute;

    public class KenoLotteryTGResultApiTests
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

        [TestCase(ResultVendorProductTypes.Keno)]
        [TestCase(ResultVendorProductTypes.Lottery)]
        [TestCase(ResultVendorProductTypes.Table)]
        [TestCase(ResultVendorProductTypes.VirtualGames)]
        public void CreateResultApi_WhenCall_ReturnCorrectResult(int productType)
        {
            // Arrange
            var service = CreateService();
            var expectedResult = new KenoLotteryTGConfigs();
            _configuration.GetSection(string.Empty).ReturnsForAnyArgs(_configurationSection);
            _configurationSection.Value.Returns(JsonConvert.SerializeObject(expectedResult));

            // Action
            var result = service.CreateResultApi(productType);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        private KenoLotteryTGResultApi CreateService()
        {
            return new KenoLotteryTGResultApi(_configuration);
        }
    }
}