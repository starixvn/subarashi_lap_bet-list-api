
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.BetList.Service.Extensions;
using Fanex.Athena.BetList.Service.Helpers;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Utility.Helpers;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;

namespace Fanex.Athena.BetList.Service
{
    public class TicketBuilderService : ITicketBuilderService
    {
        private readonly IBetListHTMLBuilder _betlistHtmlBuilder;
        private readonly ITicketHelper _ticketHelper;
        private readonly IIPService _ipService;

        public TicketBuilderService(
            IBetListHTMLBuilder betlistHtmlBuilder,
            ITicketHelper ticketHelper,
            IIPService ipService)
        {
            _betlistHtmlBuilder = betlistHtmlBuilder;
            _ticketHelper = ticketHelper;
            _ipService = ipService;
        }

        public IList<TicketText> BuildTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel, bool isAllowedCustomerComm)
        {
            var startIndex = ((metaModel.PageIndex - 1) * metaModel.PageSize) + 1;
            var ticketTextList = new List<TicketText>();
            foreach (var ticket in ticketResult.MainTicketList)
            {
                if (ticket.BetTypeId == (int)BetType.CombinationMixParlay)
                {
                    _ticketHelper.ClientDetailSystemParlayFunction = BetListHelper.GenerateClientDetailSystemParlayFunction(
                        _ticketHelper,
                        metaModel.BaseCurrency,
                        ticket,
                        ticket.CustId);
                }

                _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

                if (metaModel.IsShowIp)
                {
                    _ticketHelper.IsVnIp = _ipService.IsVNIP;
                }

                _ticketHelper.HideSabaText = SabaHelper.IsHideSaba(metaModel.SpecialPlatform);

                var ticketText = new TicketText
                {
                    Index = startIndex++,
                    ExternalId = ticket.ExternalId,
                    SystemId = ticket.UserName,
                    TransTime = _betlistHtmlBuilder.BuildTransTime(ticket),
                    RefNo = _betlistHtmlBuilder.BuildRefNo(ticket),
                    Choice = _betlistHtmlBuilder.BuildChoice(ticket, _ticketHelper, ticketResult.TicketDataList, false),
                    Odds = _betlistHtmlBuilder.BuildOddsValue(ticket, ticketResult.TicketDataList, _ticketHelper.GetValidOddsTypeNameById),
                    OddsType = _betlistHtmlBuilder.BuildOddsTypeName(ticket, ticketResult.TicketDataList, _ticketHelper.GetValidOddsTypeNameById),
                    Stake = _betlistHtmlBuilder.BuildStake(ticket, _ticketHelper),
                    LicenseeWL = _betlistHtmlBuilder.BuildLicenseeWL(ticket),
                    WinLoss = _betlistHtmlBuilder.BuildCustomerWL(ticket, isAllowedCustomerComm),
                    Status = _betlistHtmlBuilder.BuildStatus(ticket, _ticketHelper, ticketResult.TicketDataList, metaModel.IsShowIpvn),
                    Platform = PlatformType.GetPlatformName(ticket.BetFrom, metaModel.SpecialPlatform),
                    Live = _betlistHtmlBuilder.BuildLiveStatus(ticket),
                    LicenseeTxId = _betlistHtmlBuilder.BuildLicenseeTxId(ticket)
                };

                if (BetListHelper.IsContribution(ticket.BetTypeId))
                {
                    BuildContributionTicket(ticketText);
                }

                ticketTextList.Add(ticketText);
            }

            return ticketTextList;
        }

#pragma warning disable S1541 // Methods and properties should not be too complex

        public List<TicketText> BuildTicketText(
#pragma warning restore S1541 // Methods and properties should not be too complex
            TicketResult ticketResult,
            BetListMetaModel metaModel = null,
            int startIndex = 1,
            bool showOdds = false,
            bool isAllowedCustomerComm = false,
            bool isMobileView = false,
            bool isShowIpvn = false,
            bool isShowIp = true)
        {
            var baseCurrency = metaModel == null ? string.Empty : metaModel.BaseCurrency;
            var ticketTextList = new List<TicketText>();

            _ticketHelper.IsShowOddsWithChoice = showOdds;

            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

            // If show ip check ipvn
            if ((metaModel != null) && metaModel.IsShowIp || isShowIp)
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }

