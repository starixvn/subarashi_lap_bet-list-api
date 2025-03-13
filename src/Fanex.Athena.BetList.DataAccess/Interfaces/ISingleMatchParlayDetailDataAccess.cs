namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISingleMatchParlayDetailDataAccess
    {
        Task<IList<SingleMatchParlayDetail>> GetSingleMatchParlayDetail(SingleMatchParlayDetailMetaModel metaModel);
    }
}
