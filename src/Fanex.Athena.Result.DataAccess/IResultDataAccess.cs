namespace Fanex.Athena.Result.DataAccess
{
    using Common.Model.DTO;
    using Fanex.Athena.Result.Model.MetaModel;
    using Fanex.Result.Core.Racing.DTO;
    using Fanex.Result.Core.Sportsbook.DTO;
    using Fanex.Result.Core.VirtualRacing.DTO;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResultDataAccess
    {
        Task<IList<Outright>> GetOutRightAsync(ResultMetaModel metal);

        #region Filter

        Task<IList<Sport>> GetSportsAsync(DateTime dateValue, int licenseeId);

        Task<IList<League>> GetLeaguesAsync(DateTime dateValue, int sportId, IEnumerable<int> specialLeagues = null);

        #endregion Filter

        #region Report

        Task<IList<T>> GetMatchesAsync<T>(ResultMetaModel metalModel);

        Task<IList<T>> GetMatches3rdAsync<T>(ResultMetaModel metalModel);

        Task<IList<Racing>> GetVirtualSportMatchesAsync(ResultMetaModel metalModel);

        Task<IList<VirtualRacing>> GetBetradarSportMatchesAsync(ResultMetaModel metalModel);

        #endregion Report

        #region Popup

        Task<IList<T>> GetPopUpMatchesAsync<T>(ResultPopupMetaModel metaModel);

        Task<IList<Parlay>> GetPopUpParalayMatchAsync(long refNo);

        Task<Sportsbook> GetSportsbookGameDetailAsync(int matchId);

        Task<IList<T>> GetPopUpMatches3rdAsync<T>(int matchId);

        Task<Sportsbook> GetPopUpSingleMatchParlayAsync(int matchId, string marketId);

        Task<IList<VirtualRacing>> GetPopUpBetradarAsync(int eventId, DateTime eventDate);

        #endregion Popup
    }
}