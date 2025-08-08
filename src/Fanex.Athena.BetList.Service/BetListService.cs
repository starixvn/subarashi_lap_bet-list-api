namespace Fanex.Athena.BetList.Service
{
    using Common.DataAccess.Interfaces;
    using Common.Model.MetaModel;
    using Common.Services.Interfaces;
    using Common.Utility.Services.Interfaces;
    using DataAccess.Interfaces;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;

    public class BetListService : IBetListService
    {
        private readonly IBalanceUpDataAccess _balanceUpDataAccess;
        private readonly IBetListDataAccess _betlistDataAccess;
        private readonly ICacheRefAdapter _cacheRefAdapter;
        private readonly IShardInfoAccessor _shardInfoAccessor;

        public BetListService(
            IBalanceUpDataAccess balanceUpDataAccess,
            IBetListDataAccess betlistDataAccess,
            IShardInfoAccessor shardInfoAccessor,
            ICacheRefAdapter cacheRefAdapter)
        {
            _balanceUpDataAccess = balanceUpDataAccess;
            _betlistDataAccess = betlistDataAccess;
            _shardInfoAccessor = shardInfoAccessor;
            _cacheRefAdapter = cacheRefAdapter;
        }

        public string GetLeagueName(int leagueId)
        {
            return _cacheRefAdapter.GetLeagueName(leagueId);
        }

        public Task<TicketResult> GetMixParlayDetailTicket(MixParlayDetailMetaModel metaModel)
        {
            AssignBalanceUpDates(metaModel);
            return _betlistDataAccess.GetMixParlayDetailTicket(metaModel);
        }

        public Task<TicketResult> GetBetListData(BetListMetaModel metaModel)
        {
            AssignBalanceUpDates(metaModel);
            AssignShardDB(metaModel);
            return _betlistDataAccess.GetBetListData(metaModel);
        }

        private void AssignBalanceUpDates(IBalanceUpMetaModel metaModel)
        {
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            metaModel.MoveBetsDay = balanceUp.MoveBetsDay;
            metaModel.BalanceUpDay = balanceUp.BalanceUpDay;
        }

        private void AssignShardDB(IShardDBMetaModel metaModel)
            => metaModel.UseShardDB = _shardInfoAccessor.UseShardDB;
    }
}