namespace Fanex.Athena.BetList.Service.Interfaces
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using BetList.Model.ViewModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IExchangeBetListService
    {
        Task<ExchangeBetListViewModel> GetBetListDataAsync(ExchangeBetListMetaModel metaModel);

        Task<IEnumerable<ExchangeTicketDetail>> GetTicketDetailListAsync(ExchangeBetListDetailMetaModel metaModel);

        Task<ExchangeResult<ExchangeRunningTicket>> GetRunningTicketsAsync(ExchangeRunningTicketMetaModel metaModel);
    }
}