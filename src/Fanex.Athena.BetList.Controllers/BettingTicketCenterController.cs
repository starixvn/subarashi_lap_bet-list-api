namespace Fanex.Athena.BetList.Controllers
{
    using Common.Constant;
    using Common.Services.Interfaces;
    using Common.Site.Attributes;
    using DataAccess.Interfaces;
    using Excel;
    using Excel.Model;
    using Microsoft.AspNetCore.Mvc;
    using Model.DTO;
    using Model.MetaModel;
    using Resources;
    using Service.Interfaces;

    [ApiController]
    [BearerAuthorize]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class BettingTicketCenterController : BettingTicketCenterBaseController
    {
        private readonly IBettingTicketCenterDataAccess _dataAccess;
        private readonly IIPService _ipService;
        private readonly ICustomerClassService _customerClassService;
        private readonly ISportService _bettingTicketCenterSportService;

        public BettingTicketCenterController(
            IBettingTicketCenterDataAccess dataAccess,
            ITicketBuilderService ticketBuilderService,
            IBettingTicketCenterExcel bettingTicketCenterExcel,
            IIPService ipService,
            ICustomerClassService customerClassService,
            ISportService bettingTicketCenterSportService)
            : base(ticketBuilderService, bettingTicketCenterExcel)
        {
            _dataAccess = dataAccess;
            _ipService = ipService;
            _customerClassService = customerClassService;
            _bettingTicketCenterSportService = bettingTicketCenterSportService;
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
        public Task<IActionResult> GetByRunning(RunningBetListMetaModel metaModel)
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
        [BearerAuthorize(Roles.BettingTicketCenter)]
        [BearerAuthorize(Roles.BtcBookedBet)]
        [RequestValidation(validateCurrencyList: true)]
        [AutoBindMetaModel(bindAgent: true, bindShowingBetFromIP: true, bindSpecialPlatform: true, bindPlayerCom: true)]
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

            return new JsonResult(ReturnData(ticketResult, option, metaModel));
        }

        private async Task<IActionResult> GetBySingleCustomerResult(SingleCustomersBetListMetaModel metaModel)
        {
            metaModel.SportListXML = _bettingTicketCenterSportService.BuildSportTypeXML(metaModel.SportIds);
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

        private async Task<IActionResult> GetByRunningResult(RunningBetListMetaModel metaModel)
        {
            metaModel.FullCustomerClass = string.Join(",", await _customerClassService.GetAllCustomerClassId());
            metaModel.SportListXML = _bettingTicketCenterSportService.BuildSportTypeXML(metaModel.SportIds);

            var ticketResult = ShouldReturnEmptyResult(metaModel)
                ? new TicketResult() : await _dataAccess.GetRunningBetListAsync(metaModel);

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

        private bool ShouldReturnEmptyResult(RunningBetListMetaModel metaModel)
            => !string.IsNullOrWhiteSpace(metaModel.Ip) && !metaModel.IsShowIpvn && _ipService.IsVNIP(metaModel.Ip);
    }
}