namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    using Controller.ResultPopup;
    using Fanex.Result.Core.WM;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;

    public class WMResultPopupControllerTests
    {
        private IWMResultBuilder _resultApi;

        [SetUp]
        public void SetUp()
        {
            _resultApi = Substitute.For<IWMResultBuilder>();
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
            var model = new WMResultPopupController.WMResultMetaModel { TransDesc = "123", GameId3rd = "123", BetType=123, LanguageCode = "en-US" };

            _resultApi.Render("123", "123", 123).Returns(expectedResult);

            //Action
            var result = controller.Index(model) as ContentResult;

            //Assert
            Assert.That(result.Content, Is.EqualTo(expectedResult));
        }

        private WMResultPopupController CreateController()
            => new(_resultApi);
    }
}