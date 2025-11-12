namespace Fanex.Athena.Result.Controller.ResultReports
{
    using Common.Constant;
    using Common.Model.DTO;
    using Common.Utility.Extensions;
    using Common.Utility.Helpers;
    using Common.Utility.Services.Interfaces;
    using Fanex.Result.Core.Base;
    using Fanex.Result.Core.LeapGaming.DTO;
    using Fanex.Result.Core.Sportsbook.DTO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Report.Common.Helpers;
    using Resources;
    using ResultService.Interfaces;
    using Service.Interfaces;
    using SportType = Fanex.Result.Core.Base.SportType;

    [ApiController]
    [Route("api/[controller]/[action]")]
    [Route("[controller]/[action]")]
    public class ResultReportController : ControllerBase
    {
        private const string ResultExcelTemplateAddress = "App_Data/Fanex.Result/";

        private const int ParlayBetType = 9;
        private const int SystemParlayBetType = 29;

        private static readonly Dictionary<int, string> ThirdPartySportTypeMapping = new()
        {
            {(int)Common.Constant.SportType.ClotPlay, ConfigName.ClotPlayResult},
            {(int)Common.Constant.SportType.AIGaming, ConfigName.AIGamingResult},
            {(int)Common.Constant.SportType.Smartsoft, ConfigName.SmartsoftResult},
            {(int)Common.Constant.SportType.GPILottery, ConfigName.GPILotteryResult},
            {(int)Common.Constant.SportType.BGaming, ConfigName.BGamingResult},
            {(int)Common.Constant.SportType.Gemini, ConfigName.GeminiResult},
            {(int)Common.Constant.SportType.VertexPlay, ConfigName.VertexPlayResult},
            {(int)Common.Constant.SportType.WowGaming, ConfigName.WowGamingResult}
        };

        private static readonly IEnumerable<SportType> ShowingMatchCodeSportTypes =
        [
            SportType.VirtualHorseRacing,
            SportType.VirtualGreyhounds,
            SportType.VirtualSpeedway,
            SportType.VirtualMotorRacing,
            SportType.VirtualCycling,
            SportType.Bitcoin
        ];

        private static readonly IEnumerable<int> PalayBetTypes =
        [
            ParlayBetType,
            SystemParlayBetType,
            (int)BetType.VirtualSports2MixParlay,
            (int)BetType.VirtualSports4MixParlay,
            (int)BetType.VirtualSports5MixParlay
        ];

        private readonly IResultService _resultService;
        private readonly ICacheRefAdapter _cacheRefAdapter;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IThirdPartySportResultService _thirdPartySportResultService;

        private string _htmlResult;
        private MemoryStream _excelStream;

        #region Result-Page

        public ResultReportController(
            IResultService resultService,
            ICacheRefAdapter cacheRefAdapter,
            IWebHostEnvironment webHostEnvironment,
            IThirdPartySportResultService thirdPartySportResultService)
        {
            _resultService = resultService;
            _cacheRefAdapter = cacheRefAdapter;
            _hostingEnvironment = webHostEnvironment;
            _thirdPartySportResultService = thirdPartySportResultService;
        }

        [HttpPost]
        public async Task<IActionResult> Index(ResultMetaModel metalModel)
        {
            metalModel.IsExportExcel = false;
            await GetData(metalModel);
            return Content(_htmlResult);
        }

        [HttpPost]
        public async Task<IActionResult> ResultExcel(ResultMetaModel metalModel)
        {
            if (ModelState.IsValid)
            {
                metalModel.IsExportExcel = true;
                metalModel.SportId = metalModel.SportId == 0
                    ? await GetSportId(metalModel.FromDate, metalModel.LicenseeId)
                    : metalModel.SportId;

                if (metalModel.SportId > 0)
                {
                    await GetData(metalModel);
                }

                _excelStream ??= ExportNoneData(metalModel);
                _excelStream.Seek(0, SeekOrigin.Begin);
                return File(_excelStream, "application/vnd.ms-excel", "Result.xls");
            }

            return BadRequest(ViewResources.InvalidParams);
        }

        [HttpPost]
        public async Task<IActionResult> GetSports(GetSportResultMetaModel metalModel)
        {
            if (metalModel.IsOutRight)
            {
                return await GetOutRightSports(metalModel);
            }

            if (metalModel.SportId == 0)
            {
                return await GetNormalSports(metalModel);
            }

            var leagues = await _resultService.GetLeaguesAsync(metalModel.FromDate, metalModel.SportId, metalModel.SpecialLeagues);

            if (metalModel.IsHideSaba)
            {
                leagues = leagues.Select(l => new League
                {
                    IsOutRight = l.IsOutRight,
                    LeagueId = l.LeagueId,
                    LeagueName = SabaHelper.RemoveSabaText(l.LeagueName)
                }).ToList();
            }

            return new JsonResult(new { Leagues = leagues });
        }

        #endregion Result-Page

        #region PopUpPage

        [HttpPost]
        public async Task<IActionResult> ResultPopup(ResultPopupMetaModel metalModel)
        {
            if (ThirdPartySportTypeMapping.TryGetValue(metalModel.SportType, out var configKey))
            {
                Response.StatusCode = StatusCodes.Status202Accepted;
                return new ContentResult { Content = _thirdPartySportResultService.GetResultUrl(metalModel, configKey).ToString() };
            }

            await GetPopUpData(metalModel);
            return Content(_htmlResult);
        }

        [HttpPost]
        public async Task<IActionResult> ResultDetail(ResultDetailMetaModel metaModel)
        {
            var detailedData = await _resultService.GetSportsbookGameDetailAsync(metaModel.MatchId);
            if (detailedData != null && detailedData.MatchId > 0)
            {
                return Content(_resultService.HtmlGameDetailParser(
                detailedData,
                new ResultSetting
                {
                    SportType = (SportType)metaModel.SportType,
                    LanguageCode = metaModel.LanguageCode
                }));
            }

            return Content(string.Empty);
        }

        #endregion PopUpPage

        #region popup-page

        private static bool IsPalayBetType(int betType) => PalayBetTypes.Contains(betType);

        private static IEnumerable<Sport> RemoveNotAllowedSports(IEnumerable<Sport> sports, IEnumerable<string> productCodes)
        {
            var limitedSports = sports;
            if (!productCodes.Contains(ProductCodes.NG))
            {
                limitedSports = limitedSports.Where(i => i.SportId != (int)SportType.NumberGame && i.SportId != (int)SportType.Happy5);
            }

            if (!productCodes.Contains(ProductCodes.VS))
            {
                limitedSports = limitedSports.Where(i => i.SportId != (int)SportType.VirtualSports);
            }

            return limitedSports;
        }

        private static bool IsVirtualSport(SportType type)
        {
            var isVirtualRacing = type == SportType.VirtualMotorRacing;

            return isVirtualRacing
                || type == SportType.VirtualGreyhounds
                || type == SportType.VirtualCycling
                || type == SportType.VirtualSpeedway;
        }

        private async Task GetPopUpData(ResultPopupMetaModel metalModel)
        {
            var sportType = (SportType)metalModel.SportType;
            var setting = new ResultSetting
            {
                SportType = sportType,
                IsOutright = metalModel.IsOutRight.HasValue && metalModel.IsOutRight.Value > 0,
                IsParlayPopUp = IsPalayBetType(metalModel.Bettype),
                IsPopUp = true,
                LanguageCode = Thread.CurrentThread.CurrentUICulture.Name,
                IsPopupOpenFromBetList = true
            };

            if (setting.IsParlayPopUp)
            {
                if (metalModel.RefNo != null)
                {
                    var parlayData = metalModel.Bettype == (int)BetType.VirtualSports5MixParlay
                        ? await _resultService.GetPopUpVS5ParlayMatchAsync(metalModel.RefNo.Value).ConfigureAwait(false)
                        : await _resultService.GetPopUpParlayMatchAsync(metalModel.RefNo.Value).ConfigureAwait(false);
                    _htmlResult = _resultService.HtmlParser(parlayData, setting);
                }
            }
            else if (metalModel.Bettype == (int)BetType.SingleMatchParlay)
            {
                var singleMatchParlayData = await _resultService.GetPopUpSingleMatchParlayAsync(metalModel.MatchId, metalModel.Extension1);
                if (singleMatchParlayData != null)
                {
                    _htmlResult = _resultService.HtmlParser(new List<Sportsbook> { singleMatchParlayData }, setting);
                }
            }
            else
            {
                await GetNormalPopUpData(metalModel, sportType, setting);
            }
        }

        private async Task GetVirtualSportPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            if (metalModel.RaceNo != null)
            {
                var dataRc = await _resultService.GetPopUpVirtualSportMatchAsync(metalModel);
                if (dataRc != null)
                {
                    _htmlResult = _resultService.HtmlParser(dataRc, setting);
                }
            }
        }

        private async Task GetNumberGamePopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var dataNg = await _resultService.GetPopUpNumberGameMatchsAsync(metalModel.MatchId);
            if (dataNg != null)
            {
                _htmlResult = _resultService.HtmlParser(dataNg, setting);
            }
        }

        private async Task GetHappy5PopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var dataHappy5 = await _resultService.GetPopUpHappy5MatchesAsync(metalModel.MatchId);
            if (dataHappy5 != null)
            {
                _htmlResult = _resultService.HtmlParser(dataHappy5, setting);
            }
        }

        private void GetLeapGamingPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var leapgamings = new List<LeapGaming> { new LeapGaming { TransDesc = metalModel.Extension2 } };

            _htmlResult = _resultService.HtmlParser(leapgamings, setting);
        }

        private async Task GetBitcoinPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var dataBitcoin = await _resultService.GetPopUpBitcoinMatchesAsync(metalModel.MatchId);
            if (dataBitcoin != null)
            {
                _htmlResult = _resultService.HtmlParser(dataBitcoin, setting);
            }
        }

        private async Task GetBetbyPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var dataBetby = await _resultService.GetPopUpMatch3rdAsync(metalModel.MatchId);
            if (dataBetby != null)
            {
                _htmlResult = _resultService.HtmlParser(dataBetby, setting);
            }
        }

        private async Task GetBetradarPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var eventDate = metalModel.WinLossDate.HasValue ? metalModel.WinLossDate.Value : DateTimeHelper.Now;
            var dataBetradar = await _resultService.GetPopUpBetradarMatchesAsync(metalModel.MatchId, eventDate);
            if (dataBetradar != null)
            {
                _htmlResult = _resultService.HtmlParser(dataBetradar, setting);
            }
        }

        private async Task GetVirtuallySportsPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var dataVirtuallySports = await _resultService.GetPopUpVS5MatchesAsync(metalModel.MatchId).ConfigureAwait(false);
            if (dataVirtuallySports != null)
            {
                _htmlResult = _resultService.HtmlParser(dataVirtuallySports, setting);
            }
        }

        private async Task GetOtherNormalPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var data = await _resultService.GetPopUpMatchAsync(metalModel);
            if (data != null)
            {
                _htmlResult = _resultService.HtmlParser(data, setting);
            }
        }

        private async Task GetNormalPopUpData(ResultPopupMetaModel metalModel, SportType sportType, ResultSetting setting)
        {
            if (IsVirtualSport(sportType))
            {
                await GetVirtualSportPopUpData(metalModel, setting);
                return;
            }

            if (sportType == SportType.NumberGame)
            {
                await GetNumberGamePopUpData(metalModel, setting);
                return;
            }

            if (sportType == SportType.Happy5)
            {
                await GetHappy5PopUpData(metalModel, setting);
                return;
            }

            if (sportType == SportType.LeapGamingVirtualSports)
            {
                GetLeapGamingPopUpData(metalModel, setting);
                return;
            }

            if (sportType == SportType.Bitcoin)
            {
                await GetBitcoinPopUpData(metalModel, setting);
                return;
            }

            if (SportHelper.IsVSNewRacingSportTypes((int)sportType))
            {
                await GetBetradarPopUpData(metalModel, setting);
                return;
            }

            if (sportType == SportType.BetbyBasketball)
            {
                await GetBetbyPopUpData(metalModel, setting);
                return;
            }

            if (sportType == SportType.VirtuallySports)
            {
                await GetVirtuallySportsPopUpData(metalModel, setting).ConfigureAwait(false);
                return;
            }

            await GetOtherNormalPopUpData(metalModel, setting);
        }

        #endregion popup-page

        #region spots list

        private async Task<JsonResult> GetNormalSports(GetSportResultMetaModel metalModel)
        {
            var sports = await _resultService.GetSportsAsync(metalModel.FromDate, metalModel.LicenseeId);
            if (sports.Any())
            {
                var productCodes = metalModel.ProductCodeList.Split(',');

                var isAllowAllProducts = productCodes.Contains(ProductCodes.NG) && productCodes.Contains(ProductCodes.VS);

                var limitedSports = isAllowAllProducts ? sports : RemoveNotAllowedSports(sports, productCodes);

                if (limitedSports.Any())
                {
                    var leagues = await _resultService.GetLeaguesAsync(metalModel.FromDate, limitedSports.First().SportId, metalModel.SpecialLeagues);
                    if (metalModel.IsHideSaba)
                    {
                        leagues = leagues.Select(l => new League
                        {
                            IsOutRight = l.IsOutRight,
                            LeagueId = l.LeagueId,
                            LeagueName = SabaHelper.RemoveSabaText(l.LeagueName)
                        }).ToList();
                    }
                    return new JsonResult(new { Sports = limitedSports, Leagues = leagues });
                }
            }

            return new JsonResult(string.Empty);
        }

        private async Task<JsonResult> GetOutRightSports(ResultMetaModel metalModel)
        {
            var outRightData = await _resultService.GetOutRightAsync(metalModel);
            if (outRightData != null && outRightData.Count > 0)
            {
                var sports = outRightData.Where(i => i.SportType == metalModel.SportId || metalModel.SportId == 0)
                        .Select(i => i.SportType).Distinct()
                        .Select(i => new Sport
                        {
                            SportId = i,
                            SportName = _cacheRefAdapter.GetSportName(i)
                        }).ToList();

                if (sports.Count > 0)
                {
                    var selectedSport = metalModel.SportId > 0 ? metalModel.SportId : sports[0].SportId;
                    var leagues = outRightData.Where(i => i.SportType == selectedSport)
                            .Select(i => i.LeagueId).Distinct()
                            .Select(i => new League
                            {
                                LeagueId = i,
                                LeagueName = _cacheRefAdapter.GetLeagueName(i)
                            }).ToList();

                    if (metalModel.IsHideSaba)
                    {
                        leagues = leagues.Select(l => new League
                        {
                            IsOutRight = l.IsOutRight,
                            LeagueId = l.LeagueId,
                            LeagueName = SabaHelper.RemoveSabaText(l.LeagueName)
                        }).ToList();
                    }

                    return new JsonResult(new { Sports = sports, Leagues = leagues });
                }
            }

            return new JsonResult(string.Empty);
        }

        #endregion spots list

        #region main-page

        private async Task<int> GetSportId(DateTime fromDate, int licenseeId)
        {
            var sports = await _resultService.GetSportsAsync(fromDate, licenseeId);
            return sports.Any() ? sports[0].SportId : 0;
        }

        private async Task GetOutRightData(ResultMetaModel metalModel)
        {
            var setting = GetOutRightResultSetting(metalModel.LanguageCode, metalModel.IsHideSaba);
            var outRightData = await _resultService.GetOutRightAsync(metalModel);
            if (outRightData != null)
            {
                var data =
                    outRightData.Where(i => i.SportType == metalModel.SportId && (metalModel.LeagueId == 0 || metalModel.LeagueId == i.LeagueId)).ToList();

                if (data.Count > 0)
                {
                    if (metalModel.IsExportExcel)
                    {
                        _excelStream = _resultService.ExcelParser(data, setting);
                    }
                    else
                    {
                        _htmlResult = _resultService.HtmlParser(data, setting);
                    }
                }
            }
        }

