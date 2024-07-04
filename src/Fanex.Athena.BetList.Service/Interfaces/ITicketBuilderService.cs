namespace Fanex.Athena.BetList.Service.Interfaces
{
    using System.Collections.Generic;

    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;

    public interface ITicketBuilderService
    {
        IList<TicketText> BuildBookedBetTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel);

        IList<TicketText> BuildTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel, bool isAllowedCustomerComm);

#pragma warning disable S107 // Methods should not have too many parameters

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

#pragma warning restore S107 // Methods should not have too many parameters

        string BuildSumWinLoss(TicketResult ticketResult, bool isAllowedCustomerComm);
    }
}