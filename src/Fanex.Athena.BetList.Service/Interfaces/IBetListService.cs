namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IBetListService
    {
        string GetLeagueName(int leagueId);

        Task<TicketResult> GetMixParlayDetailTicket(MixParlayDetailMetaModel metaModel);

        Task<TicketResult> GetBetListData(BetListMetaModel metaModel);
    }
}