#pragma warning disable S1541 // Methods and properties should not be too complex

        private async Task GetNormalData(ResultMetaModel metalModel)
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            var setting = GetNormalResultSetting(metalModel);
            switch (setting.SportType)
            {
                case SportType.VirtualGreyhounds:
                case SportType.VirtualMotorRacing:
                case SportType.VirtualCycling:
                case SportType.VirtualSpeedway:
                    var dataRc = await _resultService.GetVirtualSportMatchesAsync(metalModel);
                    ParseResult(dataRc, setting, metalModel.IsExportExcel);
                    break;

                case SportType.NumberGame:
                    var dataNg = await _resultService.GetNumberGameMatchesAsync(metalModel);
                    ParseResult(dataNg, setting, metalModel.IsExportExcel);
                    break;

                case SportType.Happy5:
                    var dataHappy5 = await _resultService.GetHappy5MatchesAsync(metalModel);
                    ParseResult(dataHappy5, setting, metalModel.IsExportExcel);
                    break;

                case SportType.Bitcoin:
                    var dataBitcoin = await _resultService.GetBitcoinMatchesAsync(metalModel);
                    ParseResult(dataBitcoin, setting, metalModel.IsExportExcel);
                    break;

                case SportType.BetbyBasketball:
                    var dataBetby = await _resultService.GetMatches3rdAsync(metalModel);
                    ParseResult(dataBetby, setting, metalModel.IsExportExcel);
                    break;

                case SportType.VirtualHorseRacing:
                case SportType.VirtualDogRacing:
                case SportType.VirtualSteepleChase:
                case SportType.VirtualHarnessRacing:
                case SportType.VirtualMotorRacingVS7:
                    var dataDRc = await _resultService.GetVSNewRacingSportMatchesAsync(metalModel);
                    ParseResult(dataDRc, setting, metalModel.IsExportExcel);
                    break;

                case SportType.VirtuallySports:
                    var dataVirtuallySports = await _resultService.GetVS5MatchesAsync(metalModel).ConfigureAwait(false);
                    ParseResult(dataVirtuallySports, setting, metalModel.IsExportExcel);
                    break;

                default:
                    var data = await _resultService.GetMatchesAsync(metalModel);
                    ParseResult(data, setting, metalModel.IsExportExcel);
                    break;
            }
        }

        private void ParseResult<T>(IList<T> data, ResultSetting setting, bool isExportExcel)
        {
            if (data != null)
            {
                if (isExportExcel)
                {
                    _excelStream = _resultService.ExcelParser(data, setting);
                }
                else
                {
                    _htmlResult = _resultService.HtmlParser(data, setting);
                }
            }
        }

        private MemoryStream ExportNoneData(ResultMetaModel metalModel)
        {
            if (metalModel.IsOutRight)
            {
                var setting = GetOutRightResultSetting(metalModel.LanguageCode, metalModel.IsHideSaba);
                return _resultService.ExcelParser(new List<Outright>(), setting);
            }
            else
            {
                metalModel.SportId = (int)SportType.Soccer;
                var setting = GetNormalResultSetting(metalModel);
                return _resultService.ExcelParser(new List<Sportsbook>(), setting);
            }
        }

        private ResultSetting GetOutRightResultSetting(string languageCode, bool isHideSaba)
        {
            return new ResultSetting
            {
                IsOutright = true,
                ExcelTitle = ViewResources.Result,
                ExcelTemplatePath = _hostingEnvironment.MapPath(ResultExcelTemplateAddress),
                LanguageCode = languageCode,
                VisibleMatchCodeIDColumn = true,
                SupportHideSabaText = isHideSaba,
            };
        }

        private ResultSetting GetNormalResultSetting(ResultMetaModel metalModel)
        {
            var sportType = (SportType)metalModel.SportId;
            return new ResultSetting
            {
                SportType = sportType,
                ExcelTitle = ViewResources.Result,
                ExcelTemplatePath = _hostingEnvironment.MapPath(ResultExcelTemplateAddress),
                LanguageCode = Thread.CurrentThread.CurrentUICulture.Name,
                VisibleDetailedLink = true,
                VisibleFreeHeader = true,
                VisibleMatchCodeIDColumn = ShowingMatchCodeSportTypes.Contains(sportType),
                SupportHideSabaText = metalModel.IsHideSaba,
            };
        }

        private async Task GetData(ResultMetaModel metalModel)
        {
            if (metalModel.IsOutRight)
            {
                await GetOutRightData(metalModel);
            }
            else
            {
                await GetNormalData(metalModel);
            }
        }

        #endregion main-page
    }
}