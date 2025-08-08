namespace Fanex.Athena.BetList.Controllers
{
    using Common.Constant;
    using Common.Site.Attributes;
    using DataAccess.Interfaces;
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Model.DTO;
    using Model.MetaModel;
    using Model.ViewModel;
    using Resources;
    using Service.Extensions;
    using Service.Interfaces;

    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class CashOutBetListController
    {
        private readonly ICashOutBetListDataAccess _cashOutBetListDataAccess;
        private readonly ITicketHelper _ticketHelper;
        private readonly IBetListHTMLBuilder _betListHTMLBuilder;
        private readonly IIPService _ipService;

        public CashOutBetListController(
            ICashOutBetListDataAccess cashOutBetListDataAccess,
            ITicketHelper ticketHelper,
            IIPService ipService,
            IBetListHTMLBuilder betListHTMLBuilder)
        {
            _cashOutBetListDataAccess = cashOutBetListDataAccess;
            _ticketHelper = ticketHelper;
            _betListHTMLBuilder = betListHTMLBuilder;
            _ipService = ipService;
        }

        [HttpPost]
        [AutoBindMetaModel(bindAgent: true, bindPlayerCom: true)]
        public async Task<CashOutBetListDetailViewModel> Detail(CashOutBetListDetailMetaModel metaModel)
        {
            var viewModel = await _cashOutBetListDataAccess.GetDetail(metaModel);
            if (viewModel.OriginalTicket != null)
            {
                viewModel.IsHighlightOriginalTicket = metaModel.TransId == viewModel.OriginalTicket.TransId ||
                    IsCashOutBetTicket(metaModel.BetdaqId, metaModel.BetType);
            }

            viewModel.OriginalTicketText = BuildOriginalTicketText(viewModel.OriginalTicket, metaModel.HasPlayerComm, metaModel.IsMobileView, metaModel.IsShowIpvn, metaModel.IsShowIp);
            viewModel.CashOutTicketTextList = BuildCashOutTicketTextList(viewModel.CashOutTicketList, metaModel.TransId, metaModel.IsShowIpvn, metaModel.IsShowIp);

            viewModel.IsCashOut = metaModel.IsCashOut;
            return viewModel;
        }

        [HttpPost]
        [AutoBindMetaModel(bindAgent: true, bindPlayerCom: true)]
        public async Task<CashOutHistoryDetailViewModel> History(CashOutHistoryDetailMetaModel metaModel)
        {
            var viewModel = await _cashOutBetListDataAccess.GetHistoryDetail(metaModel);

            return viewModel;
        }

        private static bool IsCashOutBetTicket(long betdaqid, int bettype)
        {
            var cashOutBetTypes = new[] { 900, 912 };
            return betdaqid == 1 && !cashOutBetTypes.Contains(bettype);
        }

        private IList<CashOutTicketText> BuildCashOutTicketTextList(IEnumerable<CashOutTicket> dataList, long transId, bool isShowIpvn = false, bool isShowIp = false)
        {
            if (dataList == null || !dataList.Any())
            {
                return new List<CashOutTicketText>();
            }

            var ticketTextList = new List<CashOutTicketText>();
            _ticketHelper.HideStatusCashOutHistory = true;

            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

            // If show ip check ipvn
            if (isShowIp)
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }


            foreach (var ticket in dataList)
            {
                var betListTicket = new LrfTicket
                {
                    BetTypeId = ticket.BetType,
                    Status = ticket.Status
                };
                var statusHtml = _betListHTMLBuilder.CreateStatusBuilder(ticket.BetType).Render(betListTicket, _ticketHelper, null, isShowIpvn);
                statusHtml.StatusResult = null;
                var ticketText = new CashOutTicketText
                {
                    TransTime = ViewResources.RefNo + ": " + ticket.TransId + Environment.NewLine + "<div class='time'>" + ticket.TransDate + "</div>",
                    Stake = ticket.Stake.ToString("#,##0.##"),
                    Price = ticket.Price.ToString(FormatKey.DecimalFormat),
                    WinLoss = ticket.WinLoss.ToString(FormatKey.DecimalFormat),
                    Status = statusHtml.ToString(),
                    IsHighlight = transId == ticket.TransId
                };

                ticketTextList.Add(ticketText);
            }

            return ticketTextList;
        }

        private TicketText BuildOriginalTicketText(LrfTicket ticket, bool hasPlayerComm, bool isMobileView = false, bool isShowIpvn = false, bool isShowIp = false)
        {
            if (ticket == null)
            {
                return new TicketText();
            }

            _ticketHelper.HideStatusCashOutHistory = true;

            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

            // If show ip check ipvn
            if (isShowIp)
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }

            var statusHtml = _betListHTMLBuilder.CreateStatusBuilder(ticket.BetTypeId).Render(ticket, _ticketHelper, null, isShowIpvn);
            statusHtml.StatusResult = null;
            var odds = isMobileView
                ? _betListHTMLBuilder.BuildOddsValue(ticket, null, _ticketHelper.GetValidOddsTypeNameById)
                : _betListHTMLBuilder.BuildOdds(ticket, null, _ticketHelper.GetValidOddsTypeNameById);
            var oddsTypeName = isMobileView ? _betListHTMLBuilder.BuildOddsTypeName(ticket, null, _ticketHelper.GetValidOddsTypeNameById) : null;

            return new TicketText
            {
                RefNo = isMobileView ? _betListHTMLBuilder.BuildRefNo(ticket) : null,
                ExternalId = ticket.ExternalId,
                SystemId = ticket.UserName,
                TransTime = isMobileView ? _betListHTMLBuilder.BuildTransTime(ticket) : _betListHTMLBuilder.BuildTrans(ticket),
                Status = statusHtml.ToString(),
                Choice = _betListHTMLBuilder.BuildChoice(ticket, _ticketHelper, null, false),
                Odds = odds,
                OddsType = oddsTypeName,
                Stake = _betListHTMLBuilder.BuildStake(ticket, _ticketHelper),
                WinLoss = _betListHTMLBuilder.BuildCustomerWL(ticket, hasPlayerComm)
            };
        }
    }
}
