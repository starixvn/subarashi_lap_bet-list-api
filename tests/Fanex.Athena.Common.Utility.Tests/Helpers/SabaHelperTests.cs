namespace Fanex.Athena.Common.Utility.Tests.Helpers
{
    using Constant;
    using Utility.Helpers;

    public class SabaHelperTests
    {
        [Test]
        public void IsHideSaba_ShouldReturnTrue_WhenSpecialPlatformIsJollysports()
        {
            // Arrange
            int specialPlatform = SpecialPlatform.Jollysports;

            // Act
            bool result = SabaHelper.IsHideSaba(specialPlatform);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsHideSaba_ShouldReturnTrue_WhenSpecialPlatformIsJollysportsCV()
        {
            // Arrange
            int specialPlatform = SpecialPlatform.JollysportsCV;

            // Act
            bool result = SabaHelper.IsHideSaba(specialPlatform);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsHideSaba_ShouldReturnFalse_WhenSpecialPlatformIsNotJollysportsOrJollysportsCV()
        {
            // Arrange
            int specialPlatform = SpecialPlatform.Normal;

            // Act
            bool result = SabaHelper.IsHideSaba(specialPlatform);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveSabaText_ShouldRemoveSabaTextFromInputText()
        {
            // Arrange
            string inputText = "This is a sample text with Saba";

            // Act
            string result = SabaHelper.RemoveSabaText(inputText);

            // Assert
            Assert.That(result, Does.Not.Contain("Saba"));
        }
    }
}
