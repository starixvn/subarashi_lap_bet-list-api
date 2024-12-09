using Fanex.Athena.Result.Controller.Constants;
using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.LinkedGames;
using Fanex.Result.Core.ThirdParty;
using Fanex.Result.Core.ThirdParty.Models;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    public class Happy5LinkedGamesResultPopupControllerTests
    {
        private IBetlistResultGetterFactory _factory;
        private ILinkedGamesResult _resultApi;
        private IThirdPartyResult _thirdPartyResult;

        [SetUp]
        public void SetUp()
        {
            _factory = Substitute.For<IBetlistResultGetterFactory>();
            _thirdPartyResult = Substitute.For<IThirdPartyResult>();
            _resultApi = Substitute.For<ILinkedGamesResult>();
        }

        [TearDown]
        public void TearDown()
        {
            _factory = null;
            _thirdPartyResult = null;
            _resultApi = null;
        }

        [Test]
        public void Index_WhenCall_ReturnCorrectResult()
        {
            //Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ThirdPartyMetaModel>();
            _factory.CreateThirdPartyResultGetter($"{ConfigName.ConfigPrefix}:{ConfigName.LinkedGamesResult}")
                .Returns(_thirdPartyResult);

            //Action
            controller.Index(metaModel);

            //Assert
            _thirdPartyResult.Received().GetResultGET(Arg.Is<ResultRequest>(x => x.Platform == "D"));
        }

        [TestCase("a")]
        [TestCase("d")]
        public void Soccer5_WhenCall_ReturnCorrectResult(string platform)
        {
            //Arrange
            var controller = CreateController();
            var isDesktop = string.Equals(platform, "d", StringComparison.OrdinalIgnoreCase);

            //Action
            controller.Soccer5("1", "2", platform, "en");

            //Assert
            _resultApi.Received().GetSoccer5Result("1", "2", isDesktop, language: "en");
        }

        private Happy5LinkedGamesResultPopupController CreateController()
            => new(_resultApi, _factory);
    }
}