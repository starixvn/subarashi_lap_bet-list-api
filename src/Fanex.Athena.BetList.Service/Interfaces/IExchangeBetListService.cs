namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;
    using Model.ViewModel;

    public interface IExchangeBetListService
    {
        Task<ExchangeBetListViewModel> GetBetListDataAsync(ExchangeBetListMetaModel metaModel);

        Task<IEnumerable<ExchangeTicketDetail>> GetTicketDetailListAsync(ExchangeBetListDetailMetaModel metaModel);

        Task<ExchangeResult<ExchangeRunningTicket>> GetRunningTicketsAsync(ExchangeRunningTicketMetaModel metaModel);
    }
}