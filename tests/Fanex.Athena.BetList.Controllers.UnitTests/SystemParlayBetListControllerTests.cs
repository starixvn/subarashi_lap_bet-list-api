namespace Fanex.Athena.BetList.Controllers.UnitTests
{
    using Fanex.BetList.Core.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using NSubstitute;
    using NUnit.Framework;
    using Service.Interfaces;
    using TestHelper;

    public class SystemParlayBetListControllerTests : BaseControllerHelperTests
    {
        [Test]
        public async Task Detail_WhenCall_CallCorrectMethod()
        {
            var ticketHelper = Substitute.For<ITicketHelper>();
            var systemParlayDetailService = Substitute.For<ISystemParlayDetailService>();
            // Arrange
            var controller = new SystemParlayBetListController(ticketHelper, systemParlayDetailService);
            const string ExpectedResultContent = "System parlay detail result content";
            var expectedViewPath = Path.Combine("/Views/SystemParlayDetail/Index.cshtml");

            var metaModel = FakeHelper.Create<SystemParlayDetailMetaModel>();

            systemParlayDetailService.BuildHtmlContent(metaModel, ticketHelper, expectedViewPath).Returns(ExpectedResultContent);

            // Action
            var result = await controller.Detail(metaModel) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(ExpectedResultContent));
        }
    }
}
