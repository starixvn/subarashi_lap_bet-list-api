namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using System.Threading.Tasks;

    public interface IPlayerSegmentationBetListDataAccess
    {
        Task<TicketResult> GetAbnormalBetListAsync(PlayerSegmentationBetListMetaModel metaModel);
    }
}