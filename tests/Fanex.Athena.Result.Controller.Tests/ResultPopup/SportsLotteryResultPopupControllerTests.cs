using Fanex.Athena.Result.Controller.Constants;
using Fanex.Athena.Result.Controller.ResultPopup;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.SportsLottery;
using Fanex.Result.Core.ThirdParty;
using Fanex.Result.Core.ThirdParty.Models;
using NSubstitute;

namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    public class SportsLotteryResultPopupControllerTests
    {
        private IBetlistResultGetterFactory _factory;
        private ISportsLotteryResult _resultApi;
        private IThirdPartyResult _thirdPartyResult;

        [SetUp]
        public void SetUp()
        {
            _factory = Substitute.For<IBetlistResultGetterFactory>();
            _thirdPartyResult = Substitute.For<IThirdPartyResult>();
            _resultApi = Substitute.For<ISportsLotteryResult>();
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
            _factory.CreateThirdPartyResultGetter($"{ConfigName.ConfigPrefix}:{ConfigName.SportsLotteryDetailResult}")
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
            controller.ResultPage(platform, "en");

            //Assert
            _resultApi.Received().GetResultPage(isDesktop, language: "en");
        }

        private SportsLotteryResultPopupController CreateController()
            => new(_factory, _resultApi);
    }
}