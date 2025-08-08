namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Model.DTO;
    using Model.MetaModel;

    public interface ISingleMatchParlayDetailDataAccess
    {
        Task<IList<SingleMatchParlayDetail>> GetSingleMatchParlayDetail(SingleMatchParlayDetailMetaModel metaModel);
    }
}
