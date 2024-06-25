using Fanex.Athena.BetList.DataAccess.Interfaces;
using Fanex.Athena.BetList.Model.DTO;
using Fanex.Athena.BetList.Model.MetaModel;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Common;
using Fanex.Data;

namespace Fanex.Athena.BetList.DataAccess
{
    public class PlayerSegmentationBetListDataAccess : BaseBetListDataAccess, IPlayerSegmentationBetListDataAccess
    {
        private const string AbnormalStatus = "99";
        private readonly IBalanceUpDataAccess _balanceUpDataAccess;

        public PlayerSegmentationBetListDataAccess(
            IBalanceUpDataAccess balanceUpDataAccess,
            ICurrencyDataAccess currencyService) : base(currencyService)
        {
            _balanceUpDataAccess = balanceUpDataAccess;
        }

        public async Task<TicketResult> GetAbnormalBetListAsync(PlayerSegmentationBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_Mem";
            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();
            using (var db = GetObjectDb(UspKey))
            {
                AddAbnormalBetListParameters(db, metaModel);
                using (var reader = await db.ExecuteReaderAsync())
                {
                    ticketResult.MainTicketList.AddRange(ReadBetListTicket(reader, currencyList));
                }

                var outSqlParams = db.GetOutputParams();
                ticketResult.TotalRows = Converter.ToInt(outSqlParams["totalrows"]);
            }

            return ticketResult;
        }

        private void AddAbnormalBetListParameters(IObjectDb db, PlayerSegmentationBetListMetaModel metaModel)
        {
            var isShowBA = metaModel.ListProductCode.Contains(ProductCodes.BA);
            var productList = string.Join(",", metaModel.ListProductCode.Except(new[] { ProductCodes.BA }));
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();

            db.SetParameterValue("ShowBA", isShowBA);
            db.SetParameterValue("ProductList", productList);
            db.SetParameterValue("CustId", metaModel.CustId);
            db.SetParameterValue("FromDate", metaModel.FromDate);
            db.SetParameterValue("ToDate", metaModel.ToDate);
            db.SetParameterValue("AgentList", metaModel.AgentListXml);
            db.SetParameterValue("StatusIdList", AbnormalStatus);
            db.SetParameterValue("MoveBetsDay", balanceUp.MoveBetsDay);
            db.SetParameterValue("BalanceUpDay", balanceUp.BalanceUpDay);
            db.SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            db.SetParameterValue("PageSize", metaModel.PageSize);
            db.SetParameterValue("PageIndex", metaModel.PageIndex);
        }
    }
}