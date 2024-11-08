using Fanex.Athena.BetList.Excel;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Model.ViewModel;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExchangeWinLossBetListController
    {
        private readonly IExchangeBetListService _service;
        private readonly IExchangeRunningExcel _exchangeRunningExcel;
        private readonly IExchangeWinLossExcel _exchangeWinLossExcel;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContextService _contextService;

        public ExchangeWinLossBetListController(IExchangeBetListService service,
            IExchangeRunningExcel exchangeRunningExcel,
            IExchangeWinLossExcel exchangeWinLossExcel,
            IWebHostEnvironment webHostEnvironment,
            IAuthorizationService authorizationService,
            IContextService contextService)
        {
            _service = service;
            _exchangeRunningExcel = exchangeRunningExcel;
            _webHostEnvironment = webHostEnvironment;
            _authorizationService = authorizationService;
            _contextService = contextService;
            _exchangeWinLossExcel = exchangeWinLossExcel;
        }

        [HttpPost]
        [BearerAuthorize(Roles.ReportExchangeWinLoss)]
        [RequestValidation(validateCurrency: true)]
        public async Task<IActionResult> GetData(ExchangeBetListMetaModel metaModel)
        {
            if (!IsValidRequest(metaModel))
            {
                return new JsonResult(new ExchangeBetListViewModel
                {
                    MainData = null
                });
            }
            metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(x => x.Key, x => x.Value), metaModel.CurrencyId, metaModel.ExcludeTestData);

            if (metaModel.IsExportExcel)
            {
                return await ExportExchangeWinLossExcelAsync(metaModel);
            }

            return new JsonResult(await _service.GetBetListDataAsync(metaModel));
        }


        [HttpPost]
        [BearerAuthorize(Roles.ReportExchangeWinLoss)]
        [RequestValidation(validateCurrency: true)]
        public async Task<IEnumerable<ExchangeTicketDetail>> Detail(ExchangeBetListDetailMetaModel metaModel)
        {
            if (!IsValidRequest(metaModel))
            {
                return new List<ExchangeTicketDetail>();
            }
            metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(_contextService.AgentList.ToDictionary(x => x.Key, x => x.Value), metaModel.CurrencyId, metaModel.ExcludeTestData);
            return await _service.GetTicketDetailListAsync(metaModel);
        }


        [HttpPost]
        [BearerAuthorize(Roles.BettingTicketCenter)]
        [AutoBindMetaModel(bindAgent: true)]
        public async Task<JsonResult> GetRunningTickets(ExchangeRunningTicketMetaModel metaModel)
        {
            var result = await _service.GetRunningTicketsAsync(metaModel);
            return new JsonResult(result);
        }

        [HttpPost]
        [AutoBindMetaModel(bindAgent: true)]
        [BearerAuthorize(Roles.BettingTicketCenter)]
        public async Task<FileContentResult> ExportRunningTickets(ExchangeRunningTicketMetaModel metaModel)
        {
            var data = await _service.GetRunningTicketsAsync(metaModel);
            var template = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data/Excel/ExchangeRunningTicket.xls");
            var fileBytes = _exchangeRunningExcel.ExportRunningExcel(template, data, metaModel);

            return new FileContentResult(fileBytes, "application/octet-stream");
        }

        public async Task<FileContentResult> ExportExchangeWinLossExcelAsync(ExchangeBetListMetaModel metaModel)
        {
            var data = await _service.GetBetListDataAsync(metaModel);
            var template = Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data/Excel/ExchangeWinLossTicket.xls");
            var fileBytes = _exchangeWinLossExcel.ExportExcel(template, data, metaModel);

            return new FileContentResult(fileBytes, "application/octet-stream");
        }

        private bool IsValidRequest(BaseExchangeBetListMetaModel metaModel)
            => _authorizationService.HasPermissionOnData(_contextService.AgentList, metaModel.CustId, 0);


    }
}
