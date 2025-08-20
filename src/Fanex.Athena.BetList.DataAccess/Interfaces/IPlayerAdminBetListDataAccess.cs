namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IPlayerAdminBetListDataAccess
    {
        Task<TicketResult> GetRunningBetListAsync(PlayerAdminBetListMetaModel metaModel);
    }
}
