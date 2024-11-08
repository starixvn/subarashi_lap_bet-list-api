namespace Fanex.Athena.Result.Service.Interfaces
{
    using Common.Model.DTO;
    using Fanex.Result.Core.Base;
    using Fanex.Result.Core.Bitcoin.DTO;
    using Fanex.Result.Core.Happy5.DTO;
    using Fanex.Result.Core.NumberGame.DTO;
    using Fanex.Result.Core.Racing.DTO;
    using Fanex.Result.Core.Sportsbook.DTO;
    using Fanex.Result.Core.VirtualRacing.DTO;
    using Model.MetaModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IResultService
    {
        Task<IList<Outright>> GetOutRightAsync(ResultMetaModel metal);

        #region ResultEngine

        MemoryStream ExcelParser<T>(IList<T> dataList, ResultSetting setting);

        string HtmlGameDetailParser(dynamic data, ResultSetting setting);

        string HtmlParser<T>(IList<T> dataList, ResultSetting setting);

        #endregion ResultEngine

        #region Filter

        Task<IList<Sport>> GetSportsAsync(DateTime dateValue, int licenseeId);

        Task<IList<League>> GetLeaguesAsync(DateTime dateValue, int sportId, IEnumerable<int> specialLeagues = null);

        #endregion Filter

        #region Report

        Task<IList<Sportsbook>> GetMatchesAsync(ResultMetaModel metalModel);

        Task<IList<NumberGame>> GetNumberGameMatchesAsync(ResultMetaModel metalModel);

        Task<IList<Happy5>> GetHappy5MatchesAsync(ResultMetaModel metalModel);

        Task<IList<Racing>> GetVirtualSportMatchesAsync(ResultMetaModel metalModel);

        Task<IList<VirtualRacing>> GetBetradarSportMatchesAsync(ResultMetaModel metalModel);

        Task<IList<Bitcoin>> GetBitcoinMatchesAsync(ResultMetaModel metalModel);

        #endregion Report

        #region Popup

        Task<Sportsbook> GetSportsbookGameDetailAsync(int matchId);

        Task<IList<Sportsbook>> GetPopUpMatchAsync(ResultPopupMetaModel metalModel);

        Task<IList<Racing>> GetPopUpVirtualSportMatchAsync(ResultPopupMetaModel metalModel);

        Task<IList<Parlay>> GetPopUpParlayMatchAsync(long refNo);

        Task<IList<NumberGame>> GetPopUpNumberGameMatchsAsync(int matchId);

        Task<IList<Happy5>> GetPopUpHappy5MatchesAsync(int matchId);

        Task<IList<Bitcoin>> GetPopUpBitcoinMatchesAsync(int matchId);

        Task<IList<VirtualRacing>> GetPopUpBetradarMatchesAsync(int eventId, DateTime eventDate);

        Task<Sportsbook> GetPopUpSingleMatchParlayAsync(int matchId, string marketId);

        #endregion Popup
    }
}