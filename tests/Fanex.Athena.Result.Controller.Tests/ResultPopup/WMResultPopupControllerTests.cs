using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Result.Core.WM;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
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

            _resultApi.Render("123", "123", 123, "en-US").Returns(expectedResult);

            //Action
            var result = controller.Index("123", "123", 123, "en-US");

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private WMResultPopupController CreateController()
            => new(_resultApi);
    }
}