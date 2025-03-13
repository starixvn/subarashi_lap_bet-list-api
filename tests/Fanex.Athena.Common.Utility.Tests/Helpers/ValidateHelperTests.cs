

using Fanex.Athena.Common.Utility.Helpers;

namespace Fanex.Athena.Common.Utility.Tests.Helpers
{
    public class ValidateHelperTests
    {
        [Test]
        public void ValidateBaseCurrency_WhenBaseCurrencyIsEmptyAndIsAllowEmptyIsTrue_ReturnsTrue()
        {
            // Arrange
            string baseCurrency = "";
            bool isAllowEmpty = true;

            // Act
            bool result = ValidateHelper.ValidateBaseCurrency(baseCurrency, isAllowEmpty);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateBaseCurrency_WhenBaseCurrencyIsEmptyAndIsAllowEmptyIsFalse_ReturnsFalse()
        {
            // Arrange
            string baseCurrency = "";
            bool isAllowEmpty = false;

            // Act
            bool result = ValidateHelper.ValidateBaseCurrency(baseCurrency, isAllowEmpty);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateBaseCurrency_WhenBaseCurrencyIsNotEmpty_ReturnsTrue()
        {
            // Arrange
            string baseCurrency = "USD";
            bool isAllowEmpty = true;

            // Act
            bool result = ValidateHelper.ValidateBaseCurrency(baseCurrency, isAllowEmpty);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}
