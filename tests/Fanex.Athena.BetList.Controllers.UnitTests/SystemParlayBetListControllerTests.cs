using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.BetList.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    public class SystemParlayBetListControllerTests : BaseControllerHelperTests
    {
        [Test]
        public async Task Detail_WhenCall_CallCorrectMethod()
        {
            var ticketHelper = Substitute.For<ITicketHelper>();
            var systemParlayDetailService = Substitute.For<ISystemParlayDetailService>();
            var webHostEnv = Substitute.For<IWebHostEnvironment>();
            // Arrange
            var controller = new SystemParlayBetListController(ticketHelper, systemParlayDetailService, webHostEnv);
            const string rootPath = "RootPath";
            const string ExpectedResultContent = "System parlay detail result content";
            var expectedViewPath = Path.Combine(rootPath, "Views/SystemParlayDetail/Index.cshtml");
            webHostEnv.ContentRootPath.Returns(rootPath);

            var metaModel = FakeHelper.Create<SystemParlayDetailMetaModel>();

            systemParlayDetailService.BuildHtmlContent(metaModel, ticketHelper, expectedViewPath).Returns(ExpectedResultContent);

            // Action
            var result = await controller.Detail(metaModel);

            // Assert
            Assert.That(result, Is.EqualTo(ExpectedResultContent));
        }
    }
}
