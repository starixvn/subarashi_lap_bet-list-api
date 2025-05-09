using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Result.Controller.ResultReports;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Athena.TestHelper;
using Fanex.Result.Core.Base;
using Fanex.Result.Core.Sportsbook.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Newtonsoft.Json;
using Fanex.Result.Core.NumberGame.DTO;
using Fanex.Result.Core.Racing.DTO;
using Fanex.Athena.Resources;
using System.Net;
using System.Text;
using Fanex.Athena.Common.Constant;
using Fanex.Result.Core.Bitcoin.DTO;
using Fanex.Result.Core.Happy5.DTO;
using SportType = Fanex.Result.Core.Base.SportType;
using Fanex.Result.Core.LeapGaming.DTO;
using Fanex.Athena.Result.Service;
using Fanex.Result.Core.VirtualRacing.DTO;
using Fanex.Athena.Result.Controller.Service.Interfaces;

namespace Fanex.Athena.Result.Controller.Tests.ResultReposts
{
    public class ResultReportControllerTests
    {
        private const string LanguageCode = "vi-VN";
        private const string HtmlResult = "<abc>hello</abc>";
        private ICacheRefAdapter _cacheRefAdapter;

        private IResultService _resultService;
        private IWebHostEnvironment _webHostEnvironment;
        private IThirdPartySportResultService _thirdPartySportResultService;

        [SetUp]
        public void SetUp()
        {
            _resultService = Substitute.For<IResultService>();
            _cacheRefAdapter = Substitute.For<ICacheRefAdapter>();
            _webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            _thirdPartySportResultService = Substitute.For<IThirdPartySportResultService>();
        }

        [TearDown]
        public void TearDown()
        {
            _cacheRefAdapter = null;
            _resultService = null;
            _webHostEnvironment = null;
        }

        [Test]
        public async Task ResultDetail_DataIsNull_ReturnEmptyResult()
        {
            // Arrange
            var controller = CreateController();
            const int MatchId = 23;
            const int SportType = 45445;
            _resultService.GetSportsbookGameDetailAsync(MatchId).Returns(x => Task.FromResult<Sportsbook>(null));

            // Action
            var result = await controller.ResultDetail(new ResultDetailMetaModel
            {
                MatchId = MatchId,
                SportType = SportType,
                LanguageCode = LanguageCode
            }) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.Empty);
        }

        [Test]
        public async Task ResultDetail_WhenCall_ReturnCorrectResult()
        {
            // Arrange
            var controller = CreateController();
            const string ExpectedHtmlGameDetail = "Abc";
            const int MatchId = 1;
            const int SportId = (int)SportType.Soccer;
            var detailedData = new Sportsbook
            {
                MatchId = MatchId
            };
            _resultService.GetSportsbookGameDetailAsync(MatchId).Returns(detailedData);
            _resultService.HtmlGameDetailParser(detailedData, Arg.Any<ResultSetting>()).Returns(ExpectedHtmlGameDetail);

            // Action
            var result = await controller.ResultDetail(new ResultDetailMetaModel
            {
                MatchId = MatchId,
                SportType = SportId,
                LanguageCode = LanguageCode
            }) as ContentResult;

            // Assert
            Assert.Multiple(() =>
            {
                _resultService.Received(1).HtmlGameDetailParser(
                    detailedData,
                    Arg.Is<ResultSetting>(x =>
                        x.SportType == (SportType)SportId &&
                        x.LanguageCode == LanguageCode));
                Assert.That(result.Content, Is.EqualTo(ExpectedHtmlGameDetail));
            });
        }

        [Test]
        public async Task GetSports_IsOutRight_ReturnOutrightResult()
        {
            // Arrange
            var controller = CreateController();
            const int SportId = 99;
            var dataList = new List<Outright>
            {
                new() { SportType = SportId }
            };
            var metaModel = new GetSportResultMetaModel
            {
                SportId = SportId,
                IsOutRight = true
            };
            _resultService.GetOutRightAsync(metaModel).Returns(dataList);

            // Action
            var result = await controller.GetSports(metaModel) as JsonResult;

            // Assert
            Assert.That(JsonConvert.SerializeObject(result.Value),
                Is.EqualTo(
                    @"{""Sports"":[{""SportId"":99,""SportName"":""""}],""Leagues"":[{""LeagueId"":0,""LeagueName"":"""",""IsOutRight"":false}]}"));
        }

