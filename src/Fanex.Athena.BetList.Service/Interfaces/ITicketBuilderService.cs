namespace Fanex.Athena.BetList.Service.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;
    using Optiions;

    public interface ITicketBuilderService
    {
        IList<TicketText> BuildBookedBetTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel);

        IList<TicketText> BuildTicketText(TicketResult ticketResult, ICommonValueMetaModel metaModel, bool isAllowedCustomerComm);

        List<TicketText> BuildTicketText(
            TicketResult ticketResult,
            BetListMetaModel metaModel = null,
            int startIndex = 1,
            BuildTicketOption buildTicketOption = default);

        string BuildSumWinLoss(TicketResult ticketResult, bool isAllowedCustomerComm);
    }
}