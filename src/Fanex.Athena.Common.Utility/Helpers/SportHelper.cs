using Fanex.Athena.Common.Constant;

namespace Fanex.Athena.Report.Common.Helpers
{
    public static class SportHelper
    {
        private static readonly IEnumerable<int> VSBetRadarSportTypes = new List<int>
        {
            (int)SportType.VirtualHorseRacing ,
            (int)SportType.VirtualDogRacing
        };

        private static readonly IEnumerable<int> VSSportTypes = new List<int>
        {
            (int) SportType.VirtualGreyhound,
            (int) SportType.VirtualMotoRacing,
            (int) SportType.VirtualSpeedway,
            (int) SportType.VirtualCycling
        };

        public static bool IsVirtualBetRadarSports(int sportType) => VSBetRadarSportTypes.Contains(sportType);

        public static bool IsLeague3rdSports(int sportType) => VSBetRadarSportTypes.Contains(sportType) || sportType == (int) SportType.BetbyBasketball;

        public static bool IsMatch3rdSports(int sportType) => sportType == (int) SportType.BetbyBasketball;

        public static bool IsVSSportTypes(int sportType) => VSSportTypes.Contains(sportType);
    }
}
