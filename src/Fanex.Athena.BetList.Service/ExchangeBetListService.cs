namespace Fanex.Athena.BetList.Service
{
    using DataAccess.Interfaces;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;
    using Model.ViewModel;

    public class ExchangeBetListService(IExchangeBetListDataAccess dataAccess) : IExchangeBetListService
    {
        public async Task<ExchangeBetListViewModel> GetBetListDataAsync(ExchangeBetListMetaModel metaModel)
        {
            var betList = await dataAccess.GetBetListDataAsync(metaModel);

            if (betList.SumWinLossList.Any() && betList.TicketList.Any())
            {
                var mainData = betList.SumWinLossList.GroupJoin(
                    betList.TicketList,
                    m => new { m.UserId, m.EventId, m.MarketId },
                    d => new { d.UserId, d.EventId, d.MarketId },
                    (main, detailList) => new ExchangeBetList { Ticket = main, Details = detailList.OrderBy(d => d.BetId) })
                        .Where(x => x.Details.Any())
                        .OrderBy(x => x.Ticket.MarketId);

                return new ExchangeBetListViewModel
                {
                    MainData = mainData,
                    GrandTotal = betList.GrandTotal
                };
            }

            return new ExchangeBetListViewModel();
        }

        public Task<IEnumerable<ExchangeTicketDetail>> GetTicketDetailListAsync(ExchangeBetListDetailMetaModel metaModel)
             => dataAccess.GetTicketDetailListAsync(metaModel);

        public Task<ExchangeResult<ExchangeRunningTicket>> GetRunningTicketsAsync(ExchangeRunningTicketMetaModel metaModel)
            => dataAccess.GetRunningTicketsAsync(metaModel);
    }
}