using Fanex.Athena.BetList.Model.Constants;
using Fanex.Athena.BetList.Service.Interfaces;
using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;
using System.Text;

namespace Fanex.Athena.BetList.Service
{
    public class BettingTicketCenterSportService : IBettingTicketCenterSportService
    {
        private readonly ICacheRefAdapter _cacheRefAdapter;

        public static readonly IEnumerable<(int Id, int SportTypeId, IEnumerable<int> LeagueGroupIds)> SabaVirtualSports = new List<(int, int, IEnumerable<int>)>
        {
            ((int)BettingTicketCenterSportType.SabaSoccer, (int)SportType.Soccer, new [] { 42, 74 }),
            ((int)BettingTicketCenterSportType.SabaBasketball, (int)SportType.BasketBall, new [] { 42 }),
            ((int)BettingTicketCenterSportType.SabaCricket, (int)SportType.Cricket, new [] { 42 }),
            ((int)BettingTicketCenterSportType.SabaFutsal, (int)SportType.Soccer, new [] { 108 }),
            ((int)BettingTicketCenterSportType.SabaTennis, (int)SportType.Tennis, new [] { 42 }),
            ((int)BettingTicketCenterSportType.SabaIceHockey, (int)SportType.IceHockey, new [] { 42 }),
            ((int)BettingTicketCenterSportType.SabaKabaddi, (int)SportType.Kabaddi, new [] { 42 }),
            ((int)BettingTicketCenterSportType.SabaSoccerPingoal, (int)SportType.Soccer, new [] { 113 }),
            ((int)BettingTicketCenterSportType.SabaBasketballPinGoal, (int)SportType.BasketBall, new [] { 122 }),
            ((int)BettingTicketCenterSportType.SabaESportsPinGoal, (int)SportType.Esports, new [] { 151 })
        };

        public BettingTicketCenterSportService(ICacheRefAdapter cacheRefAdapter)
        {
            _cacheRefAdapter = cacheRefAdapter;
        }

        public string BuildSportTypeXML(string sportIds)
        {
            const int Parlay = -1;

            if (string.IsNullOrEmpty(sportIds) || string.Equals(sportIds, "All", StringComparison.OrdinalIgnoreCase))
            {
                return sportIds;
            }

            var result = new StringBuilder("<ROOT>");

            foreach (var sport in sportIds.Split(','))
            {
                Int32.TryParse(sport, out int sportId);
                var leagueGroupIds = "0";

                if (sportId == Parlay)
                {
                    sportId = (int)SportType.Parlay;
                }

                if (SabaVirtualSports.Any(_ => _.Id == sportId))
                {
                    var (Id, SportTypeId, LeagueGroupIds) = SabaVirtualSports.FirstOrDefault(_ => _.Id == sportId);
                    leagueGroupIds = string.Join(",", LeagueGroupIds);
                    sportId = SportTypeId;
                }

                result.AppendFormat("<Sport SportType=\"{0}\" LeagueGroupIds=\"{1}\"/>", sportId, leagueGroupIds);
            }

            result.Append("</ROOT>");
            return result.ToString();
        }

        public IEnumerable<SabaSport> GetSabaSports(bool isHideSaba = false)
            => SabaVirtualSports.Select(t => new SabaSport
            {
                Id = t.Id,
                LeagueGroupIds = t.LeagueGroupIds,
                Name = GetSabaName(t.Id, t.SportTypeId, isHideSaba),
                SportTypeId = t.SportTypeId
            });

        public SabaSport GetSabaSport(int id, bool isHideSaba = false)
            => GetSabaSports(isHideSaba).FirstOrDefault(t => t.Id == id);

        private string GetSabaName(int id, int sportTypeId, bool isHideSaba = false)
        {
            string sportName;
            var prefixText = isHideSaba ? ViewResources.Virtual : ViewResources.Saba;
            switch (id)
            {
                case (int)SportType.SabaSoccerPingoal:
                    sportName = ViewResources.SoccerPinGoal;
                    break;

                case (int)SportType.SabaBasketballPinGoal:
                    sportName = ViewResources.BasketballPinGoal;
                    break;

                case (int)SportType.SabaESportsPinGoal:
                    sportName = ViewResources.ESportsPinGoal;
                    break;

                case (int)SportType.SabaFutsal:
                    sportName = _cacheRefAdapter.GetSportName((int)SportType.Futsal);
                    break;

                default:
                    sportName = _cacheRefAdapter.GetSportName(sportTypeId);
                    break;
            }
            return $"{prefixText}{sportName}";
        }
    }
}
