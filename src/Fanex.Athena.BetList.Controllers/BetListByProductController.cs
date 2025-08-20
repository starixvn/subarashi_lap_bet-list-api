namespace Fanex.Athena.BetList.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Helpers;
    using Service.Interfaces;

    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
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
        [HttpPost]
        public async Task<object> Index(BetListByProductMetaModel metaModel)
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
