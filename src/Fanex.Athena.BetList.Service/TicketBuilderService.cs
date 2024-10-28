
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

        private const string HyphenTextHTML = "<div class='underdog'>-</div>";

        public TicketBuilderService(
            IBetListHTMLBuilder betlistHtmlBuilder,
            ITicketHelper ticketHelper,
            IIPService ipService)
        {
            _betlistHtmlBuilder = betlistHtmlBuilder;
            _ticketHelper = ticketHelper;
            _ipService = ipService;
        }

        public IList<TicketText> BuildBookedBetTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel)
        {
            var startIndex = ((metaModel.PageIndex - 1) * metaModel.PageSize) + 1;
            var ticketTextList = new List<TicketText>();

            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

            if (metaModel.IsShowIp)
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }

            _ticketHelper.HideSabaText = SabaHelper.IsHideSaba(metaModel.SpecialPlatform);

            foreach (var ticket in ticketResult.MainTicketList)
            {
                bool isShowIp = metaModel.IsShowIp && (metaModel.IsShowIpvn || !(_ipService.IsVNIP(ticket.IP)));
                var bookedBetRefno = ticket.RefNo;
                ticket.RefNo = SystemConstants.DefaultBookedBetRefno;

                var ticketText = new TicketText
                {
                    Index = startIndex++,
                    ExternalId = ticket.ExternalId,
                    SystemId = ticket.UserName,
                    BookedTime = _betlistHtmlBuilder.BuildTransTime(ticket),
                    RefNo = bookedBetRefno != "0" ? bookedBetRefno : string.Empty,
                    BookedId = ticket.TransId.ToString(),
                    Choice = _betlistHtmlBuilder.BuildChoice(ticket, _ticketHelper, ticketResult.TicketDataList, false),
                    OddsLower = ticket.OddsLower == 0 ? HyphenTextHTML : _betlistHtmlBuilder.BuildOddsUpperLower(ticket.OddsLower),
                    OddsUpper = ticket.OddsUpper == 0 ? HyphenTextHTML : _betlistHtmlBuilder.BuildOddsUpperLower(ticket.OddsUpper),
                    OddsType = BuildOddsTypeNameText(ticket, ticketResult),
                    Stake = _betlistHtmlBuilder.BuildStake(ticket, _ticketHelper),
                    Status = _betlistHtmlBuilder.BuildBookedBetStatus(ticket, _ticketHelper.GetResourceData, isShowIp),
                    Platform = PlatformType.GetPlatformName(ticket.BetFrom, metaModel.SpecialPlatform)
                };

                ticketTextList.Add(ticketText);
            }

            return ticketTextList;
        }

        public IList<TicketText> BuildTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel, bool isAllowedCustomerComm)
        {
            var startIndex = ((metaModel.PageIndex - 1) * metaModel.PageSize) + 1;
            var ticketTextList = new List<TicketText>();

            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;
            if (metaModel.IsShowIp)
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }
            _ticketHelper.HideSabaText = SabaHelper.IsHideSaba(metaModel.SpecialPlatform);
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

                var ticketText = new TicketText
                {
                    Index = startIndex++,
                    ExternalId = ticket.ExternalId,
                    SystemId = ticket.UserName,
                    TransTime = _betlistHtmlBuilder.BuildTransTime(ticket),
                    RefNo = _betlistHtmlBuilder.BuildRefNo(ticket),
                    Choice = _betlistHtmlBuilder.BuildChoice(ticket, _ticketHelper, ticketResult.TicketDataList, false),
                    Odds = _betlistHtmlBuilder.BuildOddsValue(ticket, ticketResult.TicketDataList, _ticketHelper.GetValidOddsTypeNameById),
                    OddsType = BuildOddsTypeNameText(ticket, ticketResult),
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
            bool isShowIp = true,
            bool isHideSaba = false)
        {
            var baseCurrency = metaModel == null ? string.Empty : metaModel.BaseCurrency;
            var ticketTextList = new List<TicketText>();

            _ticketHelper.IsShowOddsWithChoice = showOdds;

            _ticketHelper.IsVnIp = _ipService.IsAlwaysIpvn;

            // If show ip check ipvn
            if (IsShowIp(metaModel, isShowIp))
            {
                _ticketHelper.IsVnIp = _ipService.IsVNIP;
            }

            _ticketHelper.HideSabaText = (metaModel != null) ? metaModel.IsHideSaba : isHideSaba;

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
                    Status = _betlistHtmlBuilder.BuildStatus(ticket, _ticketHelper, ticketResult.TicketDataList, IsShowIpVn(metaModel, isShowIpvn)),
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


            static bool IsShowIp(BetListMetaModel metaModel, bool configShow)
                => (metaModel != null) && metaModel.IsShowIp || configShow;

            static bool IsShowIpVn(BetListMetaModel metaModel, bool configShow)          
                => (metaModel != null) && metaModel.IsShowIpvn || configShow;
            
        }

        public string BuildSumWinLoss(TicketResult ticketResult, bool isAllowedCustomerComm)
        {
            var ticket = new LrfTicket { Winlost = ticketResult.SumWinLoss, PlayerComm = ticketResult.SumPlayerComm };
            return _betlistHtmlBuilder.BuildCustomerWL(ticket, isAllowedCustomerComm);
        }

        private static void BuildContributionTicket(TicketText ticketText)
        {
            ticketText.LicenseeWL = HyphenTextHTML;
            ticketText.WinLoss = HyphenTextHTML;
        }

        private string BuildOddsText(LrfTicket ticket, bool isMobileView)
        {
            return isMobileView
                   ? _betlistHtmlBuilder.BuildOddsValue(ticket, null, _ticketHelper.GetValidOddsTypeNameById)
                   : _betlistHtmlBuilder.BuildOdds(ticket, null, _ticketHelper.GetValidOddsTypeNameById);
        }

        private string BuildOddsTypeNameText(LrfTicket ticket, TicketResult ticketResult)
        {
            if (ticket.BetTypeId == (int)BetType.VirtualSports4MixParlay)
            {
                ticket.OddsType = string.Empty;
            }

            return _betlistHtmlBuilder.BuildOddsTypeName(ticket, ticketResult.TicketDataList, _ticketHelper.GetValidOddsTypeNameById);
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