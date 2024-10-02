using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [BearerAuthorize()]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class PlayerAdminBetListController : BettingTicketCenterBaseController
    {
        private readonly IPlayerAdminBetListDataAccess _dataAccess;

        public PlayerAdminBetListController(
            IPlayerAdminBetListDataAccess dataAccess,
            ITicketBuilderService ticketBuilderService,
            IBettingTicketCenterExcel excel)
            : base(ticketBuilderService, excel)
        {
            _dataAccess = dataAccess;
        }

        [HttpPost]
        public async Task<object> GetBetList(PlayerAdminBetListMetaModel metaModel)
        {
            var ticketResult = await _dataAccess.GetRunningBetListAsync(metaModel);

            return ReturnData(ticketResult, GetBetListOption(metaModel), metaModel);
        }

        [HttpPost]
        public async Task<ActionResult> Export(PlayerAdminBetListMetaModel metaModel)
        {
            metaModel.PageIndex = 1;
            metaModel.PageSize = int.MaxValue - 1;
            var ticketResult = await _dataAccess.GetRunningBetListAsync(metaModel);

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
