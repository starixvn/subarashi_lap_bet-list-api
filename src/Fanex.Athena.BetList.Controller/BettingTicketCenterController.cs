using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Excel.Model;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [BearerAuthorize()]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class BettingTicketCenterController : BettingTicketCenterBaseController
    {
        private readonly IContextService _contextService;
        private readonly IBettingTicketCenterDataAccess _dataAccess;

        public BettingTicketCenterController(
            IContextService contextService,
            IBettingTicketCenterDataAccess dataAccess,
            ITicketBuilderService ticketBuilderService,
            IBettingTicketCenterExcel bettingTicketCenterExcel)
            : base(ticketBuilderService, bettingTicketCenterExcel)
        {
            _contextService = contextService;
            _dataAccess = dataAccess;
        }

        [HttpPost]
        [BearerAuthorize(Roles.BettingTicketCenter, Roles.PortalSuperAdmin, Roles.PAAccountInfo)]
        [BearerAuthorize(Roles.BtcSingleCustomer, Roles.PortalSuperAdmin, Roles.PAAccountInfo)]
        [RequestValidation]
        [AutoBindMetaModel(bindAgent: true, bindShowingBetFromIP: true, bindProductCodeList: true, bindPlayerCom: true, bindSpecialPlatform: true)]
        public Task<IActionResult> GetBySingleCustomer(SingleCustomersBetListMetaModel metaModel)
            => GetBySingleCustomerResult(metaModel);

        [HttpPost]
        [BearerAuthorize(Roles.BettingTicketCenter, Roles.StatementServiceRoleUser)]
        [BearerAuthorize(Roles.BtcRefNo, Roles.StatementServiceRoleUser)]
        [RequestValidation]
        [AutoBindMetaModel(bindAgent: true, bindShowingBetFromIP: true, bindPlayerCom: true, bindSpecialPlatform: true)]
        public Task<IActionResult> GetByRefno(RefNoBetListMetaModel metaModel)
            => GetByRefnoResult(metaModel);

        [HttpPost]
        [BearerAuthorize(Roles.BettingTicketCenter)]
        [BearerAuthorize(Roles.BtcRunning)]
        [RequestValidation(validateCurrencyList: true)]
        [AutoBindMetaModel(bindAgent: true, applySubsidiaryBusiness: true, bindShowingBetFromIP: true, bindProductCodeList: true, bindPlayerCom: true, bindSpecialPlatform: true)]
        public Task<IActionResult> GetByRunning(BaseBettingTicketMetaModel metaModel)
            => GetByRunningResult(metaModel);

        [HttpPost]
        [BearerAuthorize(Roles.StatementServiceRoleUser)]
        [AutoBindMetaModel(bindAgent: true, bindShowingBetFromIP: true, bindSpecialPlatform: true, bindPlayerCom: true)]
        [RequestValidation]
        public async Task<IActionResult> GetByTxID(TxIDBetListMetaModel metaModel)
        {
            var ticketResult = await _dataAccess.GetBetListByTxIDAsync(metaModel);

            var option = new BettingTicketCenterOption
            {
                ShowLicenseeWl = true,
                ShowWinLoss = true,
                ShowTotal = false,
                ShowExternalId = true,
                ShowSystemId = true,
                IsAllowedCustomerComm = metaModel.HasPlayerCom,
                ShowTxId = true,
                IsShowIpvn = metaModel.IsShowIpvn,
                IsShowIp = metaModel.IsShowIp,
                SpecialPlatform = metaModel.SpecialPlatform,
            };

            if (metaModel.IsExport)
            {
                return Export(ticketResult, option, metaModel, null);
            }

            return new JsonResult(ReturnData(ticketResult, option, metaModel));
        }

        [HttpPost]
        public async Task<IActionResult> GetByBookedBet(BookedBetBetListMetaModel metaModel)
        {
            var ticketResult = await _dataAccess.GetBetListForBookedBetAsync(metaModel);

            var option = new BettingTicketCenterOption
            {
                IsBookedBet = true,
                ShowTotal = true,
                ShowSumStake = true,
                ShowSystemId = true,
                ShowExternalId = true,
                IsShowIpvn = metaModel.IsShowIpvn,
                IsShowIp = metaModel.IsShowIp
            };

            if (metaModel.IsExport)
            {
                return Export(ticketResult, option, metaModel, ViewResources.Total);
            }

            return ReturnData(ticketResult, option, metaModel);
        }

        private async Task<IActionResult> GetBySingleCustomerResult(SingleCustomersBetListMetaModel metaModel)
        {
            var ticketResult = await _dataAccess.GetBetListForSingleCustomerAsync(metaModel);

            var option = new BettingTicketCenterOption
            {
                ShowTotal = true,
                ShowSumStake = true,
                ShowLicenseeWl = true,
                ShowWinLoss = true,
                ShowExternalId = true,
                ShowSystemId = true,
                IsAllowedCustomerComm = metaModel.HasPlayerCom,
                ShowTxId = metaModel.IsAllowTxIDColumn,
                IsShowIpvn = metaModel.IsShowIpvn,
                IsShowIp = metaModel.IsShowIp,
                SpecialPlatform = metaModel.SpecialPlatform,
            };

            if (metaModel.IsExport)
            {
                return Export(ticketResult, option, metaModel, ViewResources.Total);
            }

            return new JsonResult(ReturnData(ticketResult, option, metaModel));
        }

        private async Task<IActionResult> GetByRefnoResult(RefNoBetListMetaModel metaModel)
        {
            var ticketResult = await _dataAccess.GetBetListByRefnoAsync(metaModel);

            var option = new BettingTicketCenterOption
            {
                ShowLicenseeWl = true,
                ShowWinLoss = true,
                ShowTotal = false,
                ShowExternalId = true,
                ShowSystemId = true,
                IsAllowedCustomerComm = metaModel.HasPlayerCom,
                ShowTxId = metaModel.IsAllowTxIDColumn,
                IsShowIpvn = metaModel.IsShowIpvn,
                IsShowIp = metaModel.IsShowIp,
                SpecialPlatform = metaModel.SpecialPlatform,
                ViewSabaCoins = metaModel.ViewSabaCoins
            };

            if (metaModel.IsExport)
            {
                return Export(ticketResult, option, metaModel, null);
            }

            return new JsonResult(ReturnData(ticketResult, option, metaModel));
        }

        private async Task<IActionResult> GetByRunningResult(BaseBettingTicketMetaModel metaModel)
        {
            var ticketResult = await _dataAccess.GetRunningBetListAsync(metaModel);

            var option = new BettingTicketCenterOption
            {
                ShowTotal = true,
                ShowSumStake = true,
                IsAllowedCustomerComm = false,
                ShowExternalId = true,
                ShowSystemId = true,
                ShowTxId = metaModel.IsAllowTxIDColumn,
                IsShowIpvn = metaModel.IsShowIpvn,
                IsShowIp = metaModel.IsShowIp,
                SpecialPlatform = metaModel.SpecialPlatform,
            };

            if (metaModel.IsExport)
            {
                return Export(ticketResult, option, metaModel, $"{ViewResources.Totalin} {metaModel.BaseCurrencyName}");
            }

            return new JsonResult(ReturnData(ticketResult, option, metaModel));
        }

        private IActionResult Export(TicketResult ticket, BettingTicketCenterOption option, BaseBettingTicketMetaModel metaModel, string totalText)
        {
            if (ticket.MainTicketList.Count == 0)
            {
                return new NoContentResult();
            }

            var excelModel = new BettingTicketCenterExcelModel
            {
                Data = ticket,
                Option = option,
                Title = GetTitle(metaModel),
                TotalText = totalText
            };

            return ExportExcel(excelModel);
        }
    }
}
