namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface IBettingTicketCenterDataAccess
    {
        Task<TicketResult> GetBetListForSingleCustomerAsync(SingleCustomersBetListMetaModel metaModel);

        Task<TicketResult> GetBetListByRefnoAsync(RefNoBetListMetaModel metaModel);

        Task<TicketResult> GetRunningBetListAsync(RunningBetListMetaModel metaModel);

        Task<TicketResult> GetBetListByTxIDAsync(TxIDBetListMetaModel metaModel);

        Task<TicketResult> GetBetListForBookedBetAsync(BookedBetBetListMetaModel metaModel);
    }
}