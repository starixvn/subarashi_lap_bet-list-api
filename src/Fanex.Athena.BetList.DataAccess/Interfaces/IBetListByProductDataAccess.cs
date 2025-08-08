namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IBetListByProductDataAccess
    {
        Task<TicketResult> QueryAsync(BetListByProductMetaModel metaModel);
    }
}
