namespace Fanex.Athena.BetList.DataAccess
{
    using Common.DataAccess;
    using Fanex.Common.Extensions;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;

    public class SingleMatchParlayDetailDataAccess : BaseDataAccess, ISingleMatchParlayDetailDataAccess
    {
        public async Task<IList<SingleMatchParlayDetail>> GetSingleMatchParlayDetail(SingleMatchParlayDetailMetaModel metaModel)
        {
            const string UspName = "Lrf_BetList_BetBuilder_Detail";
            var result = new List<SingleMatchParlayDetail>();

            using (var db = GetObjectDb(UspName))
            {
                db.SetParameterValue("TransId", metaModel.TransId);
                db.SetParameterValue("CustId", metaModel.CustId);
                using (var reader = await db.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        result.Add(new SingleMatchParlayDetail
                        {
                            SelectionType = reader.ToInt("SelectionType"),
                            Result = reader.ToString("Result")
                        });
                    }
                }
            }

            return result;
        }
    }
}
