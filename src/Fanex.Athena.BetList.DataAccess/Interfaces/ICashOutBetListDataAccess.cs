namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using BetList.Model.ViewModel;
    using Fanex.Athena.BetList.Model.MetaModel;
    using System.Threading.Tasks;

    public interface ICashOutBetListDataAccess
    {
        Task<CashOutBetListDetailViewModel> GetDetail(CashOutBetListDetailMetaModel metaModel);

        Task<CashOutHistoryDetailViewModel> GetHistoryDetail(CashOutHistoryDetailMetaModel metaModel);
    }
}