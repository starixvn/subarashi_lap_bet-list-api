using Fanex.Athena.Common.Utility.Helpers;

namespace Fanex.Athena.Common.Utility.Tests.Helpers
{
    public class NumberHelperTest
    {
        [TestCase(0, "0.00")]
        [TestCase(2.5, "2.50")]
        [TestCase(2.53, "2.53")]
        [TestCase(2.534, "2.53")]
        [TestCase(2.535, "2.54")]
        [TestCase(2.5361, "2.54")]
        [TestCase(2, "2.00")]
        [TestCase(-1, "-1.00")]
        [TestCase(-2.5, "-2.50")]
        public void DecFormat_OneArgumentWithDecimalType_ReturnFormattedNumberWithTwoDigits(object value, string expected)
        {
            // Arrange
            var decimalNumber = Convert.ToDecimal(value);

            // Action
            var result = NumberHelper.DecFormat(decimalNumber);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0, "0.000")]
        [TestCase(2.5, "2.500")]
        [TestCase(2.53, "2.530")]
        [TestCase(2.534, "2.534")]
        [TestCase(2.5354, "2.535")]
        [TestCase(2.5366, "2.537")]
        [TestCase(2, "2.000")]
        [TestCase(-1, "-1.000")]
        [TestCase(-2.5, "-2.500")]
        public void DecFormat_TwoArgumentsWithDecimalType_ReturnFormattedNumberWithThreeDigits(object value, string expected)
        {
            // Arrange
            var decimalNumber = Convert.ToDecimal(value);
            byte digit = 3;

            // Action
            var result = NumberHelper.DecFormat(decimalNumber, digit);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0, 0)]
        [TestCase(1, 100)]
        [TestCase(0.5, 50)]
        [TestCase(-0.25, -25)]
        public void ToPercentage_ReturnPercentageValue(decimal value, decimal expected)
        {
            // Action
            var result = NumberHelper.ToPercentage(value);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0, true)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        public void IsEvenNumber_DecimalValue_ReturnsCorrectResult(decimal value, bool expected)
        {
            // Action
            var result = NumberHelper.IsEvenNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0, true)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        public void IsEvenNumber_IntValue_ReturnsCorrectResult(int value, bool expected)
        {
            // Action
            var result = NumberHelper.IsEvenNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void ToInt_ReturnsCorrectIntValue(bool value, int expected)
        {
            // Action
            var result = NumberHelper.ToInt(value);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
