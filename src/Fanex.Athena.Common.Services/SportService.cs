using Fanex.Athena.Common.Constant;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Resources;

namespace Fanex.Athena.Common.Services
{
    public class SportService : ISportService
    {
        private static readonly IEnumerable<(int Id, int SportTypeId, IEnumerable<int> LeagueGroupIds)> SabaVirtualSports = new List<(int, int, IEnumerable<int>)>
        {
            ((int)SportType.SabaSoccer, (int)SportType.Soccer, new [] { 42, 74 }),
            ((int)SportType.SabaBasketball, (int)SportType.BasketBall, new [] { 42 }),
            ((int)SportType.SabaCricket, (int)SportType.Cricket, new [] { 42 }),
            ((int)SportType.SabaFutsal, (int)SportType.Soccer, new [] { 108 }),
            ((int)SportType.SabaTennis, (int)SportType.Tennis, new [] { 42 }),
            ((int)SportType.SabaIceHockey, (int)SportType.IceHockey, new [] { 42 }),
            ((int)SportType.SabaKabaddi, (int)SportType.Kabaddi, new [] { 42 }),
            ((int)SportType.SabaSoccerPingoal, (int)SportType.Soccer, new [] { 113 }),
            ((int)SportType.SabaBasketballPinGoal, (int)SportType.BasketBall, new [] { 122 }),
            ((int)SportType.SabaESportsPinGoal, (int)SportType.Esports, new [] { 151 })
        };

        private readonly ICacheRefAdapter _cacheRefAdapter;

        public SportService(ICacheRefAdapter cacheRefAdapter)
        {
            _cacheRefAdapter = cacheRefAdapter;
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