            foreach (var ticket in ticketResult.MainTicketList)
            {
                if (ticket.BetTypeId == (int)BetType.CombinationMixParlay && metaModel != null)
                {
                    var custid = ticket.CustId != 0 ? ticket.CustId : metaModel.CustId;
                    _ticketHelper.ClientDetailSystemParlayFunction = BetListHelper.GenerateClientDetailSystemParlayFunction(_ticketHelper, baseCurrency, ticket, custid);
                }

                var odds = BuildOddsText(ticket, isMobileView);
                var oddsTypeName = BuildOddsTypeNameText(ticket, isMobileView);

                var ticketText = new TicketText
                {
                    RefNo = BuildRefNoText(ticket, isMobileView),
                    Index = startIndex++,
                    ExternalId = ticket.ExternalId,
                    SystemId = ticket.UserName,
                    TransTime = BuildTransTimeText(ticket, isMobileView),
                    Status = _betlistHtmlBuilder.BuildStatus(ticket, _ticketHelper, ticketResult.TicketDataList, (metaModel != null) && metaModel.IsShowIpvn || isShowIpvn),
                    Choice = _betlistHtmlBuilder.BuildChoice(ticket, _ticketHelper, ticketResult.TicketDataList, false),
                    Odds = odds,
                    OddsType = oddsTypeName,
                    Stake = _betlistHtmlBuilder.BuildStake(ticket, _ticketHelper),
                    LicenseeWL = _betlistHtmlBuilder.BuildLicenseeWL(ticket),
                    WinLoss = _betlistHtmlBuilder.BuildCustomerWL(ticket, isAllowedCustomerComm),
                    BetEffective = _betlistHtmlBuilder.BuildBetEffective(ticket),
                    TransId = ticket.TransId.ToString()
                };

                if (BetListHelper.IsContribution(ticket.BetTypeId))
                {
                    BuildContributionTicket(ticketText);
                }

                ticketTextList.Add(ticketText);
            }

            return ticketTextList;
        }

        public string BuildSumWinLoss(TicketResult ticketResult, bool isAllowedCustomerComm)
        {
            var ticket = new LrfTicket { Winlost = ticketResult.SumWinLoss, PlayerComm = ticketResult.SumPlayerComm };
            return _betlistHtmlBuilder.BuildCustomerWL(ticket, isAllowedCustomerComm);
        }

        private static void BuildContributionTicket(TicketText ticketText)
        {
            const string HyphenTextHTML = "<div class='underdog'>-</div>";

            ticketText.LicenseeWL = HyphenTextHTML;
            ticketText.WinLoss = HyphenTextHTML;
        }

        private string BuildOddsText(LrfTicket ticket, bool isMobileView)
        {
            return isMobileView
                   ? _betlistHtmlBuilder.BuildOddsValue(ticket, null, _ticketHelper.GetValidOddsTypeNameById)
                   : _betlistHtmlBuilder.BuildOdds(ticket, null, _ticketHelper.GetValidOddsTypeNameById);
        }

        private string BuildOddsTypeNameText(LrfTicket ticket, bool isMobileView)
        {
            return isMobileView
                ? _betlistHtmlBuilder.BuildOddsTypeName(ticket, null, _ticketHelper.GetValidOddsTypeNameById)
                : null;
        }

        private string BuildTransTimeText(LrfTicket ticket, bool isMobileView)
        {
            return isMobileView ? _betlistHtmlBuilder.BuildTransTime(ticket) : _betlistHtmlBuilder.BuildTrans(ticket);
        }

        private string BuildRefNoText(LrfTicket ticket, bool isMobileView)
        {
            return isMobileView ? _betlistHtmlBuilder.BuildRefNo(ticket) : null;
        }
    }
}