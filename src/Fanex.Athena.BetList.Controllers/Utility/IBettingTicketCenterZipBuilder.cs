namespace Fanex.Athena.BetList.Controllers.Utility
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IBettingTicketCenterZipBuilder
    {
        MemoryStream BuildZip(GroupTicketResult ticketResult, string title, AdvancedSearchBetListMetaModel metaModel, string totalText, CancellationToken cancellationToken = default);
    }
}
