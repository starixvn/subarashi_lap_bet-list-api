namespace Fanex.Athena.BetList.DataAccess
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading.Tasks;
    using BetList.Model.DTO;
    using BetList.Model.MetaModel;
    using Common.DataAccess.Interfaces;
    using Common.Utility.Services.Interfaces;
    using Data;
    using Fanex.Athena.Common.Constant;
    using Fanex.Athena.Common.Utility.Helpers;
    using Fanex.Common;
    using Interfaces;
    using Newtonsoft.Json;

    public class BettingTicketCenterAdvancedSearchDataAccess : BaseBetListDataAccess, IBettingTicketCenterAdvancedSearchDataAccess
    {
        private readonly IBalanceUpDataAccess _balanceUpDataAccess;
        private readonly ITempFileService _tempFileService;

        public BettingTicketCenterAdvancedSearchDataAccess(IBalanceUpDataAccess balanceUpDataAccess,
            ITempFileService tempFileService, ICurrencyDataAccess currencyService) : base(currencyService)
        {
            _balanceUpDataAccess = balanceUpDataAccess;
            _tempFileService = tempFileService;
        }

        public async Task<GroupTicketResult> GetBetListForMultipleCustomersAsync(
            AdvancedSearchBetListMetaModel metaModel,
            int rowPerGroup)
        {
            const string UspKey = "Lrf_BetList_Licensee";
            var groupData = new GroupTicketResult
            {
                RowPerGroup = rowPerGroup
            };
            var currencyList = await this.GetCurrencyListAsync();

            using (var db = GetObjectDb(UspKey))
            {
                AddAdvancedSearchParameters(db, metaModel);

                using (var reader = await db.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var count = 0;
                    var ticket = new TicketResult();

                    var columns = GetColumns();
                    var safeReader = this.GetSafeDataReader(reader, columns.Select(x => x.Value).ToList());

                    while (reader.Read())
                    {
                        if (count == rowPerGroup)
                        {
                            SaveDataToFile(groupData, ticket);
                            count = 0;
                            ticket = new TicketResult();
                        }

                        ticket.MainTicketList.Add(MapTicket(safeReader, columns, currencyList));
                        count++;
                    }

                    if (ticket.MainTicketList.Any())
                    {
                        SaveDataToFile(groupData, ticket);
                    }
                }

                var outSqlParams = db.GetOutputParams();
                ReadOutputParams(groupData, outSqlParams);

                return groupData;
            }
        }

        public async Task<TicketResult> GetBetListForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_Licensee";

            var ticketResult = new TicketResult();
            var currencyList = await this.GetCurrencyListAsync();

            using (var db = GetObjectDb(UspKey))
            {
                AddAdvancedSearchParameters(db, metaModel);

                using (var reader = await db.ExecuteReaderAsync().ConfigureAwait(false))
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

        public async Task<int> GetTotalRowForMultipleCustomersAsync(AdvancedSearchBetListMetaModel metaModel)
        {
            const string UspKey = "Lrf_BetList_Licensee_TotalRows";

            using (var db = GetObjectDb(UspKey))
            {
                var balanceUp = _balanceUpDataAccess.GetBalanceUp();
                db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
                db.SetParameterValue(DbColumn.FromDate, metaModel.FromDate);
                db.SetParameterValue(DbColumn.ToDate, metaModel.ToDate);
                db.SetParameterValue(DbColumn.MoveBetsDay, balanceUp.MoveBetsDay);
                db.SetParameterValue(DbColumn.SportType, metaModel.SportId);
                db.SetParameterValue(DbColumn.BetType, metaModel.BetTypeId);
                db.SetParameterValue(DbColumn.LeagueId, metaModel.LeagueId);
                db.SetParameterValue(DbColumn.MatchId, metaModel.MatchId);
                db.SetParameterValue(DbColumn.StatusIdList, metaModel.Status);
                db.SetParameterValue(DbColumn.UserName, SqlHelper.ToSqlTextSearch(metaModel.UserName));
                db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveStatus);
                db.SetParameterValue(DbColumn.Product, metaModel.ProductCodeList);
                db.SetParameterValue(DbColumn.OddsGroup, metaModel.OddsGroup);
                db.SetParameterValue(DbColumn.PlatformType, metaModel.Platform);
                db.SetParameterValue(DbColumn.CustomerClass, metaModel.CustomerClass);
                db.SetParameterValue(DbColumn.FullCustomerClass, metaModel.FullCustomerClass);
                db.SetParameterValue(DbColumn.LeagueGroupIds, metaModel.LeagueGroupIds);

                db.SetParameterValue(DbColumn.TotalRows, null);

                await db.ExecuteNonQueryAsync().ConfigureAwait(false);

                var outSqlParams = db.GetOutputParams();
                return Converter.ToInt(outSqlParams[DbColumn.TotalRows]);
            }
        }

        private static void ReadOutputParams(GroupTicketResult groupData, Hashtable outSqlParams)
        {
            groupData.SumStake = Converter.ToDecimal(outSqlParams[DbColumn.SumStake]);
            groupData.SumWinLoss = Converter.ToDecimal(outSqlParams[DbColumn.SumWinLost]);
            groupData.SumLicenseeWL = Converter.ToDecimal(outSqlParams[DbColumn.SumLicenseeWL]);
            groupData.SumComm = Converter.ToDecimal(outSqlParams[DbColumn.SumComm]);
            groupData.SumPlayerComm = Converter.ToDecimal(outSqlParams[DbColumn.SumPlayerComm]);
            groupData.TotalRows = Converter.ToInt(outSqlParams[DbColumn.TotalRows]);
        }

        private static void AddSortParameter(IObjectDb db, string sortBy, string sortOrder)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                db.SetParameterValue(DbColumn.Sort, sortBy + " " + sortOrder);
            }
        }

        private void SaveDataToFile(GroupTicketResult groupData, TicketResult ticket)
        {
            var data = JsonConvert.SerializeObject(ticket);
            var dataFileName = $"{Guid.NewGuid().ToString()}.json";
            _tempFileService.Save(dataFileName, data);
            groupData.DataPath.Add(dataFileName);
            ticket.MainTicketList.Clear();
            ticket.MainTicketList.TrimExcess();
        }

        private void AddAdvancedSearchParameters(IObjectDb db, AdvancedSearchBetListMetaModel metaModel)
        {
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.FromDate, metaModel.FromDate);
            db.SetParameterValue(DbColumn.ToDate, metaModel.ToDate);
            db.SetParameterValue(DbColumn.MoveBetsDay, balanceUp.MoveBetsDay);
            db.SetParameterValue(DbColumn.SportType, metaModel.SportId);
            db.SetParameterValue(DbColumn.BetType, metaModel.BetTypeId);
            db.SetParameterValue(DbColumn.LeagueId, metaModel.LeagueId);
            db.SetParameterValue(DbColumn.MatchId, metaModel.MatchId);
            db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.StatusIdList, metaModel.Status);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveStatus);
            db.SetParameterValue(DbColumn.UserName, SqlHelper.ToSqlTextSearch(metaModel.UserName));
            db.SetParameterValue(DbColumn.PageSize, metaModel.IsExport ? int.MaxValue - 1 : metaModel.PageSize);
            db.SetParameterValue(DbColumn.PageIndex, metaModel.IsExport ? 1 : metaModel.PageIndex);
            db.SetParameterValue(DbColumn.Product, metaModel.ProductCodeList);
            db.SetParameterValue(DbColumn.ConvertToBaseCur, metaModel.IsConvertAll);
            db.SetParameterValue(DbColumn.Ip, metaModel.Ip);
            db.SetParameterValue(DbColumn.PlatformType, metaModel.Platform);
            db.SetParameterValue(DbColumn.CustomerClass, metaModel.CustomerClass);
            db.SetParameterValue(DbColumn.FullCustomerClass, metaModel.FullCustomerClass);
            db.SetParameterValue(DbColumn.LeagueGroupIds, metaModel.LeagueGroupIds);
            db.SetParameterValue(DbColumn.OddsGroup, metaModel.OddsGroup);

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