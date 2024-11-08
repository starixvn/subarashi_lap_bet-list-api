namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using System.Threading.Tasks;

    public interface ILastBetsMonitoringBetListService
    {
        Task<TicketResult> GetLastBetsMonitoringData(LastBetsMonitoringMetaModel metaModel
           );
    }
}