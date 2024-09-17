namespace Fanex.Athena.BetList.DataAccess
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using Common.Constant;
    using Common.DataAccess.Interfaces;
    using Data;
    using Fanex.Athena.Common.Services.Interfaces;
    using Fanex.Athena.Common.Utility.Helpers;
    using Fanex.Common;
    using Interfaces;

    public class BettingTicketCenterDataAccess : BaseBetListDataAccess, IBettingTicketCenterDataAccess
    {
        private readonly IBalanceUpDataAccess _balanceUpDataAccess;
        private readonly IShardInfoAccessor _shardInfoAccessor;

        public BettingTicketCenterDataAccess(IBalanceUpDataAccess balanceUpDataAccess,
            ICurrencyDataAccess currencyService,
            IShardInfoAccessor shardInfoAccessor) : base(currencyService)
        {
            _balanceUpDataAccess = balanceUpDataAccess;
            _shardInfoAccessor = shardInfoAccessor;
        }

        public async Task<TicketResult> GetBetListForSingleCustomerAsync(
            SingleCustomersBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_Mem";

            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();
            using (var db = GetObjectDb(UspKey))
            {
                AddSingleCustomerParameters(db, metaModel);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    ticketResult.MainTicketList.AddRange(ReadBetListTicket(reader, currencyList));
                }

                var outSqlParams = db.GetOutputParams();
                ticketResult.SumStake = Converter.ToDecimal(outSqlParams[DbColumn.SumStake]);
                ticketResult.SumWinLoss = Converter.ToDecimal(outSqlParams[DbColumn.SumWinLost]);
                ticketResult.SumLicenseeWL = Converter.ToDecimal(outSqlParams[DbColumn.SumLicenseeWL]);
                ticketResult.SumComm = Converter.ToDecimal(outSqlParams[DbColumn.SumComm]);
                ticketResult.SumPlayerComm = Converter.ToDecimal(outSqlParams[DbColumn.SumPlayerComm]);
                ticketResult.TotalRows = Converter.ToInt(outSqlParams[DbColumn.TotalRows]);
            }

            return ticketResult;
        }

        public async Task<TicketResult> GetBetListByRefnoAsync(RefNoBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_RefNo_Mem";

            var ticketResult = new TicketResult();
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            metaModel.UseShardDB = _shardInfoAccessor.UseShardDB;

            if (long.TryParse(metaModel.RefNo, out long refno) || !string.IsNullOrEmpty(metaModel.TxId))
            {
                ticketResult = await GetMainTicketListBasedRangeAsync(metaModel, metaModel.ViewSabaCoins ? $"{UspKey}_SabaCoins" : UspKey,
                    assignCommonParamters: db =>
                    {
                        db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
                        db.SetParameterValue(DbColumn.UserName, SqlHelper.ToSqlTextSearch(metaModel.UserName));
                        db.SetParameterValue(DbColumn.RefNo, refno);
                        db.SetParameterValue(DbColumn.TxId, metaModel.TxId);
                        db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
                        db.SetParameterValue(DbColumn.MoveBetsDay, balanceUp.MoveBetsDay);
                        db.SetParameterValue(DbColumn.BalanceUpDay, balanceUp.BalanceUpDay);
                        db.SetParameterValue(DbColumn.ViewSabaCoins, metaModel.ViewSabaCoins);
                    }, getInOneRange: true);

                ticketResult.TotalRows = ticketResult.MainTicketList.Count;
            }

            return ticketResult;
        }

        public async Task<TicketResult> GetRunningBetListAsync(RunningBetListMetaModel metaModel)
        {
            const string UspKey = "Shard_Lrf_BetList_Running";

            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();

            using (var db = GetObjectDb(UspKey))
            {
                var productCodes = metaModel.ProductCodeList.Split(',');
                var productList = string.Join(",", productCodes.Except(new[] { ProductCodes.BA }));

                db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
                db.SetParameterValue(DbColumn.UserName, SqlHelper.ToSqlTextSearch(metaModel.UserName));
                db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
                db.SetParameterValue(DbColumn.ProductList, productList);
                db.SetParameterValue(DbColumn.OddsGroup, metaModel.OddsGroup);
                db.SetParameterValue(DbColumn.Platform, metaModel.Platform);
                db.SetParameterValue(DbColumn.CustomerClass, metaModel.CustomerClass);
                db.SetParameterValue(DbColumn.FullCustomerClass, metaModel.FullCustomerClass);
                db.SetParameterValue(DbColumn.SportTypeList, metaModel.SportListXML);
                db.SetParameterValue(DbColumn.BetTypeList, metaModel.BetTypeIds);
                db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveStatus);
                db.SetParameterValue(DbColumn.Ip, metaModel.Ip);
                db.SetParameterValue(DbColumn.LeagueList, metaModel.LeagueIds);
                db.SetParameterValue(DbColumn.MatchId, metaModel.MatchId);
                db.SetParameterValue(DbColumn.ShowBA, productCodes.Contains(ProductCodes.BA));
                db.SetParameterValue(DbColumn.PageSize, metaModel.IsExport ? int.MaxValue - 1 : metaModel.PageSize);
                db.SetParameterValue(DbColumn.PageIndex, metaModel.IsExport ? 1 : metaModel.PageIndex);
                db.SetParameterValue(DbColumn.StatusFilter, metaModel.Status);
                db.SetParameterValue(DbColumn.ConvertToBaseCur, metaModel.IsConvertAll);
                db.SetParameterValue(DbColumn.TotalRows, null);
                db.SetParameterValue(DbColumn.SumStake, null);
                AddSortParameter(db, metaModel.SortBy, metaModel.SortOrder);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    ticketResult.MainTicketList.AddRange(ReadBetListTicket(reader, currencyList));
                }

                var outSqlParams = db.GetOutputParams();
                ticketResult.SumStake = Converter.ToDecimal(outSqlParams[DbColumn.SumStake]);
                ticketResult.TotalRows = Converter.ToInt(outSqlParams[DbColumn.TotalRows]);
            }

            return ticketResult;
        }

        public async Task<TicketResult> GetBetListByTxIDAsync(TxIDBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_TxID_Mem";

            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();

            using (var db = GetObjectDb(UspKey))
            {
                var balanceUp = _balanceUpDataAccess.GetBalanceUp();
                db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
                db.SetParameterValue(DbColumn.TxId, metaModel.TxID);
                db.SetParameterValue(DbColumn.UserName, SqlHelper.ToSqlTextSearch(metaModel.UserName));
                db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
                db.SetParameterValue(DbColumn.MoveBetsDay, balanceUp.MoveBetsDay);
                db.SetParameterValue(DbColumn.BalanceUpDay, balanceUp.BalanceUpDay);
                db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    ticketResult.MainTicketList.AddRange(ReadBetListTicket(reader, currencyList));
                    ticketResult.TotalRows = ticketResult.MainTicketList.Count;
                }
            }

            return ticketResult;
        }

        public async Task<TicketResult> GetBetListForBookedBetAsync(BookedBetBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_BookedBets";
            const int UserNameOption = 3;

            var ticketResult = new TicketResult();
            var currencyList = await GetCurrencyListAsync();

            if (metaModel.SearchOption == UserNameOption || long.TryParse(metaModel.SearchText, out long _))
            {
                using (var db = GetObjectDb(UspKey))
                {
                    db.SetParameterValue(DbColumn.FromDate, metaModel.FromDate);
                    db.SetParameterValue(DbColumn.ToDate, metaModel.ToDate);
                    db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
                    db.SetParameterValue(DbColumn.SearchText, SqlHelper.ToSqlTextSearch(metaModel.SearchText));
                    db.SetParameterValue(DbColumn.SearchOption, metaModel.SearchOption);
                    db.SetParameterValue(DbColumn.StatusIdList, metaModel.Status);
                    db.SetParameterValue(DbColumn.Platform, metaModel.Platform);
                    db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
                    db.SetParameterValue(DbColumn.ConvertToBaseCur, metaModel.IsConvertAll);
                    db.SetParameterValue(DbColumn.PageSize, metaModel.IsExport ? int.MaxValue - 1 : metaModel.PageSize);
                    db.SetParameterValue(DbColumn.PageIndex, metaModel.IsExport ? 1 : metaModel.PageIndex);

                    db.SetParameterValue(DbColumn.TotalRows, null);
                    db.SetParameterValue(DbColumn.SumStake, null);
                    AddSortParameter(db, metaModel.SortBy, metaModel.SortOrder);

                    using (var reader = await db.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        ticketResult.MainTicketList.AddRange(ReadBetListTicket(reader, currencyList));
                    }

                    var outSqlParams = db.GetOutputParams();
                    ticketResult.SumStake = Converter.ToDecimal(outSqlParams[DbColumn.SumStake]);
                    ticketResult.TotalRows = Converter.ToInt(outSqlParams[DbColumn.TotalRows]);
                }
            }

            return ticketResult;
        }

        private static void AddSortParameter(IObjectDb db, string sortBy, string sortOrder)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                db.SetParameterValue(DbColumn.Sort, sortBy + " " + sortOrder);
            }
        }

        private void AddSingleCustomerParameters(IObjectDb db, SingleCustomersBetListMetaModel metaModel)
        {
            var productCodes = metaModel.ProductCodeList.Split(',');
            var productList = string.Join(",", productCodes.Except(new[] { ProductCodes.BA }));
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            db.SetParameterValue(DbColumn.ShowBA, productCodes.Contains(ProductCodes.BA));
            db.SetParameterValue(DbColumn.ProductList, productList);
            db.SetParameterValue(DbColumn.FromDate, metaModel.FromDate);
            db.SetParameterValue(DbColumn.ToDate, metaModel.ToDate);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.UserName, SqlHelper.ToSqlTextSearch(metaModel.UserName));
            db.SetParameterValue(DbColumn.StatusIdList, metaModel.Status);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveStatus);
            db.SetParameterValue(DbColumn.Platform, metaModel.Platform);
            db.SetParameterValue(DbColumn.SportTypeList, metaModel.SportListXML);
            db.SetParameterValue(DbColumn.BetTypeList, metaModel.BetTypeIds);
            db.SetParameterValue(DbColumn.CashOutMode, metaModel.CashOutMode);
            db.SetParameterValue(DbColumn.MoveBetsDay, balanceUp.MoveBetsDay);
            db.SetParameterValue(DbColumn.BalanceUpDay, balanceUp.BalanceUpDay);
            db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.PageSize, metaModel.IsExport ? int.MaxValue - 1 : metaModel.PageSize);
            db.SetParameterValue(DbColumn.PageIndex, metaModel.IsExport ? 1 : metaModel.PageIndex);

            db.SetParameterValue(DbColumn.TotalRows, null);
            db.SetParameterValue(DbColumn.SumStake, null);
            db.SetParameterValue(DbColumn.SumWinLost, null);
            db.SetParameterValue(DbColumn.SumLicenseeWL, null);
            db.SetParameterValue(DbColumn.SumComm, null);
            db.SetParameterValue(DbColumn.SumPlayerComm, null);
            AddSortParameter(db, metaModel.SortBy, metaModel.SortOrder);
        }
    }
}