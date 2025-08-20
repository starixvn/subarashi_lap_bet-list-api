namespace Fanex.Athena.Common.Utility.Tests.Helpers
{
    using Utility.Helpers;

    public class SqlHelperTests
    {
        [Test]
        public void ToSqlTextSearch_WhenTextIsNull_ReturnsNull()
        {
            // Arrange
            string text = null;

            // Act
            string result = SqlHelper.ToSqlTextSearch(text);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ToSqlTextSearch_WhenTextIsEmpty_ReturnsEmpty()
        {
            // Arrange
            string text = "";

            // Act
            string result = SqlHelper.ToSqlTextSearch(text);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ToSqlTextSearch_WhenTextHasUnderscore_ReplacesUnderscoreWithSquareBracketUnderscore()
        {
            // Arrange
            string text = "abc_def";

            // Act
            string result = SqlHelper.ToSqlTextSearch(text);

            // Assert
            Assert.That(result, Is.EqualTo("abc[_]def"));
        }

        [Test]
        public void ToSqlTextSearch_WhenTextHasNoUnderscore_ReturnsSameText()
        {
            // Arrange
            string text = "abcdef";

            // Act
            string result = SqlHelper.ToSqlTextSearch(text);

            // Assert
            Assert.That(result, Is.EqualTo(text));
        }
    }
}
