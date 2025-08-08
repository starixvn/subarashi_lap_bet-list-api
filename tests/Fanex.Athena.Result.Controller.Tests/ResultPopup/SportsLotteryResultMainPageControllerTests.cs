namespace Fanex.Athena.Result.Controller.Tests.ResultPopup
{
    using Controller.ResultPopup;
    using Fanex.Result.Core.SportsLottery;
    using Model.MetaModel;
    using NSubstitute;

    public class SportsLotteryResultMainPageControllerTests
    {
        private ISportsLotteryResult _resultApi;

        [SetUp]
        public void SetUp()
        {
            _resultApi = Substitute.For<ISportsLotteryResult>();
        }

        [TearDown]
        public void TearDown()
        {
            _resultApi = null;
        }


        [TestCase("a")]
        [TestCase("d")]
        public void Soccer5_WhenCall_ReturnCorrectResult(string platform)
        {
            //Arrange
            var controller = CreateController();
            var isDesktop = string.Equals(platform, "d", StringComparison.OrdinalIgnoreCase);
            var model = new SportsLotteryResultMetaModel
            {
                Platform = platform,
                LanguageCode = "en"
            };

            //Action
            controller.ResultPage(model);

            //Assert
            _resultApi.Received().GetResultPage(isDesktop, language: "en");
        }

        private SportsLotteryResultMainPageController CreateController()
            => new(_resultApi);
    }
}