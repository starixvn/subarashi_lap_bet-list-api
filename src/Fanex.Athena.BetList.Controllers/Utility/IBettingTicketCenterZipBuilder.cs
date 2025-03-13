using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;

namespace Fanex.Athena.BetList.Controllers.Utility
{
    public interface IBettingTicketCenterZipBuilder
    {
        MemoryStream BuildZip(GroupTicketResult ticketResult, string title, AdvancedSearchBetListMetaModel metaModel, string totalText);
    }
}
