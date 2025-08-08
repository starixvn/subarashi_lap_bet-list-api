namespace Fanex.Athena.Common.Site.Tests.Attributes
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Services.Interfaces;
    using Site.Attributes;

    public class ApiBetlistAuthorizeAtrributeTests
    {
        private ILicenseeSettingService _licenseeSettingServiceMock;
        private IContextService _contextServiceMock;
        private ApiBetlistAuthorizeAttribute _attribute;
        private AuthorizationFilterContext _filterContext;

        [SetUp]
        public void Setup()
        {
            _licenseeSettingServiceMock = Substitute.For<ILicenseeSettingService>();
            _contextServiceMock = Substitute.For<IContextService>();
            _attribute = new ApiBetlistAuthorizeAttribute();
            _filterContext = CreateFilterContext();
        }

        [TearDown]
        public void TearDown()
        {
            _licenseeSettingServiceMock = null;
            _contextServiceMock = null;
            _attribute = null;
            _filterContext = null;
        }

        [Test]
        public void OnAuthorization_WhenAuthorizedOnBetList_ReturnsNoResult()
        {
            // Arrange
            _licenseeSettingServiceMock.IsAuthorizedOnBetList(Arg.Any<string>()).Returns(true);
            _contextServiceMock.SiteName.Returns("LicenseeName");
            _filterContext.HttpContext.RequestServices = CreateServiceProvider();

            // Act
            _attribute.OnAuthorization(_filterContext);

            // Assert
            Assert.That(_filterContext.Result, Is.Null);
        }

        [Test]
        public void OnAuthorization_WhenUnauthorizedOnBetList_ReturnsUnauthorizedResult()
        {
            // Arrange
            _licenseeSettingServiceMock.IsAuthorizedOnBetList(Arg.Any<string>()).Returns(false);
            _contextServiceMock.SiteName.Returns("LicenseeName");
            _filterContext.HttpContext.RequestServices = CreateServiceProvider();

            // Act
            _attribute.OnAuthorization(_filterContext);

            // Assert
            Assert.That(_filterContext.Result, Is.Not.Null);
            Assert.That(_filterContext.Result, Is.InstanceOf<JsonResult>());
            var jsonResult = (JsonResult)_filterContext.Result;
            Assert.That(jsonResult.StatusCode, Is.EqualTo(401));
        }

        private IServiceProvider CreateServiceProvider()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(ILicenseeSettingService)).Returns(_licenseeSettingServiceMock);
            serviceProvider.GetService(typeof(IContextService)).Returns(_contextServiceMock);
            return serviceProvider;
        }

        private AuthorizationFilterContext CreateFilterContext() => new AuthorizationFilterContext(
                       new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                                  Array.Empty<IFilterMetadata>());
    }
}
