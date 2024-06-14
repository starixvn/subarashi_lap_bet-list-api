using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Microsoft.AspNetCore.Http;

namespace Fanex.Athena.BetList.Controllers.Utility
{
    public interface IBettingTicketCenterZipBuilder
    {
        MemoryStream BuildZip(HttpResponse response, GroupTicketResult ticketResult, string title, AdvancedSearchBetListMetaModel metaModel, string totalText);
    }
}
