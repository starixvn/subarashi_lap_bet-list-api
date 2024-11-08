namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using System.Collections.Generic;

    public interface ITicketBuilderService
    {
        IList<TicketText> BuildBookedBetTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel);

        IList<TicketText> BuildTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel, bool isAllowedCustomerComm);


        List<TicketText> BuildTicketText(
            TicketResult ticketResult,
            BetListMetaModel metaModel = null,
            int startIndex = 1,
            bool showOdds = false,
            bool isAllowedCustomerComm = false,
            bool isMobileView = false,
            bool isShowIpvn = false,
            bool isShowIp = true,
            bool isHideSaba = false);


        string BuildSumWinLoss(TicketResult ticketResult, bool isAllowedCustomerComm);
    }
}