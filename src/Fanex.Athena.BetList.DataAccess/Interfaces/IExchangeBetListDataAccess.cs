namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IExchangeBetListDataAccess
    {
        Task<ExchangeBetListResult> GetBetListDataAsync(ExchangeBetListMetaModel metaModel);

        Task<IEnumerable<ExchangeTicketDetail>> GetTicketDetailListAsync(ExchangeBetListDetailMetaModel metaModel);

        Task<ExchangeResult<ExchangeRunningTicket>> GetRunningTicketsAsync(ExchangeRunningTicketMetaModel metaModel);
    }
}