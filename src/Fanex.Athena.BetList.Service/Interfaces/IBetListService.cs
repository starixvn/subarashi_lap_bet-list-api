// -----------------------------------------------------------------------
// <copyright file="IBetListService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using System.Threading.Tasks;

    public interface IBetListService
    {
        string GetLeagueName(int leagueId);

        Task<TicketResult> GetMixParlayDetailTicket(MixParlayDetailMetaModel metaModel);

        Task<TicketResult> GetBetListData(BetListMetaModel metaModel);
    }
}