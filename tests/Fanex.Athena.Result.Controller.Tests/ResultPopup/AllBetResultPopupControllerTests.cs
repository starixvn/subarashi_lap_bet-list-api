using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Result.Core.AllBetCasino;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    public class AllBetResultPopupControllerTests
    {
        private IAllbetResultBuilder _resultApi;

        [SetUp]
        public void SetUp()
        {
            _resultApi = Substitute.For<IAllbetResultBuilder>();
        }

        [TearDown]
        public void TearDown()
        {
            _resultApi = null;
        }

        [Test]
        public void Index_WhenCall_ReturnCorrectResult()
        {
            //Assert
            var controller = CreateController();
            var expectedResult = "abc";
            var model = new AllBetResultPopupController.AllBetResultMetaModel
            {
                TransDesc = "123",
                BetCheck = "123",
                BetType = "123",
                LanguageCode = "en"
            };
            _resultApi.Render("123", "123", "123", "en").Returns(expectedResult);

            //Action
            var result = controller.Index(model) as ContentResult;

            //Assert
            Assert.That(result.Content, Is.EqualTo(expectedResult));
        }

        private AllBetResultPopupController CreateController()
            => new(_resultApi);
    }
}