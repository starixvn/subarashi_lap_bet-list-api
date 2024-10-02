namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using System.Threading.Tasks;

    public interface IBettingTicketCenterAdvancedSearchDataAccess
    {
        Task<TicketResult> GetBetListForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel);

        Task<GroupTicketResult> GetBetListForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel, int rowPerGroup);

        Task<int> GetTotalRowForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel);
    }
}