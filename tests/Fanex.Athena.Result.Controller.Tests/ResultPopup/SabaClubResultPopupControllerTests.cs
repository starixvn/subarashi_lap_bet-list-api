namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    using Controller.ResultPopup;
    using Fanex.Result.Core.SabaClub;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;

    public class SabaClubResultPopupControllerTests
    {
        private ISabaClubResult _resultApi;

        [SetUp]
        public void SetUp()
        {
            _resultApi = Substitute.For<ISabaClubResult>();
        }

        [TearDown]
        public void TearDown()
        {
            _resultApi = null;
        }

        [Test]
        public void Index_WhenCall_ReturnCorrectResult()
        {
            //Arrange
            const string expectedResult = "expectedResult";
            var controller = CreateController();
            var model = new SabaClubResultPopupController.SabaClubResultMetaModel
            {
                CustId = "123",
                GameRoundId = "123",
                GameId = "gameId",
                SiteId = "siteId",
                SportType = 1,
                LanguageCode = "en-US"
            };

            _resultApi.GetResult("123", "123", "gameId", "siteId", 1).Returns(expectedResult);

            //Action
            var result = controller.Index(model) as ContentResult;

            //Assert
            Assert.That(result.Content, Is.EqualTo(expectedResult));
        }

        private SabaClubResultPopupController CreateController()
            => new(_resultApi);
    }
}