

//using Fanex.Athena.Common.Utility.Services;
//using Fanex.Athena.Common.Utility.Services.Interfaces;
//using Microsoft.AspNetCore.Mvc.Razor;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using NSubstitute;

//namespace Fanex.Athena.Common.Utility.Tests.Services
//{
//    public class TemplateBuilderTests
//    {
//        private ITemplateBuilder _templateBuilder;
//        private IRazorViewEngine _viewEngine;
//        private IServiceProvider _serviceProvider;
//        private ITempDataProvider _tempDataProvider;

//        [SetUp]
//        public void Setup()
//        {
//            _viewEngine = Substitute.For<IRazorViewEngine>();
//            _serviceProvider = Substitute.For<IServiceProvider>();
//            _tempDataProvider = Substitute.For<ITempDataProvider>();

//            _templateBuilder = new TemplateBuilder(_viewEngine, _serviceProvider, _tempDataProvider);
//        }

//        [Test]
//        public async Task BuildContent_ShouldReturnCorrectContent()
//        {
//            // Arrange
//            string path = "path/to/view";
//            string expectedContent = "Expected content";
//            var model = new TestModel();

//            var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext();
//            var view = Substitute.For<IView>();
//            var output = new StringWriter();

//            _viewEngine.GetView(null, path, true).ReturnsForAnyArgs(ViewEngineResult.Found("view", view));
//            view.RenderAsync(Arg.Any<ViewContext>()).ReturnsForAnyArgs(Task.CompletedTask);
//            output.ToString().Returns(expectedContent);

//            // Act
//            var result = await _templateBuilder.BuildContent(path, model);

//            // Assert
//            Assert.That(result, Is.EqualTo(expectedContent));
//        }

//        private class TestModel
//        {
//            // Define properties for test model
//        }
//    }
//}
