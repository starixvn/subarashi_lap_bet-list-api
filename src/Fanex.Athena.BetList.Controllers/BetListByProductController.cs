
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class BetListByProductController 
    {
        private readonly IBetListByProductService _service;

        private readonly ITicketBuilderService _ticketBuilderService;

        public BetListByProductController(IBetListByProductService betListByProductService, ITicketBuilderService ticketBuilderService)
        {
            _service = betListByProductService;
            _ticketBuilderService = ticketBuilderService;
        }

        [HttpGet]
        public async Task<object> Index([FromQuery]BetListByProductMetaModel metaModel)
        {
            var ticketResult = await _service.QueryAsync(metaModel);
            var result = new
            {
                Data = _ticketBuilderService.BuildTicketText(ticketResult, metaModel, metaModel.HasPlayerCom),
                ticketResult.TotalRows
            };
            BetListHelper.ClearData(ticketResult);
            return result;
        }
    }
}
