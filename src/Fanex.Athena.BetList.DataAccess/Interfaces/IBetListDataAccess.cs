namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IBetListDataAccess
    {
        Task<TicketResult> GetMixParlayDetailTicket(MixParlayDetailMetaModel metaModel);

        Task<TicketResult> GetBetListData(BetListMetaModel metaModel);
    }
}