using Fanex.Athena.Result.Controller.Constants;
using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.ThirdParty;
using Fanex.Result.Core.ThirdParty.Models;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    public class PlayStarResultPopupControllerTests
    {
        private IBetlistResultGetterFactory _factory;
        private IThirdPartyResult _thirdPartyResult;

        [SetUp]
        public void SetUp()
        {
            _factory = Substitute.For<IBetlistResultGetterFactory>();
            _thirdPartyResult = Substitute.For<IThirdPartyResult>();
        }

        [TearDown]
        public void TearDown()
        {
            _factory = null;
            _thirdPartyResult = null;
        }

        [Test]
        public void Index_WhenCall_ReturnCorrectResult()
        {
            //Arrange
            var controller = CreateController();
            var metaModel = FakeHelper.Create<ThirdPartyMetaModel>();
            _factory.CreateThirdPartyResultGetter($"{ConfigName.ConfigPrefix}:{ConfigName.PlayStarResult}")
                .Returns(_thirdPartyResult);

            //Action
            controller.Index(metaModel);

            //Assert
            _thirdPartyResult.ReceivedWithAnyArgs().GetResultGET(Arg.Any<ResultRequest>());
        }

        private PlayStarResultPopupController CreateController()
            => new(_factory);
    }
}