namespace Fanex.Athena.Common.Site.Tests.Attributes
{
    using System.Net;
    using Core.Utils;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Services.Interfaces;
    using Site.Attributes;

    public class RequestValidationAttributeTests
    {
        private IContextService _contextService;
        private ICurrencyValidation _currencyValidation;
        private ILoggerFactory _logFactory;
        private ActionExecutingContext _actionExecutingContext;
        private ActionExecutionDelegate _nextDelegate;

        [SetUp]
        public void Setup()
        {
            _contextService = Substitute.For<IContextService>();
            _currencyValidation = Substitute.For<ICurrencyValidation>();
            _logFactory = Substitute.For<ILoggerFactory>();
            _actionExecutingContext = CreateExecutingContext();
            _nextDelegate = Substitute.For<ActionExecutionDelegate>();
        }

        [TearDown]
        public void TearDown()
        {
            _logFactory.Dispose();
            _contextService = null;
            _currencyValidation = null;
            _logFactory = null;
            _actionExecutingContext = null;
            _nextDelegate = null;
        }

        [Test]
        public async Task OnActionExecutionAsync_InvalidParams_ReturnsBadRequest()
        {
            // Arrange
            var attribute = new DataValidationImplAttribute(_contextService, _currencyValidation, _logFactory, false, false);
            var model = Substitute.For<IValidationMetaModel>();
            model.IsValid.Returns(false);
            _actionExecutingContext.ActionArguments.Add("model", model);

            // Act
            await attribute.OnActionExecutionAsync(_actionExecutingContext, CreateDelegate());

            // Assert
            Assert.That(_actionExecutingContext.Result, Is.InstanceOf<JsonResult>());
            var jsonResult = _actionExecutingContext.Result as JsonResult;
            Assert.That(jsonResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task OnActionExecutionAsync_ValidParams_CallsNextDelegate()
        {
            // Arrange
            var attribute = new DataValidationImplAttribute(_contextService, _currencyValidation, _logFactory, false, false);
            var model = Substitute.For<IValidationMetaModel>();
            model.IsValid.Returns(true);
            _actionExecutingContext.ActionArguments.Add("model", model);

            // Act
            await attribute.OnActionExecutionAsync(_actionExecutingContext, _nextDelegate);

            // Assert
            await _nextDelegate.Received(1).Invoke();
        }

        private ActionExecutionDelegate CreateDelegate() => () => Task.FromResult(new ActionExecutedContext(
           new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
           Array.Empty<IFilterMetadata>(),
           new object())
            );

        private ActionExecutingContext CreateExecutingContext() => new ActionExecutingContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            Array.Empty<IFilterMetadata>(),
            new Dictionary<string, object>(),
            Substitute.For<Controller>()
            );
    }
}
