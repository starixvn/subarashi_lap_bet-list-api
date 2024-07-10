using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Extensions;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Resources;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    public class LastBetMonitoringBetListController
    {
        private static readonly IEnumerable<int> ConvertingDecimalOddsBetType = new[] { 1, 2, 3, 7, 8, 12, 20 };

        private readonly ILastBetsMonitoringBetListService _lastBetsMonitoringBetListService;

        private readonly ITicketHelper _ticketHelper;

        private readonly IIPService _ipService;

        public LastBetMonitoringBetListController(
            ILastBetsMonitoringBetListService lastBetsMonitoringBetListService,
            ITicketHelper ticketHelper,
            IIPService ipService)
        {
            _lastBetsMonitoringBetListService = lastBetsMonitoringBetListService;
            _ticketHelper = ticketHelper;
            _ipService = ipService;
        }

        [HttpPost]
        public async Task<object> GetData(LastBetsMonitoringMetaModel metaModel)
        {
            var ticketResult = await _lastBetsMonitoringBetListService.GetLastBetsMonitoringData(metaModel);
            var transLogIdSportType = new List<int> { (int)SportType.SportsLottery };

            if (ticketResult.MainTicketList.Any())
            {
                var rows = GetLastMonitorBetListContent(metaModel, ticketResult);
                var transIdTicket = ticketResult.MainTicketList.Where(t => !transLogIdSportType.Contains(t.SportTypeId)).OrderByDescending(t => t.TransId).FirstOrDefault();
                var transLogIdTicket = ticketResult.MainTicketList.Where(t => transLogIdSportType.Contains(t.SportTypeId)).OrderByDescending(t => t.TransId).FirstOrDefault();

                var result = new
                {
                    NewRow = true,
                    Rows = rows,
                    LastTransId = transIdTicket == null ? "0" : transIdTicket.TransId.ToString(),
                    LastTransLogId = transLogIdTicket == null ? "0" : transLogIdTicket.TransId.ToString()
                };
                BetListHelper.ClearData(ticketResult);
                return result;
            }
            else
            {
                return new
                {
                    NewRow = false
                };
            }
        }

        private string GetLastMonitorBetListContent(LastBetsMonitoringMetaModel metaModel, TicketResult ticketResult)
        {
            const string EndTag = "</td>";
            const string Uncategorized = "Uncategorized";

            var builder = new BetListHTMLBuilder();
            _ticketHelper.HideSabaText = metaModel.IsHideSaba;

            foreach (var ticket in ticketResult.MainTicketList)
            {
                _ticketHelper.ClientDetailSystemParlayFunction = GetClientDetailSystemParlayFunction(metaModel, ticket);

                var platformName = PlatformType.GetPlatformName(ticket.BetFrom.Trim(), metaModel.SpecialPlatform);
                var riskLevel = metaModel.RiskLevels.FirstOrDefault(_ => _.CustomerClassList.Contains(ticket.CustomerClass.ToString()))?.Name
                    ?? ViewResources.ResourceManager.GetString(Uncategorized);

                bool isShowIp = metaModel.IsShowIp && (metaModel.IsShowIpvn || !(_ipService.IsVNIP(ticket.IP)));

                var row = $"<tr class='content-row monitor-hightlight' id='trans-{ticket.TransId}' data-transid='{ticket.TransId}'>";
                builder.Append(row);

                builder.Append($"<td data-name='Platform' title='{platformName}'>{platformName}</td>");
                builder.Append($"<td data-name='ExternalId' class='special-color-{ticket.SpecialLevel}' title='{ticket.ExternalId}'>{ticket.ExternalId}</td>");
                builder.Append($"<td data-name='SystemId' class='special-color-{ticket.SpecialLevel}' title='{ticket.UserName}'>{ticket.UserName}</td>");
                builder.Append($"<td data-name='RiskLevel' title='{riskLevel}'>{riskLevel}</td>");
                builder.AddTrans(ticket, $"<td data-name='TransTime' class='special-color-0' title='{ticket.TransId}'>", EndTag);
                builder.AddChoice(
                    ticket,
                    _ticketHelper,
                    ticketResult.TicketDataList,
                    "<td data-name='Choice' class='bl-event'>",
                    EndTag,
                    false);
                BuildOdds(metaModel, ticketResult, ticket, builder);
                builder.AddStake(ticket, _ticketHelper, "<td data-name='Stake' class='bl-stake'>", EndTag);
                builder.Append($"<td data-name='Status' class='bl-status'>{builder.BuildLiveStatus(ticket, isShowIp)}</td></tr>");
            }

            return builder.GetBetList();
        }

        private string GetClientDetailSystemParlayFunction(LastBetsMonitoringMetaModel metaModel, LrfTicket ticket)
        {
            if (ticket.BetTypeId == (int)BetType.CombinationMixParlay)
            {
                var baseCurrency = metaModel == null ? string.Empty : metaModel.BasedCurrency;
                if (!SystemConstants.BaseCurrencyList.Contains(baseCurrency.ToUpperInvariant()))
                {
                    baseCurrency = string.Empty;
                }

                return BetListHelper.GenerateClientDetailSystemParlayFunction(_ticketHelper, baseCurrency, ticket, 0);
            }

            return string.Empty;
        }

        private void BuildOdds(LastBetsMonitoringMetaModel metaModel, TicketResult ticketResult, LrfTicket ticket, BetListHTMLBuilder builder)
        {
            const string OddStr = "<td data-name=\"Odds\" class=\"bl-odds\"><span class=\"underdog {1}\">{0}</span><br><span class=\"oddstype\">{2}</span></td>";
            if (metaModel == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(metaModel.OddsType)
                || !ConvertingDecimalOddsBetType.Contains(ticket.BetTypeId)
                || ticket.OddsType == OddsType.Myanmar)
            {
                builder.AddOdds(
                    ticket,
                    ticketResult.TicketDataList,
                    "<td data-name=\"Odds\" class='bl-odds'>",
                    "</td>",
                    _ticketHelper.GetValidOddsTypeNameById);
            }
            else
            {
                builder.Append(
                    string.Format(
                        OddStr,
                        Fanex.BetList.Core.Utils.ConvertByBetType.Odds(ticket.Odds),
                        ticket.Odds < 0 ? "negative" : string.Empty,
                        metaModel.OddsType));
            }
        }
    }
}
