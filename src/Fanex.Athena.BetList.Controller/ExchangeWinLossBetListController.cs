using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExchangeWinLossBetListController
    {
        private readonly IExchangeBetListService _service;

        public ExchangeWinLossBetListController(IExchangeBetListService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ExchangeBetListViewModel> GetData(ExchangeBetListMetaModel metaModel)
            => await _service.GetBetListDataAsync(metaModel);
        

        [HttpPost]
        public async Task<IEnumerable<ExchangeTicketDetail>> Detail(ExchangeBetListDetailMetaModel metaModel)        
            => await _service.GetTicketDetailListAsync(metaModel);
        
    }
}
