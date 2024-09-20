using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Extensions;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Services;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Attributes;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.Athena.Resources;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Fanex.Athena.BetList.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Route("api/[controller]/[action]")]
    [BearerAuthorize(Roles.ReportLastBetsMonitoring)]
    public class LastBetMonitoringBetListController
    {
        private const string ALL = "All";

        private static readonly IEnumerable<int> ConvertingDecimalOddsBetType = new[] { 1, 2, 3, 7, 8, 12, 20 };

        private readonly ILastBetsMonitoringBetListService _lastBetsMonitoringBetListService;
        private readonly ITicketHelper _ticketHelper;
        private readonly IIPService _ipService;
        private readonly ICustomerClassService _customerClassService;
        private readonly IContextService _contextService;
        private readonly ISportService _sportService;

        public LastBetMonitoringBetListController(
            ILastBetsMonitoringBetListService lastBetsMonitoringBetListService,
            ITicketHelper ticketHelper,
            IIPService ipService,
            ICustomerClassService customerClassService,
            IContextService contextService,
            ISportService sportService)
        {
            _lastBetsMonitoringBetListService = lastBetsMonitoringBetListService;
            _ticketHelper = ticketHelper;
            _ipService = ipService;
            _customerClassService = customerClassService;
            _contextService = contextService;
            _sportService = sportService;
        }


        [HttpPost]
        [RequestValidation]
        [AutoBindMetaModel(bindProductCodeList: true, bindShowingBetFromIP: true, bindSpecialPlatform: true)]
        public async Task<object> GetData(LastBetsMonitoringMetaModel metaModel)
        {
            var riskLevels = await _customerClassService.GetAllRiskLevel();
            var agentList = _contextService.AgentList.ToDictionary(_ => _.Key, _ => _.Value);

            if (string.IsNullOrWhiteSpace(metaModel.Currency))
            {
                metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(agentList);
            }
            else
            {
                var currencyIdList = metaModel.Currency.Split(',').Select(int.Parse);
                metaModel.AgentListXml = AuthParameterHelper.GenerateAgentListXml(agentList, currencyIdList);
            }
            metaModel.SportTypeList = BuildSportTypeList(metaModel.SportIds);
            metaModel.RiskLevels = riskLevels.ToList();

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

        private string BuildSportTypeList(string sportIds)
        {
            if (string.IsNullOrWhiteSpace(sportIds))
            {
                return string.Empty;
            }

            if (string.Equals(sportIds, ALL, StringComparison.OrdinalIgnoreCase))
            {
                return ALL;
            }

            var listSportId = sportIds.Split(',').Select(int.Parse);
            XDocument x = new XDocument(new XElement("ROOT"));
            var root = x.Root;
            foreach (var id in listSportId)
            {
                var elem = BuildSportTypeElement(id);
                root.Add(elem);
            }
            return x.ToString();
        }

        private XElement BuildSportTypeElement(int id)
        {
            const string _sport_tag = "Sport";
            const string _sport_type_attr = "SportType";
            const string _league_group_id_attr = "LeagueGroupIds";
            const int _default_league_group_id = 0;

            var elem = new XElement(_sport_tag);
            if (id > 0)
            {
                elem.SetAttributeValue(_sport_type_attr, id);
                elem.SetAttributeValue(_league_group_id_attr, _default_league_group_id);
            }
            else
            {
                var sabaSport = _sportService.GetSabaSport(id);
                var leagueGroupIds = string.Join(",", sabaSport.LeagueGroupIds);
                elem.SetAttributeValue(_sport_type_attr, sabaSport.SportTypeId);
                elem.SetAttributeValue(_league_group_id_attr, leagueGroupIds);
            }
            return elem;
        }
    }
}
