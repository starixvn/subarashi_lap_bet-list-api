namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface ILastBetsMonitoringBetListDataAccess
    {
        Task<TicketResult> GetLastBetsMonitoringData(LastBetsMonitoringMetaModel metaModel);

        Task<IList<BetListMatch>> GetMatchList(string uspKey);

        Task<BetListMatch> GetMatchById(int matchId);
    }
}