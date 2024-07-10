using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MixParlayBetListController
    {
        private readonly ICacheRefAdapter _cacheRefAdapter;

        private readonly IBetListService _betListService;

        private readonly ITicketHelper _ticketHelper;

        private readonly IBetListHTMLBuilder _betlistHtmlBuilder;
        private readonly ITicketBuilderService _ticketBuilderService;
        private readonly IIPService _ipService;


        public MixParlayBetListController(
            ICacheRefAdapter cacheRefAdapter,
            IBetListService betListService,
            ITicketHelper ticketHelper,
            IBetListHTMLBuilder betlistHtmlBuilder,
             ITicketBuilderService ticketBuilderService,
             IIPService ipService)
        {
            _cacheRefAdapter = cacheRefAdapter;
            _betListService = betListService;
            _ticketHelper = ticketHelper;
            _betlistHtmlBuilder = betlistHtmlBuilder;
            _ticketBuilderService = ticketBuilderService;
            _ipService = ipService;
        }

        [HttpPost]
        public async Task<object> Detail(MixParlayDetailMetaModel metaModel)
        {
            var ticketResult = await _betListService.GetMixParlayDetailTicket(metaModel);
            var result = _ticketBuilderService.BuildTicketText(ticketResult: ticketResult, showOdds: true, isShowIpvn: metaModel.IsShowIpvn, isHideSaba: metaModel.IsHideSaba);
            BetListHelper.ClearData(ticketResult);
            return result;
        }

        [HttpPost]
        public async Task<object> NewParlayDetail(NewParlayDetailMetaModel metaModel)
        {
            var texts = _cacheRefAdapter.GetCombinationTypeCombo(metaModel.BetTeam, metaModel.BetCheck, metaModel.Handicap1, metaModel.Handicap2);
            if (texts == null)
            {
                return null;
            }

            texts = (string[])texts.Clone(); // must do this weird step
            var ticketResult = await _betListService.GetMixParlayDetailTicket(metaModel);
            var tickets = ticketResult.MainTicketList.Where(x => x.Stake != 0).ToList();

            const string TextRenderPattern = "<p class='system-parlay-ticket-infor'>"
                                    + "<span class='system-parlay-combo-type'>{0}</span> x "
                                    + "<span class='system-parlay-unit-stake'>{1} {2}</span>"
                                    + "</p>";
            if (tickets.Any())
            {
                if (metaModel.Handicap1 == 0)
                {
                    // System Parlay.
                    var ticket = tickets.FirstOrDefault();
                    texts[0] = string.Format(TextRenderPattern, texts[0], ticket.Currency, ticket.Stake.ToString("N0"));
                }
                else
                {
                    // Combo parlay.
                    tickets = tickets.OrderBy(x => x.MatchId).ToList();
                    var length = Math.Min(texts.Count(), tickets.Count);
                    for (int i = 0; i < length; i++)
                    {
                        var text = texts[i];
                        var ticket = tickets[i];
                        texts[i] = string.Format(TextRenderPattern, text, ticket.Currency, ticket.Stake.ToString("N0"));
                    }
                }
            }

            var result = new
            {
                Data = BuildParlayDetailTickets(ticketResult: ticketResult, showOdds: true, isShowIpvn: metaModel.IsShowIpvn, isShowIp: metaModel.IsShowIp, isHideSaba: metaModel.IsHideSaba),
                Texts = texts
            };
            BetListHelper.ClearData(ticketResult);
            return result;
        }

        protected internal virtual IList<string> BuildParlayDetailTickets(TicketResult ticketResult, bool showOdds = false, bool isShowIpvn = false, bool isShowIp = false, bool isHideSaba = false)
        {
            var ticketTextList = new List<string>();

            _ticketHelper.IsShowOddsWithChoice = showOdds;


            _ticketHelper.HideSabaText = isHideSaba;
            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

            // If show ip check ipvn
            if (isShowIp)
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }

            foreach (var ticket in ticketResult.MainTicketList)
            {
                var statusTemplate = _betlistHtmlBuilder.CreateStatusBuilder(ticket.BetTypeId).Render(ticket, _ticketHelper, ticketResult.TicketDataList, isShowVNIP: isShowIpvn);
                statusTemplate.ShowIP = null;
                statusTemplate.StatusResult = null;

                var choice = _betlistHtmlBuilder.BuildChoice(ticket, _ticketHelper, ticketResult.TicketDataList, false);
                var transDateAndStatus = _betlistHtmlBuilder.BuildTransEventDateAndStatusResult(ticket, ticketResult.TicketDataList);

                ticketTextList.Add(choice + transDateAndStatus);
            }

            return ticketTextList;
        }

    }
}