        [Test]
        public async Task GetSports_SportIdIsZero_ReturnNormalSportsResult()
        {
            // Arrange
            var controller = CreateController();
            const int SportId = 99;
            var dataList = new List<Sport>
            {
                new() { SportId = SportId }
            };
            var metaModel = new GetSportResultMetaModel
            {
                SportId = 0,
                FromDate = new DateTime(2017, 1, 1),
                LicenseeId = 123,
                ProductCodeList = "SB"
            };
            _resultService.GetSportsAsync(metaModel.FromDate, metaModel.LicenseeId).Returns(dataList);

            // Action
            var result = await controller.GetSports(metaModel) as JsonResult;

            // Assert
            Assert.That(JsonConvert.SerializeObject(result.Value),
                Is.EqualTo(@"{""Sports"":[{""SportId"":99,""SportName"":null}],""Leagues"":[]}"));
        }

        [Test]
        public async Task GetSports_SportIdIsNotZero_ReturnCorrectResult()
        {
            // Arrange
            var controller = CreateController();
            const int SportId = 99;
            var dataList = new List<League>
            {
                new League()
            };
            var metaModel = new GetSportResultMetaModel
            {
                SportId = SportId,
                FromDate = new DateTime(2017, 1, 1)
            };
            _resultService.GetLeaguesAsync(metaModel.FromDate, SportId).Returns(dataList);

            // Action
            var result = await controller.GetSports(metaModel) as JsonResult;

            // Assert
            Assert.That(JsonConvert.SerializeObject(result.Value),
                Is.EqualTo(@"{""Leagues"":[{""LeagueId"":0,""LeagueName"":null,""IsOutRight"":false}]}"));
        }

        [Test]
        public async Task Index_ValidModelState_ResultContentIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            _resultService.HtmlParser(Arg.Any<IList<Sportsbook>>(), null).ReturnsForAnyArgs(HtmlResult);

