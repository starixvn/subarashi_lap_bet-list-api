using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Extensions;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using Fanex.Athena.Result.Model.MetaModel;
using Fanex.Athena.Result.Service.Interfaces;
using Fanex.Result.Core.Base;
using Fanex.Result.Core.LeapGaming.DTO;
using Fanex.Result.Core.Sportsbook.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace Fanex.Athena.Result.Controller.ResultReports
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Route("[controller]/[action]")]
    public class ResultReportController : ControllerBase
    {
        private const string ResultExcelTemplateAddress = "App_Data/Fanex.Result/";

        private const int ParlayBetType = 9;
        private const int SystemParlayBetType = 29;

        private static readonly IEnumerable<SportType> ShowingMatchCodeSportTypes = new[]
        {
            SportType.VirtualHorseRacing,
            SportType.VirtualGreyhounds,
            SportType.VirtualSpeedway,
            SportType.VirtualMotorRacing,
            SportType.VirtualCycling,
            SportType.Bitcoin
        };

        private readonly IResultService _resultService;
        private readonly ICacheRefAdapter _cacheRefAdapter;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private string _htmlResult;
        private MemoryStream _excelStream;

        #region Result-Page

        public ResultReportController(
            IResultService resultService,
            ICacheRefAdapter cacheRefAdapter,
            IWebHostEnvironment webHostEnvironment)
        {
            _resultService = resultService;
            _cacheRefAdapter = cacheRefAdapter;
            _hostingEnvironment = webHostEnvironment;
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
                return File(_excelStream, "application/vnd.ms-excel", "Result.xls");
            }
            else
            {
                return BadRequest(ViewResources.InvalidParams);
            }

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
            await GetPopUpData(metalModel);
            return Content(_htmlResult);
        }

        [HttpPost]
        public async Task<IActionResult> ResultDetail(int matchId, int sportType, string languageCode)
        {
            var detailedData = await _resultService.GetSportsbookGameDetailAsync(matchId);
            if (detailedData != null && detailedData.MatchId > 0)
            {
                return Content(_resultService.HtmlGameDetailParser(
                detailedData,
                new ResultSetting
                {
                    SportType = (SportType)sportType,
                    LanguageCode = languageCode
                }));
            }

            return Content(string.Empty);
        }

        #endregion PopUpPage

        #region Utilities

        private static string GenerateOutRightCacheKey(ResultMetaModel metalModel)
        {
            const string PatternKey = "{0}{1}Result.OutRight.Data_{2}_{3}";
            return string.Format(PatternKey, metalModel.FromDate, metalModel.ToDate, metalModel.UserId, metalModel.LicenseeId);
        }

        #region popup-page

        private static bool IsPalayBetType(int betType)
        {
            return betType == ParlayBetType
                || betType == SystemParlayBetType
                || betType == (int)Common.Constant.BetType.VirtualSports2MixParlay
                || betType == (int)Common.Constant.BetType.VirtualSports4MixParlay;
        }

        private static IEnumerable<Sport> RemoveNotAllowedSports(IEnumerable<Sport> sports, IEnumerable<string> productCodes)
        {
            var limitedSports = sports;
            if (!productCodes.Contains(Common.Constant.ProductCodes.NG))
            {
                limitedSports = limitedSports.Where(i => i.SportId != (int)SportType.NumberGame && i.SportId != (int)SportType.Happy5);
            }

            if (!productCodes.Contains(Common.Constant.ProductCodes.VS))
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
                    var parlayData = await _resultService.GetPopUpParlayMatchAsync(metalModel.RefNo.Value);
                    _htmlResult = _resultService.HtmlParser(parlayData, setting);
                }
            }
            else if (metalModel.Bettype == (int)Common.Constant.BetType.SingleMatchParlay)
            {
                var singleMatchParlayData = await _resultService.GetPopUpSingleMatchParlayAsync(metalModel.MatchId, metalModel.BetId?.ToString());
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
            var leapgamings = new List<LeapGaming> { new LeapGaming { TransDesc = metalModel.TransDesc } };

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

        private async Task GetBetradarPopUpData(ResultPopupMetaModel metalModel, ResultSetting setting)
        {
            var eventDate = metalModel.WinLossDate.HasValue ? metalModel.WinLossDate.Value : DateTimeHelper.Now;
            var dataBetradar = await _resultService.GetPopUpBetradarMatchesAsync(metalModel.MatchId, eventDate).ConfigureAwait(false);
            if (dataBetradar != null)
            {
                _htmlResult = _resultService.HtmlParser(dataBetradar, setting);
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

            if (sportType == SportType.VirtualDogRacing || sportType == SportType.VirtualHorseRacing)
            {
                await GetBetradarPopUpData(metalModel, setting).ConfigureAwait(false);
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

                var isAllowAllProducts = productCodes.Contains(Common.Constant.ProductCodes.NG) && productCodes.Contains(Common.Constant.ProductCodes.VS);

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
            var cacheKey = GenerateOutRightCacheKey(metalModel);
            var outRightData = (IList<Outright>)HttpRuntime.Cache.Get(cacheKey) ?? await _resultService.GetOutRightAsync(metalModel);
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
            var cacheKey = GenerateOutRightCacheKey(metalModel);
            var outRightData = (IList<Outright>)HttpRuntime.Cache.Get(cacheKey) ?? await _resultService.GetOutRightAsync(metalModel);
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

        private async Task GetNormalData(ResultMetaModel metalModel)
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

                case SportType.VirtualHorseRacing:
                case SportType.VirtualDogRacing:
                    var dataDRc = await _resultService.GetBetradarSportMatchesAsync(metalModel).ConfigureAwait(false);
                    ParseResult(dataDRc, setting, metalModel.IsExportExcel);
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

        #endregion Utilities
    }
}
