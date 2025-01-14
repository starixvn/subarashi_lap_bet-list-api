using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [BearerAuthorize(Roles.PlayerVIPManagement)]
    public class PlayerSegmentationBetListController
    {
        private readonly IPlayerSegmentationBetListDataAccess _dataAccess;
        private readonly ITicketBuilderService _ticketBuilderService;
        private readonly IProductAccess _productDataAccess;
        private readonly IContextService _contextService;

        public PlayerSegmentationBetListController(
            IPlayerSegmentationBetListDataAccess dataAccess,
            ITicketBuilderService ticketBuilderService,
            IProductAccess productDataAccess,
            IContextService contextService)
        {
            _dataAccess = dataAccess;
            _ticketBuilderService = ticketBuilderService;
            _productDataAccess = productDataAccess;
            _contextService = contextService;
        }

        [HttpPost]
        [AutoBindMetaModel(bindAgent: true, bindShowingBetFromIP: true, bindPlayerCom: true)]
        public async Task<object> AbnormalBets(PlayerSegmentationBetListMetaModel metaModel)
        {
            var listProduct = _productDataAccess.GetProductAccessList(_contextService.LicenseeId);
            metaModel.ListProductCode = listProduct.Select(_ => _.ProductCode);
            var tickets = await _dataAccess.GetAbnormalBetListAsync(metaModel);
            var result = new
            {
                Data = _ticketBuilderService.BuildTicketText(tickets, metaModel, metaModel.HasPlayerCom),
                tickets.TotalRows
            };

            return result;
        }
    }
}
