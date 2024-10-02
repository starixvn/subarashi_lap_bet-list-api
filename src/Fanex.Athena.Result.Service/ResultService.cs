namespace Fanex.Athena.Result.Service
{
    using Common.Model.DTO;
    using DataAccess;
    using Fanex.Result.Core;
    using Fanex.Result.Core.Base;
    using Fanex.Result.Core.Bitcoin.DTO;
    using Fanex.Result.Core.Happy5.DTO;
    using Fanex.Result.Core.NumberGame.DTO;
    using Fanex.Result.Core.Racing.DTO;
    using Fanex.Result.Core.Sportsbook.DTO;
    using Interfaces;
    using Model.MetaModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class ResultService : IResultService
    {
        private readonly IResultDataAccess _dataAccess;

        private readonly IResultNumberGameDataAccess _numberGameDataAccess;

        public ResultService(IResultDataAccess dataAccess, IResultNumberGameDataAccess numberGameDataAccess)
        {
            _dataAccess = dataAccess;
            _numberGameDataAccess = numberGameDataAccess;
        }

        public async Task<IList<Outright>> GetOutRightAsync(ResultMetaModel metal)
        {
            return await _dataAccess.GetOutRightAsync(metal);
        }

        public async Task<IList<Sport>> GetSportsAsync(DateTime dateValue, int licenseeId)
        {
            return await _dataAccess.GetSportsAsync(dateValue, licenseeId);
        }

        public async Task<IList<League>> GetLeaguesAsync(DateTime dateValue, int sportId, IEnumerable<int> specialLeagues = null)
        {
            return await _dataAccess.GetLeaguesAsync(dateValue, sportId, specialLeagues);
        }

        #region ResultEngine wrapping

        public MemoryStream ExcelParser<T>(IList<T> dataList, ResultSetting setting)
        {
            return ResultEngine.ExcelParser<T>(dataList, setting);
        }

        public string HtmlGameDetailParser(dynamic data, ResultSetting setting)
        {
            return ResultEngine.HtmlGameDetailParser(data, setting);
        }

        public string HtmlParser<T>(IList<T> dataList, ResultSetting setting)
        {
            return ResultEngine.HtmlParser(dataList, setting);
        }

        #endregion ResultEngine wrapping

        #region Report

        public async Task<IList<Sportsbook>> GetMatchesAsync(ResultMetaModel metalModel)
        {
            return await _dataAccess.GetMatchesAsync<Sportsbook>(metalModel);
        }

        public async Task<IList<NumberGame>> GetNumberGameMatchesAsync(ResultMetaModel metalModel)
        {
            return await _numberGameDataAccess.GetNumberGameMatchesAsync(metalModel);
        }

        public async Task<IList<Happy5>> GetHappy5MatchesAsync(ResultMetaModel metalModel)
        {
            return await _numberGameDataAccess.GetHappy5MatchesAsync(metalModel);
        }

        public async Task<IList<Racing>> GetVirtualSportMatchesAsync(ResultMetaModel metalModel)
        {
            return await _dataAccess.GetVirtualSportMatchesAsync(metalModel);
        }

        #endregion Report

        #region Popup

        public async Task<Sportsbook> GetSportsbookGameDetailAsync(int matchId)
        {
            return await _dataAccess.GetSportsbookGameDetailAsync(matchId);
        }

        public async Task<IList<Sportsbook>> GetPopUpMatchAsync(ResultPopupMetaModel metalModel)
        {
            metalModel.RaceNo = null;
            return await _dataAccess.GetPopUpMatchesAsync<Sportsbook>(metalModel);
        }

        public async Task<IList<Racing>> GetPopUpVirtualSportMatchAsync(ResultPopupMetaModel metalModel)
        {
            return await _dataAccess.GetPopUpMatchesAsync<Racing>(metalModel);
        }

        public async Task<IList<Parlay>> GetPopUpParlayMatchAsync(long refNo)
        {
            return await _dataAccess.GetPopUpParalayMatchAsync(refNo);
        }

        public async Task<Sportsbook> GetPopUpSingleMatchParlayAsync(int matchId, string marketId)
        {
            return await _dataAccess.GetPopUpSingleMatchParlayAsync(matchId, marketId);
        }

        public async Task<IList<NumberGame>> GetPopUpNumberGameMatchsAsync(int matchId)
        {
            return await _numberGameDataAccess.GetPopUpNumberGameMatchAsync(matchId);
        }

        public async Task<IList<Happy5>> GetPopUpHappy5MatchesAsync(int matchId)
        {
            return await _numberGameDataAccess.GetPopUpHappy5MatchAsync(matchId);
        }

        public async Task<IList<Bitcoin>> GetPopUpBitcoinMatchesAsync(int matchId)
        {
            return await _dataAccess.GetPopUpMatches3rdAsync<Bitcoin>(matchId);
        }

        public async Task<IList<Bitcoin>> GetBitcoinMatchesAsync(ResultMetaModel metalModel)
        {
            return await _dataAccess.GetMatches3rdAsync<Bitcoin>(metalModel);
        }

        #endregion Popup
    }
}