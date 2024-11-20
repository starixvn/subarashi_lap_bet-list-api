using Microsoft.AspNetCore.Http;
using Fanex.Athena.Common.Site.Helpers;

namespace Fanex.Athena.Common.Site.Tests.Helpers
{
    public class RoutingHelperTests
    {
        [Test]
        public void FixDoubleSlashes_WithEmptyPath_ReturnsEmptyPath()
        {
            // Arrange
            PathString path = new PathString("");

            // Act
            PathString result = path.FixDoubleSlashes();

            // Assert
            Assert.That(result.Value, Is.EqualTo(""));
        }

        [Test]
        public void FixDoubleSlashes_WithSingleSlashPath_ReturnsSamePath()
        {
            // Arrange
            PathString path = new PathString("/");

            // Act
            PathString result = path.FixDoubleSlashes();

            // Assert
            Assert.That(result.Value, Is.EqualTo("/"));
        }

        [Test]
        public void FixDoubleSlashes_WithDoubleSlashPath_ReturnsSingleSlashPath()
        {
            // Arrange
            PathString path = new PathString("//");

            // Act
            PathString result = path.FixDoubleSlashes();

            // Assert
            Assert.That(result.Value, Is.EqualTo("/"));
        }

        [Test]
        public void FixDoubleSlashes_WithNoDoubleSlashPath_ReturnsSamePath()
        {
            // Arrange
            PathString path = new PathString("/test/path");

            // Act
            PathString result = path.FixDoubleSlashes();

            // Assert
            Assert.That(result.Value, Is.EqualTo("/test/path"));
        }
    }
}
