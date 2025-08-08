namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.MetaModel;
    using Model.ViewModel;

    public interface ICashOutBetListDataAccess
    {
        Task<CashOutBetListDetailViewModel> GetDetail(CashOutBetListDetailMetaModel metaModel);

        Task<CashOutHistoryDetailViewModel> GetHistoryDetail(CashOutHistoryDetailMetaModel metaModel);
    }
}