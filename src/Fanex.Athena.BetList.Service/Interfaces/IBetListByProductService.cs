namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IBetListByProductService
    {
        Task<TicketResult> QueryAsync(BetListByProductMetaModel metaModel);
    }
}
