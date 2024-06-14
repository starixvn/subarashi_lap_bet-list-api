using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Resources;
using Microsoft.AspNetCore.Mvc;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [BearerAuthorize()]
    [Route("api/[controller]/[action]")]
    public class PlayerAdminBetListController : BettingTicketCenterController
    {
        private readonly IPlayerAdminBetListDataAccess _dataAccess;

        public PlayerAdminBetListController(
            IContextService contextService,
            IPlayerAdminBetListDataAccess dataAccess,
            IBettingTicketCenterDataAccess bettingTicketDataAccess,
            ITicketBuilderService ticketBuilderService,
            IBettingTicketCenterExcel excel)
            : base(contextService, bettingTicketDataAccess, ticketBuilderService, excel)
        {
            _dataAccess = dataAccess;
        }

        [HttpPost]
        public async Task<object> GetBetList(PlayerAdminBetListMetaModel metaModel)
        {
            var ticketResult = await _dataAccess.GetRunningBetListAsync(metaModel).ConfigureAwait(false);

            return ReturnData(ticketResult, GetBetListOption(metaModel), metaModel);
        }

        [HttpPost]
        public async Task<ActionResult> Export(PlayerAdminBetListMetaModel metaModel)
        {
            metaModel.PageIndex = 1;
            metaModel.PageSize = int.MaxValue - 1;
            var ticketResult = await _dataAccess.GetRunningBetListAsync(metaModel).ConfigureAwait(false);

            if (ticketResult.MainTicketList.Count == 0)
            {
                return new NoContentResult();
            }

            var excelModel = new BettingTicketCenterExcelModel
            {
                Data = ticketResult,
                Option = GetBetListOption(metaModel),
                TotalText = ViewResources.Total,
                Title = $"{ViewResources.Currency}: {metaModel.Currency}"
            };

            excelModel.Option.ShowTxId = metaModel.IsAllowTxIDColumn;

            return ExportExcel(excelModel);
        }

        private static BettingTicketCenterOption GetBetListOption(PlayerAdminBetListMetaModel metaModel)
        {
            return new BettingTicketCenterOption
            {
                ShowTotal = true,
                ShowSumStake = true,
                SpecialPlatform = metaModel.SpecialPlatform
            };
        }
    }
}
