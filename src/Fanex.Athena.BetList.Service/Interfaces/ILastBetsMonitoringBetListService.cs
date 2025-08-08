namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface ILastBetsMonitoringBetListService
    {
        Task<TicketResult> GetLastBetsMonitoringData(LastBetsMonitoringMetaModel metaModel);
    }
}