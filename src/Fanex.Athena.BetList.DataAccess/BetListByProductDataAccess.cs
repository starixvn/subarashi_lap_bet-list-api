namespace Fanex.Athena.BetList.DataAccess
{
    using System.Collections;
    using Common.DataAccess.Interfaces;
    using Data;
    using Fanex.Common;
    using Interfaces;
    using Model.DTO;
    using Model.MetaModel;

    public class BetListByProductDataAccess : BaseBetListDataAccess, IBetListByProductDataAccess
    {
        private readonly IBalanceUpDataAccess _balanceUpDataAccess;

        public BetListByProductDataAccess(IBalanceUpDataAccess balanceUpDataAccess,
            ICurrencyDataAccess currenyDataAccess) : base(currenyDataAccess)
        {
            _balanceUpDataAccess = balanceUpDataAccess;
        }

        public async Task<TicketResult> QueryAsync(BetListByProductMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_Statement_Mem";

            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();
            using (var db = GetObjectDb(UspKey))
            {
                SetParam(metaModel, db);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    ticketResult.MainTicketList.AddRange(ReadBetListTicket(reader,
                        currencyList));
                }

                var outSqlParams = db.GetOutputParams();
                GetOutPutResult(ticketResult, outSqlParams);
            }

            return ticketResult;
        }

        private static void GetOutPutResult(TicketResult ticketResult, Hashtable outSqlParams)
        {
            ticketResult.TotalRows = Converter.ToInt(outSqlParams["totalrows"]);
            ticketResult.SumStake = Converter.ToDecimal(outSqlParams["SumStake"]);
            ticketResult.SumWinLoss = Converter.ToDecimal(outSqlParams["SumWinLost"]);
            ticketResult.SumLicenseeWL = Converter.ToDecimal(outSqlParams["SumLicenseeWL"]);
            ticketResult.SumComm = Converter.ToDecimal(outSqlParams["SumComm"]);
            ticketResult.SumPlayerComm = Converter.ToDecimal(outSqlParams["SumPlayerComm"]);
        }

        private void SetParam(BetListByProductMetaModel metaModel, IObjectDb db)
        {
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            db.SetParameterValue("@AgentList", metaModel.AgentListXml);
            db.SetParameterValue("CustId", metaModel.CustId);
            db.SetParameterValue("@Date", metaModel.WinlostDate.Date);
            db.SetParameterValue("@MoveBetsDay", balanceUp.MoveBetsDay);
            db.SetParameterValue("@BalanceUpDay", balanceUp.BalanceUpDay);
            db.SetParameterValue("@ProductCode", metaModel.ProductCode);
            db.SetParameterValue("@TransType", metaModel.TransType);
            db.SetParameterValue("BaseCurrency", metaModel.BaseCurrency);
            db.SetParameterValue("PageSize", metaModel.PageSize);
            db.SetParameterValue("PageIndex", metaModel.PageIndex);
            db.SetParameterValue("@Sort", metaModel.Sort);
            db.SetParameterValue("@ViewSabaCoins", --metaModel.ViewType);
            db.SetParameterValue("@TotalRows", null);
            db.SetParameterValue("@SumStake", null);
            db.SetParameterValue("@SumWinLost", null);
            db.SetParameterValue("@SumLicenseeWL", null);
            db.SetParameterValue("@SumComm", null);
            db.SetParameterValue("@SumPlayerComm", null);
        }
    }
}