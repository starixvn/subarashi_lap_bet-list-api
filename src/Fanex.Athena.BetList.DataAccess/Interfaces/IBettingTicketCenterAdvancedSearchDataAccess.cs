namespace Fanex.Athena.BetList.DataAccess.Interfaces;

using Model.DTO;
using Model.MetaModel;

public interface IBettingTicketCenterAdvancedSearchDataAccess
{
    Task<TicketResult> GetBetListForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel, CancellationToken cancellationToken);

    Task<GroupTicketResult> GetBetListForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel, int rowPerGroup, CancellationToken cancellationToken);

    Task<int> GetTotalRowForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel, int maxTotalRows, CancellationToken cancellationToken);
}