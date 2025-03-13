using Fanex.Athena.Common.Constant;

namespace Fanex.Athena.Report.Common.Helpers
{
    public static class SportHelper
    {
        private static readonly IEnumerable<int> VSBetRadarSportTypes = new List<int>
        {
            (int)SportType.VirtualHorseRacing,
            (int)SportType.VirtualDogRacing
        };

        private static readonly IEnumerable<int> VSSportTypes = new List<int>
        {
            (int)SportType.VirtualGreyhound,
            (int)SportType.VirtualMotoRacing,
            (int)SportType.VirtualSpeedway,
            (int)SportType.VirtualCycling
        };

        private static readonly IEnumerable<int> Match3rdSportTypes = new List<int>
        {
            (int)SportType.BetbyBasketball,
            (int)SportType.VirtuallySports
        };

        private static readonly IEnumerable<int> League3rdSportsTypes = new List<int>
        {
            (int)SportType.BitCoin,
            (int)SportType.VirtualHorseRacing,
            (int)SportType.VirtualDogRacing,
            (int)SportType.BetbyBasketball,
            (int)SportType.VirtuallySports
        };

        public static bool IsVirtualBetRadarSports(int sportType) => VSBetRadarSportTypes.Contains(sportType);

        public static bool IsLeague3rdSports(int sportType) => League3rdSportsTypes.Contains(sportType);

        public static bool IsMatch3rdSports(int sportType) => Match3rdSportTypes.Contains(sportType);

        public static bool IsVSSportTypes(int sportType) => VSSportTypes.Contains(sportType);
    }
}