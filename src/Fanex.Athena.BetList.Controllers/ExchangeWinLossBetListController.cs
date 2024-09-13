using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class ExchangeWinLossBetListController
    {
        private readonly IExchangeBetListService _service;
        private readonly IExchangeExcel _exchangeExcel;

        public ExchangeWinLossBetListController(IExchangeBetListService service, IExchangeExcel exchangeExcel)
        {
            _service = service;
            _exchangeExcel = exchangeExcel;
        }

        [HttpPost]
        public async Task<ExchangeBetListViewModel> GetData(ExchangeBetListMetaModel metaModel)
            => await _service.GetBetListDataAsync(metaModel);
        

        [HttpPost]
        public async Task<IEnumerable<ExchangeTicketDetail>> Detail(ExchangeBetListDetailMetaModel metaModel)        
            => await _service.GetTicketDetailListAsync(metaModel);

        [HttpPost]
        public async Task<JsonResult> GetRunningTickets(ExchangeRunningTicketMetaModel metaModel)
        {
            var result = await _service.GetRunningTicketsAsync(metaModel);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<FileContentResult> ExportRunningTickets(ExchangeRunningTicketMetaModel metaModel)
        {
            var data = await _service.GetRunningTicketsAsync(metaModel);
            var template = Server.MapPath("~/App_Data/ExchangeRunningTicket.xls");
            var fileBytes = _exchangeExcel.ExportRunningExcel(template, data, metaModel);

            return new FileContentResult(fileBytes, "application/octet-stream");
        }

    }
}
