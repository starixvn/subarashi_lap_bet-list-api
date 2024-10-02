// -----------------------------------------------------------------------
// <copyright file="ILastBetsMonitoringBetListDataAccess.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Athena.BetList.DataAccess.Interfaces
{
    using Fanex.Athena.BetList.Model.DTO;
    using Fanex.Athena.BetList.Model.MetaModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILastBetsMonitoringBetListDataAccess
    {
        Task<TicketResult> GetLastBetsMonitoringData(LastBetsMonitoringMetaModel metaModel);

        Task<IList<BetListMatch>> GetMatchList(string uspKey);

        Task<BetListMatch> GetMatchById(int matchId);
    }
}