            // Action
            var result = await controller.Index(new ResultMetaModel()) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(HtmlResult));
        }

        [Test]
        public async Task Index_IsOutRight_ResultContentIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            const int SportId = 1;
            var metaModel = new ResultMetaModel
            {
                SportId = SportId,
                IsOutRight = true
            };
            var dataList = new List<Outright>
            {
                new Outright
                {
                    SportType = SportId
                }
            };
            _resultService.GetOutRightAsync(metaModel).Returns(dataList);
            _resultService.HtmlParser(Arg.Any<IList<Outright>>(), Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.Index(metaModel) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(HtmlResult));
        }

        [TestCase((int)SportType.VirtualGreyhounds)]
        [TestCase((int)SportType.VirtualMotorRacing)]
        [TestCase((int)SportType.VirtualCycling)]
        [TestCase((int)SportType.VirtualSpeedway)]
        public async Task IndexPost_VirtualSport_ResultContentIsCorrect(int sportId)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = new ResultMetaModel
            {
                SportId = sportId,
                IsOutRight = false
            };
            var dataList = new List<Racing> { new Racing() };
            _resultService.GetVirtualSportMatchesAsync(metaModel).Returns(dataList);
            _resultService.HtmlParser(Arg.Any<IList<Racing>>(), Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.Index(metaModel) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(HtmlResult));
        }

        [Test]
        public async Task IndexPost_NumberGame_ResultContentIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = new ResultMetaModel
            {
                SportId = (int)SportType.NumberGame,
                IsOutRight = false
            };
            var dataList = new List<NumberGame> { new NumberGame() };
            _resultService.GetNumberGameMatchesAsync(metaModel).Returns(dataList);
            _resultService.HtmlParser(Arg.Any<IList<NumberGame>>(), Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.Index(metaModel) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(HtmlResult));
        }

        [Test]
        public async Task ResultExcel_InvalidModelState_ReturnBadRequestResponse()
        {
            // Arrange
            var controller = CreateController();
            controller.ModelState.AddModelError("error", "hihi");

            // Action
            var result = await controller.ResultExcel(new ResultMetaModel()) as ObjectResult;

            // Assert
            Assert.Multiple(() => { Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest)); });
        }

        [Test]
        public async Task ResultExcel_ValidModelState_ReturnCorrectResponse()
        {
            // Arrange
            const string ExpectedStreamData = "Hello everyone";
            var controller = CreateController();
            using (var excelStream = new MemoryStream(Encoding.UTF8.GetBytes(ExpectedStreamData)))
            {
                _resultService.ExcelParser(new List<Sportsbook>(), null).ReturnsForAnyArgs(excelStream);

                // Action
                var result = await controller.ResultExcel(new ResultMetaModel()) as FileStreamResult;

                // Assert
                Assert.That(Encoding.UTF8.GetString(((MemoryStream)result.FileStream).ToArray()),
                    Is.EqualTo(ExpectedStreamData));
            }
        }

        [Test]
        public async Task ResultPopup_ValidModelState_ResultIsCorrect()
        {
            var controller = CreateController();
            {
                // Arrange
                _resultService.HtmlParser(Arg.Any<IList<Sportsbook>>(), null).ReturnsForAnyArgs(HtmlResult);

                // Action
                var result = await controller.ResultPopup(new ResultPopupMetaModel()) as ContentResult;

                // Assert
                Assert.That(result.Content, Is.EqualTo(HtmlResult));
            }
        }

        [TestCase(1, true, (int)BetType.VirtualSports2MixParlay, true)]
        [TestCase(0, false, 123, false)]
        [TestCase(null, false, BetType.MixParlay, true)]
        public async Task ResultPopup_ValidModelState_CreateCorrectResultSetting(
            int? inputOutRight,
            bool expectedIsOutRight,
            int bettype,
            bool expectedIsParlayPopup)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = new ResultPopupMetaModel
            {
                SportType = (int)SportType.AGCasino,
                IsOutRight = inputOutRight,
                Bettype = bettype
            };

            // Action
            await controller.ResultPopup(new ResultPopupMetaModel()).ConfigureAwait(false);

            // Assert
            Assert.Multiple(() =>
            {
                _resultService.HtmlParser(
                    Arg.Any<IList<Sportsbook>>(),
                    Arg.Is<ResultSetting>(x =>
                        x.SportType == (SportType)metaModel.SportType &&
                        x.IsOutright == expectedIsOutRight &&
                        x.IsParlayPopUp == expectedIsParlayPopup &&
                        x.IsPopUp &&
                        x.LanguageCode == LanguageCode));
            });
        }

        [TestCase(1, HtmlResult)]
        [TestCase(0, HtmlResult)]
        [TestCase(null, null)]
        public async Task ResultPopup_IsParlayPopup_ResultIsCorrect(long? refNo, string expectedHtmlResult)
        {
            {
                // Arrange
                var controller = CreateController();
                var metaModel = new ResultPopupMetaModel
                {
                    Bettype = (int)BetType.VirtualSports2MixParlay,
                    RefNo = refNo
                };
                var dataList = new List<Parlay> { new Parlay() };
                _resultService.GetPopUpParlayMatchAsync(metaModel.RefNo.HasValue ? metaModel.RefNo.Value : 0)
                    .Returns(dataList);
                _resultService.HtmlParser(dataList, Arg.Any<ResultSetting>()).Returns(HtmlResult);

                // Action
                var result = await controller.ResultPopup(metaModel) as ContentResult;

                // Assert
                Assert.That(result.Content, Is.EqualTo(expectedHtmlResult));
            }
        }

        [TestCase(SportType.VirtualMotorRacing)]
        [TestCase(SportType.VirtualGreyhounds)]
        [TestCase(SportType.VirtualSpeedway)]
        [TestCase(SportType.VirtualCycling)]
        public async Task ResultPopup_VirtualSport_ResultIsCorrect(SportType sportType)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = CreateResultPopupMetaModel(sportType);
            var dataList = new List<Racing> { new Racing() };
            _resultService.GetPopUpVirtualSportMatchAsync(metaModel).Returns(dataList);
            _resultService.HtmlParser(dataList, Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.ResultPopup(metaModel) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(HtmlResult));
        }

        [Test]
        public async Task ResultPopup_NumberGameSport_ResultIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = CreateResultPopupMetaModel(SportType.NumberGame);
            var dataList = new List<NumberGame> { new NumberGame() };
            _resultService.GetPopUpNumberGameMatchsAsync(metaModel.MatchId).Returns(dataList);
            _resultService.HtmlParser(dataList, Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.ResultPopup(metaModel) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(HtmlResult));
        }

        [Test]
        public async Task ResultPopup_Happy5Sport_ResultIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = CreateResultPopupMetaModel(SportType.Happy5);
            var dataList = new List<Happy5> { new Happy5() };
            _resultService.GetPopUpHappy5MatchesAsync(metaModel.MatchId).Returns(dataList);
            _resultService.HtmlParser(dataList, Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.ResultPopup(metaModel) as ContentResult;

            // Assert
            Assert.That(result.Content, Is.EqualTo(HtmlResult));
        }

        [Test]
        public async Task ResultPopup_SportBookSports_ResultIsCorrect()
        {
            {
                // Arrange
                var controller = CreateController();
                var metaModel = CreateResultPopupMetaModel(SportType.Soccer);
                var dataList = new List<Sportsbook> { new Sportsbook() };
                _resultService.GetPopUpMatchAsync(metaModel).Returns(dataList);
                _resultService.HtmlParser(dataList, Arg.Any<ResultSetting>()).Returns(HtmlResult);

                // Action
                var result = await controller.ResultPopup(metaModel) as ContentResult;

                // Assert
                Assert.That(result.Content, Is.EqualTo(HtmlResult));
            }
        }

        [Test]
        public async Task ResultPopup_SingleMatchParlay_NoData_ResultNull()
        {
            {
                // Arrange
                var controller = CreateController();
                var metaModel = CreateResultPopupMetaModel(SportType.Soccer);
                metaModel.Bettype = (int)BetType.SingleMatchParlay;

                Sportsbook data = null;
                _resultService.GetPopUpSingleMatchParlayAsync(metaModel.MatchId, metaModel.Extension1?.ToString())
                    .Returns(Task.FromResult(data));

                // Action
                var result = await controller.ResultPopup(metaModel) as ContentResult;

                // Assert
                Assert.That(result?.Content, Is.Null);
            }
        }

        [Test]
        public async Task ResultPopup_SingleMatchParlay_ResultIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = CreateResultPopupMetaModel(SportType.Soccer);
            metaModel.Bettype = (int)BetType.SingleMatchParlay;

            var data = new Sportsbook();
            _resultService.GetPopUpSingleMatchParlayAsync(metaModel.MatchId, metaModel.Extension1?.ToString())
                .Returns(Task.FromResult(data));
            _resultService.HtmlParser(Arg.Is<List<Sportsbook>>(x => x[0] == data), Arg.Any<ResultSetting>())
                .Returns(HtmlResult);

            // Action
            var result = await controller.ResultPopup(metaModel) as ContentResult;

            // Assert
            Assert.Multiple(() =>
            {
                _resultService.Received(1).HtmlParser(Arg.Any<List<Sportsbook>>(),
                    Arg.Is<ResultSetting>(x => x.IsPopupOpenFromBetList == true));
                Assert.That(result.Content, Is.EqualTo(HtmlResult));
            });
        }

        [Test]
        public async Task ResultPopup_BitCoinSportType_ResultIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = CreateResultPopupMetaModel(SportType.Bitcoin);

            var dataList = new List<Bitcoin> { new Bitcoin() };
            _resultService.GetPopUpBitcoinMatchesAsync(metaModel.MatchId).Returns(dataList);
            _resultService.HtmlParser(dataList, Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.ResultPopup(metaModel) as ContentResult;

            // Assert
            Assert.Multiple(() => { Assert.That(result.Content, Is.EqualTo(HtmlResult)); });
        }

        [Test]
        public async Task ResultPopup_LeapGaming_ResultIsCorrect()
        {
            // Arrange
            var controller = CreateController();
            var metaModel = CreateResultPopupMetaModel(SportType.LeapGamingVirtualSports);

            _resultService.HtmlParser(Arg.Is<List<LeapGaming>>(x => x[0].TransDesc == metaModel.Extension2),
                Arg.Any<ResultSetting>()).Returns(HtmlResult);

            // Action
            var result = await controller.ResultPopup(metaModel) as ContentResult;

            // Assert
            Assert.Multiple(() => { Assert.That(result.Content, Is.EqualTo(HtmlResult)); });
        }

        [TestCase(SportType.VirtualDogRacing, true)]
        [TestCase(SportType.VirtualHorseRacing, false)]
        public async Task ResultPopup_BetRadar_ResultIsCorrect(SportType sportType, bool isGenWinLossDate)
        {
            // Arrange
            var controller = CreateController();
            var metaModel = CreateResultPopupMetaModel(sportType);
            metaModel.WinLossDate = isGenWinLossDate ? DateTime.Now : null;

            var dataList = new List<VirtualRacing> { new VirtualRacing() };
            _resultService.GetPopUpBetradarMatchesAsync(metaModel.MatchId, Arg.Any<DateTime>()).Returns(dataList);
            _resultService.HtmlParser(Arg.Is<List<VirtualRacing>>(x => x[0] == dataList[0]), Arg.Any<ResultSetting>())
                .Returns(HtmlResult);

            // Action
            var result = await controller.ResultPopup(metaModel) as ContentResult;

            // Assert
            Assert.Multiple(() => { Assert.That(result.Content, Is.EqualTo(HtmlResult)); });
        }

        private static ResultPopupMetaModel CreateResultPopupMetaModel(SportType sportType)
        {
            return new ResultPopupMetaModel
            {
                MatchId = 1,
                RaceNo = 2,
                Bettype = 3,
                Extension1 = 4.ToString(),
                SportType = (int)sportType
            };
        }

        private ResultReportController CreateController()
        {
            var controller = new ResultReportController(
                _resultService,
                _cacheRefAdapter,
                _webHostEnvironment,
                _thirdPartySportResultService);
            WebHelper.SetUpFakeControllerContext(controller);
            return controller;
        }
    }
}