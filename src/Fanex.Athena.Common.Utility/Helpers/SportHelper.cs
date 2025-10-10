namespace Fanex.Athena.Report.Common.Helpers
{
    using Athena.Common.Constant;

    public static class SportHelper
    {
        private static readonly IEnumerable<int> VSNewRacingSportTypes = new List<int>
        {
            (int)SportType.VirtualHorseRacing,
            (int)SportType.VirtualDogRacing,
            (int)SportType.VirtualSteepleChase,
            (int)SportType.VirtualHarnessRacing,
            (int)SportType.VirtualMotorRacing
        };

        private static readonly IEnumerable<int> VSSportTypes = new List<int>
        {
            (int)SportType.VirtualGreyhound,
            (int)SportType.VirtualMotoRacing,
            (int)SportType.VirtualSpeedway,
            (int)SportType.VirtualCycling
        };

        private static readonly IEnumerable<int> BetbySportTypes = new List<int>
        {
            (int)SportType.BetbyBasketball,
            (int)SportType.BetbyeVolleyball,
            (int)SportType.BetbyeSoccer
        };

        private static readonly IEnumerable<int> Match3rdSportTypes = new List<int>
        {
            (int)SportType.BetbyBasketball,
            (int)SportType.VirtuallySports,
            (int)SportType.VirtualSteepleChase,
            (int)SportType.VirtualHarnessRacing,
            (int)SportType.VirtualMotorRacing,
            (int)SportType.BetbyeVolleyball,
            (int)SportType.BetbyeSoccer
        };

        private static readonly IEnumerable<int> League3rdSportsTypes = new List<int>
        {
            (int)SportType.BitCoin,
            (int)SportType.VirtualHorseRacing,
            (int)SportType.VirtualDogRacing,
            (int)SportType.BetbyBasketball,
            (int)SportType.VirtuallySports,
            (int)SportType.VirtualSteepleChase,
            (int)SportType.VirtualHarnessRacing,
            (int)SportType.VirtualMotorRacing,
            (int)SportType.BetbyeVolleyball,
            (int)SportType.BetbyeSoccer
        };

        public static bool IsVSNewRacingSportTypes(int sportType) => VSNewRacingSportTypes.Contains(sportType);

        public static bool IsLeague3rdSports(int sportType) => League3rdSportsTypes.Contains(sportType);

        public static bool IsMatch3rdSports(int sportType) => Match3rdSportTypes.Contains(sportType);

        public static bool IsVSSportTypes(int sportType) => VSSportTypes.Contains(sportType);

        public static bool IsBetbySportTypes(int sportType) => BetbySportTypes.Contains(sportType);
    }
}