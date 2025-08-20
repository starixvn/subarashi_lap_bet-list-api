namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    using Constants;
    using Controller.ResultPopup;
    using Fanex.Result.Core.ThirdParty;
    using Fanex.Result.Core.ThirdParty.Models;
    using Model.MetaModel;
    using NSubstitute;
    using Service.Interfaces;
    using TestHelper;

    public class VGamingResultPopupControllerTests
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
            _factory.CreateThirdPartyResultGetter($"{ConfigName.ConfigPrefix}:{ConfigName.VGamingResult}")
                .Returns(_thirdPartyResult);

            //Action
            controller.Index(metaModel);

            //Assert
            _thirdPartyResult.ReceivedWithAnyArgs().GetResultGET(Arg.Any<ResultRequest>());
        }

        private VGamingResultPopupController CreateController()
            => new(_factory);
    }
}