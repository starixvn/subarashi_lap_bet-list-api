namespace Fanex.Athena.BetList.DataAccess
{
    using BetList.Model.DTO;
    using BetList.Model.ViewModel;
    using Common.Utility.Collections;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.Common.Constant;
    using Fanex.Athena.Common.DataAccess.Interfaces;
    using Fanex.Athena.Common.Services.Interfaces;
    using Fanex.Common.Extensions;
    using Interfaces;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    public class CashOutBetListDataAccess : BaseBetListDataAccess, ICashOutBetListDataAccess
    {
        private readonly IBalanceUpDataAccess _balanceUpDataAccess;
        private readonly IShardInfoAccessor _shardInfoAccessor;

        public CashOutBetListDataAccess(ICurrencyDataAccess currencyService,
            IShardInfoAccessor shardInfoAccessor,
            IBalanceUpDataAccess balanceUpDataAccess) : base(currencyService)
        {
            _balanceUpDataAccess = balanceUpDataAccess;
            _shardInfoAccessor = shardInfoAccessor;
        }

        public async Task<CashOutBetListDetailViewModel> GetDetail(
            CashOutBetListDetailMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_CashOut_Detail";

            var balanceUpData = _balanceUpDataAccess.GetBalanceUp();
            var currencyList = await GetCurrencyListAsync();

            using (var db = GetObjectDb(UspKey))
            {
                var param = new
                {
                    metaModel.TransId,
                    metaModel.RefNo,
                    metaModel.BetType,
                    OriginalTransId = metaModel.BetdaqId,
                    AgentList = metaModel.AgentListXml,
                    metaModel.BaseCurrency,
                    balanceUpData.MoveBetsDay,
                    balanceUpData.BalanceUpDay
                };
                using (var reader = await db.ExecuteReaderAsync(param))
                {
                    var viewModel = new CashOutBetListDetailViewModel
                    {
                        OriginalTicket = ReadBetListTicket(reader, currencyList).GetFirstOrDefault()
                    };

                    if (reader.NextResult())
                    {
                        viewModel.CashOutTicketList = ReadCashOutTickets(reader);
                    }

                    return viewModel;
                }
            }
        }

        public async Task<CashOutHistoryDetailViewModel> GetHistoryDetail(
            CashOutHistoryDetailMetaModel metaModel)
        {
            var balanceUpData = _balanceUpDataAccess.GetBalanceUp();
            var useSiteDB = _shardInfoAccessor.UseShardDB && metaModel.Date > balanceUpData.MoveBetsDay;
            using (var db = GetObjectDb(useSiteDB ? "SiteDB_Lrf_BetList_CashOut_History" : "Lrf_BetList_CashOut_History"))
            {
                db.SetParameterValue(DbColumn.TransId, metaModel.TransId);
                db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
                db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
                db.SetParameterValue("WinLostDate", metaModel.Date);
                db.SetParameterValue("Currency", metaModel.CurrencyId);
                if (!useSiteDB)
                {
                    db.SetParameterValue(DbColumn.BalanceUpDay, balanceUpData.BalanceUpDay);
                }

                using (var reader = await db.ExecuteReaderAsync())
                {
                    var result = new CashOutHistoryDetailViewModel
                    {
                        TransId = metaModel.TransId,
                        Currency = metaModel.Currency,
                        BaseCurrency = metaModel.BaseCurrency
                    };
                    if (reader.Read())
                    {
                        result = ReadCashOutHistoryTicket(reader, result);
                    }
                    return result;
                }
            }
        }

        private static IEnumerable<CashOutTicket> ReadCashOutTickets(IDataReader reader)
        {
            var result = new SortableList<CashOutTicket>();

            while (reader.Read())
            {
                var item = new CashOutTicket
                {
                    BetType = reader.ToInt("bettype"),
                    TransId = reader.ToLong("transid"),
                    RefNo = reader.ToString("refno"),
                    Status = reader.ToString("status"),
                    Stake = reader.ToDecimal("stake"),
                    WinLoss = reader.ToDecimal("winlost"),
                    Price = reader.ToDecimal("price"),
                    WinLostDate = reader.ToDateTime("winlostdate"),
                    TransDate = reader.ToDateTime("transdate"),
                    Currency = reader.ToInt("currency"),
                    CustId = reader.ToInt("custid")
                };

                result.Add(item);
            }

            return result;
        }

        private static CashOutHistoryDetailViewModel ReadCashOutHistoryTicket(IDataReader reader, CashOutHistoryDetailViewModel result)
        {
            result.TransId = reader.ToLong("TransId");
            result.CashoutExtraInfo = reader.ToString("CashoutExtraInfo");
            result.Rate = reader.ToDecimal("Rate");

            return result;
        }
    }
}