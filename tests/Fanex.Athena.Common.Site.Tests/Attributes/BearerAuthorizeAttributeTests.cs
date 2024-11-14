

using Fanex.Athena.Common.Site.Attributes;

namespace Fanex.Athena.Common.Site.Tests.Attributes
{
    public class BearerAuthorizeAttributeTests
    {
        [Test]
        public void BearerAuthorizeAttribute_RolesSet_RolesJoined()
        {
            // Arrange
            var roles = new[] { "Admin", "User" };

            // Act
            var attribute = new BearerAuthorizeAttribute(roles);

            // Assert
            Assert.That(attribute.Roles, Is.EqualTo("Admin,User"));
        }

        [Test]
        public void BearerAuthorizeAttribute_NoRolesSet_RolesNull()
        {
            // Arrange
            var roles = new string[0];

            // Act
            var attribute = new BearerAuthorizeAttribute(roles);

            // Assert
            Assert.That(attribute.Roles, Is.Null);
        }

        [Test]
        public void BearerAuthorizeAttribute_AuthenticationSchemesSet_Bearer()
        {
            // Arrange
            var roles = new[] { "Admin", "User" };

            // Act
            var attribute = new BearerAuthorizeAttribute(roles);

            // Assert
            Assert.That(attribute.AuthenticationSchemes, Is.EqualTo("Bearer"));
        }
    }
}
