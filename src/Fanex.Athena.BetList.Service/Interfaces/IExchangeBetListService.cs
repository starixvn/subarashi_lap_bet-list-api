using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;

namespace Fanex.Athena.BetList.Service.Interfaces
{
    public interface IExchangeBetListService
    {
        Task<ExchangeBetListViewModel> GetBetListDataAsync(ExchangeBetListMetaModel metaModel);

        Task<IEnumerable<ExchangeTicketDetail>> GetTicketDetailListAsync(ExchangeBetListDetailMetaModel metaModel);

        Task<ExchangeResult<ExchangeRunningTicket>> GetRunningTicketsAsync(ExchangeRunningTicketMetaModel metaModel);
    }
}