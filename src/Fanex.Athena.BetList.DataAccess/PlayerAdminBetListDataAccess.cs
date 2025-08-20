namespace Fanex.Athena.BetList.DataAccess
{
    using Common.DataAccess.Interfaces;
    using Data;
    using Fanex.Common;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;

    public class PlayerAdminBetListDataAccess : BaseBetListDataAccess, IPlayerAdminBetListDataAccess
    {
        public PlayerAdminBetListDataAccess(ICurrencyDataAccess currencyService) : base(currencyService)
        {
        }

        public async Task<TicketResult> GetRunningBetListAsync(PlayerAdminBetListMetaModel metaModel)
        {
            const string UspKey = "Shard_Lrf_BetList_Running_Mem";

            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();

            using (var db = GetObjectDb(UspKey))
            {
                SetParameter(db, metaModel);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    ticketResult.MainTicketList = ReadBetListTicket(reader, currencyList);

                    if (reader.NextResult())
                    {
                        ticketResult.TicketDataList.AddRange(ReadReferneceTicket(reader));
                    }
                }

                var outSqlParams = db.GetOutputParams();
                ticketResult.SumStake = Converter.ToDecimal(outSqlParams["SumStake"]);
                ticketResult.TotalRows = Converter.ToInt(outSqlParams["TotalRows"]);
            }

            return ticketResult;
        }

        private static void SetParameter(IObjectDb db, PlayerAdminBetListMetaModel metaModel)
        {
            db.SetParameterValue("AgentList", metaModel.AgentListXml);
            db.SetParameterValue("CustId", metaModel.CustId);
            db.SetParameterValue("PageIndex", metaModel.PageIndex);
            db.SetParameterValue("PageSize", metaModel.PageSize);
            db.SetParameterValue("SumStake", 0);
            db.SetParameterValue("TotalRows", 0);
            db.SetParameterValue("SortType", metaModel.SortType);
            db.SetParameterValue("SortOrder", metaModel.SortOrder);
        }
    }
}