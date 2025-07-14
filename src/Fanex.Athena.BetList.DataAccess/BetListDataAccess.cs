namespace Fanex.Athena.BetList.DataAccess
{
    using Common.Utility.Collections;
    using Fanex.Athena.BetList.DataAccess.Interfaces;
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.Common.Constant;
    using Fanex.Athena.Common.DataAccess.Interfaces;
    using Fanex.Common;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;

    public class BetListDataAccess : BaseBetListDataAccess, IBetListDataAccess
    {
        public BetListDataAccess(ICurrencyDataAccess currencyDataAccess) : base(currencyDataAccess)
        {
        }

        public async Task<TicketResult> GetMixParlayDetailTicket(MixParlayDetailMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_SelParlayDetail";
            var ticketResult = await GetMainTicketListBasedRangeAsync(metaModel, UspKey,
                assignCommonParamters: db =>
                {
                    db.SetParameterValue(nameof(metaModel.TransId), metaModel.TransId);
                    db.SetParameterValue(nameof(metaModel.CustId), metaModel.CustId);
                    db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
                    db.SetParameterValue(DbColumn.BalanceUpDay, metaModel.BalanceUpDay);
                });

            ticketResult.MainTicketList.Sort("TransId", false);
            return ticketResult;
        }

        public async Task<TicketResult> GetBetListData(BetListMetaModel metaModel)
        {
            var shardUspKey = $"SiteDB_{metaModel.UspKey}";
            var result = await GetBetListDataAsync(metaModel.UseShardDB
                && metaModel.InBettransRange
                && BetListParameterBuilder.IsExistedSpActionMap(shardUspKey)
                ? metaModel.ShallowCloneWith(_ => _.UspKey = shardUspKey) : metaModel);
            return result;
        }

        private async Task<TicketResult> GetBetListDataAsync(BetListMetaModel metaModel)
        {
            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();

            using (var db = GetObjectDb(metaModel.UspKey))
            {
                BetListParameterBuilder.CreateBetListParameters(db, metaModel);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    ticketResult.MainTicketList.AddRange(ReadBetListTicket(reader,
                       currencyList, metaModel.CustId));
                    if (reader.NextResult())
                    {
                        ticketResult.TicketDataList.AddRange(ReadReferneceTicket(reader).ToList());
                    }
                    SortData(metaModel, ticketResult.MainTicketList);
                }

                var outSqlParams = db.GetOutputParams() ?? new Hashtable();

                UpdateSummaryData(ticketResult, outSqlParams);

                return ticketResult;
            }
        }

        private static void UpdateSummaryData(TicketResult ticketResult, Hashtable outPutParams)
        {
            ticketResult.TotalRows = Converter.ToInt(outPutParams["totalrows"]);
            ticketResult.SumWinLoss = outPutParams.ContainsKey("sumwinlost")
                ? Converter.ToDecimal(outPutParams["sumwinlost"])
                : ticketResult.MainTicketList.Sum(x => x.Winlost);
            ticketResult.SumBetEffective = outPutParams.ContainsKey("sumbeteffective")
                ? Converter.ToDecimal(outPutParams["sumbeteffective"])
                : ticketResult.MainTicketList.Sum(x => x.BetEffective);
            ticketResult.SumLicenseeWL = outPutParams.ContainsKey("sumlicenseewl")
                ? Converter.ToDecimal(outPutParams["sumlicenseewl"])
                : ticketResult.MainTicketList.Sum(x => x.LicenseeWL);
            ticketResult.SumComm = outPutParams.ContainsKey("sumcomm")
                ? Converter.ToDecimal(outPutParams["sumcomm"])
                : ticketResult.MainTicketList.Sum(x => x.Comm);
            ticketResult.SumPlayerComm = outPutParams.ContainsKey("sumplayercomm")
                ? Converter.ToDecimal(outPutParams["sumplayercomm"])
                : ticketResult.MainTicketList.Sum(x => x.PlayerComm);
        }

        private static void SortData(BetListMetaModel metaModel, SortableList<LrfTicket> ticketList)
        {
            if (!metaModel.IsServerPaging)
            {
                var sortBy = metaModel.SortType switch
                {
                    (byte)SortTypes.TransDate => "TransDate",
                    (byte)SortTypes.Stake => "Stake",
                    (byte)SortTypes.WinLoss => "WinLossComm",
                    (byte)SortTypes.LicenseeWL => "LicenseeWLComm",
                    _ => "TransDate"
                };

                ticketList.Sort(sortBy, metaModel.SortOrder == (byte)Common.Constant.SortOrders.Ascending);
            }
        }
    }
}