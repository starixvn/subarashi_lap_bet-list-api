using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Result.Core.AllBetCasino;
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
            //
            var controller = CreateController();
            var expectedResult = "abc";
            _resultApi.Render("123", "123", "123", "en").Returns(expectedResult);

            //Action
            var result = controller.Index("123", "123", "123", "en");

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private AllBetResultPopupController CreateController()
            => new(_resultApi);
    }
}