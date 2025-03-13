using Fanex.Athena.Result.Controller.ResultReports;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.KenoLotteryTG;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultReposts
{
    public class ResultKenoReportControllerTests
    {
        private const int VendorProductId = 34343;
        private const string UserId = "987343";
        private const string LanguageCode = "en-US";
        private IActionContextAccessor _actionContextAccessor;

        private IKenoLotteryTGResultFactory _kenoApi;
        private IKenoLotteryTGResult _resultApi;
        private IUrlHelperFactory _urlHelperFactory;

        [SetUp]
        public void SetUp()
        {
            _kenoApi = Substitute.For<IKenoLotteryTGResultFactory>();
            _resultApi = Substitute.For<IKenoLotteryTGResult>();
            _actionContextAccessor = Substitute.For<IActionContextAccessor>();
            _urlHelperFactory = Substitute.For<IUrlHelperFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            _kenoApi = null;
            _resultApi = null;
            _actionContextAccessor = null;
            _urlHelperFactory = null;
        }

        [Test]
        public void GetResultPage_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            const string ExpectedUrl = "http://abc";
            _resultApi.GetResult(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<DateTime>(), Arg.Any<string>())
                .Returns(ExpectedUrl);
            _kenoApi.CreateResultApi(Arg.Any<int>()).Returns(_resultApi);

            var controller = CreateController();

            // Action
            var result = controller.GetResultPage(new Model.MetaModel.ResultKenoMetaModel
            {
                VendorProductId = VendorProductId,
                CustId = UserId,
                LanguageCode = LanguageCode
            }) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(ExpectedUrl));
        }

        private ResultKenoReportController CreateController()
        {
            var httpRequest = Substitute.For<HttpRequest>();
            var context = Substitute.For<HttpContext>();
            context.Request.Returns(httpRequest);
            var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
            _actionContextAccessor.ActionContext.Returns(actionContext);

            return new ResultKenoReportController(_kenoApi, _urlHelperFactory, _actionContextAccessor);
        }
    }
}