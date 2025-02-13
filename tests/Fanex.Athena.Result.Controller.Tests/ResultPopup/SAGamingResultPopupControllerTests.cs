using Fanex.Athena.Result.Controller.Constants;
using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.SabaClub;
using Fanex.Result.Core.SAGaming;
using Fanex.Result.Core.ThirdParty;
using Fanex.Result.Core.ThirdParty.Models;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    public class SAGamingResultPopupControllerTests
    {
        private ISAGamingResultBuilder _resultApi;

        [SetUp]
        public void SetUp()
        {
            _resultApi = Substitute.For<ISAGamingResultBuilder>();
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
            var model = new SAGamingResultPopupController.SAGamingResultMetaModel
            {
                TransDesc = "123",
                MatchCode = "123",
                BetType = "betType",
                LanguageCode = "en-US"
            };

            _resultApi.Render("123", "123", "betType", "en-US").Returns(expectedResult);

            //Action
            var result = controller.Index(model);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private SAGamingResultPopupController CreateController()
            => new(_resultApi);
    }
}