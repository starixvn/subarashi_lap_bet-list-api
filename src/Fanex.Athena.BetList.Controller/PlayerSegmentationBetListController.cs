

using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PlayerSegmentationBetListController
    {
        private readonly IPlayerSegmentationBetListDataAccess _dataAccess;
        private readonly ITicketBuilderService _ticketBuilderService;

        public PlayerSegmentationBetListController(
            IPlayerSegmentationBetListDataAccess dataAccess,
            ITicketBuilderService ticketBuilderService)
        {
            _dataAccess = dataAccess;
            _ticketBuilderService = ticketBuilderService;
        }

        [HttpPost]
        public async Task<object> AbnormalBets(PlayerSegmentationBetListMetaModel metaModel)
        {
            var tickets = await _dataAccess.GetAbnormalBetListAsync(metaModel);
            var result = new
            {
                Data = _ticketBuilderService.BuildTicketText(tickets, metaModel, metaModel.IsAllowedCustomerComm),
                tickets.TotalRows
            };

            return result;
        }
    }
}
