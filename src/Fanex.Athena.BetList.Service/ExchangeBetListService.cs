namespace Fanex.Athena.BetList.Service
{
    using DataAccess.Interfaces;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;
    using Model.ViewModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ExchangeBetListService : IExchangeBetListService
    {
        private readonly IExchangeBetListDataAccess _dataAccess;

        public ExchangeBetListService(IExchangeBetListDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<ExchangeBetListViewModel> GetBetListDataAsync(ExchangeBetListMetaModel metaModel)
        {
            var betList = await _dataAccess.GetBetListDataAsync(metaModel);

            if (betList.SumWinLossList.Any() && betList.TicketList.Any())
            {
                var mainData = betList.SumWinLossList.GroupJoin(
                    betList.TicketList,
                    m => new { m.UserId, m.EventId, m.MarketId },
                    d => new { d.UserId, d.EventId, d.MarketId },
                   (main, detailList) => new ExchangeBetList { Ticket = main, Details = detailList }).Where(x => x.Details.Any());

                return new ExchangeBetListViewModel
                {
                    MainData = mainData,
                    GrandTotal = betList.GrandTotal
                };
            }

            return new ExchangeBetListViewModel();
        }

        public Task<IEnumerable<ExchangeTicketDetail>> GetTicketDetailListAsync(ExchangeBetListDetailMetaModel metaModel)
             => _dataAccess.GetTicketDetailListAsync(metaModel);


        public Task<ExchangeResult<ExchangeRunningTicket>> GetRunningTicketsAsync(ExchangeRunningTicketMetaModel metaModel)
            => _dataAccess.GetRunningTicketsAsync(metaModel);
    }
}