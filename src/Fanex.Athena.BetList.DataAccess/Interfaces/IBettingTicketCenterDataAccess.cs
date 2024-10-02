namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using System.Threading.Tasks;

    public interface IBettingTicketCenterDataAccess
    {
        Task<TicketResult> GetBetListForSingleCustomerAsync(SingleCustomersBetListMetaModel metaModel);

        Task<TicketResult> GetBetListByRefnoAsync(RefNoBetListMetaModel metaModel);

        Task<TicketResult> GetRunningBetListAsync(RunningBetListMetaModel metaModel);

        Task<TicketResult> GetBetListByTxIDAsync(TxIDBetListMetaModel metaModel);

        Task<TicketResult> GetBetListForBookedBetAsync(BookedBetBetListMetaModel metaModel);
    }
}