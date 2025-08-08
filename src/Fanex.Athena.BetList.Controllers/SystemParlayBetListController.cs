namespace Fanex.Athena.BetList.Controllers
{
    using Fanex.BetList.Core.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Model.MetaModel;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class SystemParlayBetListController
    {
        private readonly ITicketHelper _ticketHelper;
        private readonly ISystemParlayDetailService _systemParlayDetailService;

        public SystemParlayBetListController(
            ITicketHelper ticketHelper,
            ISystemParlayDetailService systemParlayDetailService)
        {
            _ticketHelper = ticketHelper;
            _systemParlayDetailService = systemParlayDetailService;
        }

        [HttpPost]
        public async Task<IActionResult> Detail(SystemParlayDetailMetaModel metaModel)
        {
            const string viewPath =  "/Views/SystemParlayDetail/Index.cshtml";
            return new ContentResult 
            {
                Content = (await _systemParlayDetailService.BuildHtmlContent(metaModel, _ticketHelper, viewPath)),
            };
        }
    }
}
