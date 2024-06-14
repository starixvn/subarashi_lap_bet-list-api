// -----------------------------------------------------------------------
// <copyright file="LastBetsMonitoringBetListDataAccess.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Athena.BetList.DataAccess
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using Fanex.Athena.Common.DataAccess.Interfaces;
    using Interfaces;

    public class LastBetsMonitoringBetListDataAccess : BaseBetListDataAccess, ILastBetsMonitoringBetListDataAccess
    {
        public LastBetsMonitoringBetListDataAccess(
        ICurrencyDataAccess currencyService) : base(currencyService)
        {
        }

        public async Task<TicketResult> GetLastBetsMonitoringData(LastBetsMonitoringMetaModel metaModel)
        {
            const string UspKey = "Shard_Lrf_BetList_LastBetsMonitoring";

            var ticketResult = new TicketResult();
            var currencyList = await this.GetCurrencyListAsync();

            using (var db = this.GetObjectDb(UspKey))
            {
                db.SetParameterValue("AgentList", metaModel.AgentListXml);
                db.SetParameterValue("BetType", metaModel.BetType);
                db.SetParameterValue("Currency", metaModel.Currency);
                db.SetParameterValue("SpecialLevel", metaModel.Level);
                db.SetParameterValue("TransId", metaModel.TransId);
                db.SetParameterValue("LimitRowCount", metaModel.PageSize);
                db.SetParameterValue("SportTypeList", metaModel.SportTypeList);
                db.SetParameterValue("LeagueList", metaModel.LeagueList);
                db.SetParameterValue("MatchId", metaModel.MatchId);
                db.SetParameterValue("MinStake", metaModel.StakeFrom);
                db.SetParameterValue("MaxStake", metaModel.StakeTo);
                db.SetParameterValue("LiveIndicator", metaModel.LiveStatus);
                db.SetParameterValue("BaseCurrency", metaModel.BasedCurrency);
                db.SetParameterValue("BetFrom", metaModel.BetFrom);
                db.SetParameterValue("TransLogId", metaModel.TransLogId);
                db.SetParameterValue("ProductList", metaModel.ProductCodeList);
                db.SetParameterValue("CustomerClass", metaModel.CustomerClass);
                db.SetParameterValue("FullCustomerClass", string.Join(",", metaModel.RiskLevels.Select(_ => string.Join(",", _.CustomerClassList))));
                using (var reader = await db.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var mainTicket = ReadBetListTicket(reader, currencyList);
                    mainTicket.Sort("TransId", false);
                    ticketResult.MainTicketList.AddRange(mainTicket);
                }

                return ticketResult;
            }
        }

        public async Task<IList<BetListMatch>> GetMatchList(string uspKey)
        {
            using (var db = this.GetObjectDb(uspKey))
            {
                return (await db.QueryAsync<BetListMatch>().ConfigureAwait(false)).ToList();
            }
        }

        public async Task<BetListMatch> GetMatchById(int matchId)
        {
            const string UspName = "Lrf_Monitor_MatchSelById";

            using (var db = this.GetObjectDb(UspName))
            {
                db.SetParameterValue("MatchId", matchId);

                return (await db.QueryEntityAsync<BetListMatch>().ConfigureAwait(false));
            }
        }
    }
}