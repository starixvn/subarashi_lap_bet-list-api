using Fanex.Athena.Result.Controller.Constants;
using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.SabaClub;
using Fanex.Result.Core.ThirdParty;
using Fanex.Result.Core.ThirdParty.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
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

            _resultApi.GetResult("123", "123", "gameId", "siteId", 1, "en-US").Returns(expectedResult);

            //Action
            var result = controller.Index(model) as ContentResult;

            //Assert
            Assert.That(result.Content, Is.EqualTo(expectedResult));
        }

        private SabaClubResultPopupController CreateController()
            => new(_resultApi);
    }
}