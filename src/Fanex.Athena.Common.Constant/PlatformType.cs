namespace Fanex.Athena.Common.Constant
{
    using System.Collections.Generic;
    using Resources;

    public static class PlatformType
    {
        private const string DesktopNewAsia = "DesktopNewAsia";
        private const string DesktopChina = "DesktopChina";
        private const string DesktopOthers = "DesktopOthers";
        private const string DesktopGalaxy = "DesktopGalaxy";
        private const string DesktopCompact = "DesktopCompact";
        private const string MobileStandard = "MobileStandard";
        private const string MobileEuro = "MobileEuro";
        private const string MobileGalaxy = "MobileGalaxy";
        private const string MobileCompact = "MobileCompact";
        private const string MobileOthers = "MobileOthers";
        private const string ESportsDesktop = "ESportsDesktop";
        private const string ESportsMobile = "ESportsMobile";
        private const string ESportsApp = "ESportsApp";
        private const string MobileApp = "MobileApp";
        private const string NativeApp = "NativeApp";
        private const string Tablet = "Tablet";
        private const string DeepLink = "DeepLink";
        private const string Wap = "Wap";
        private const string ChatRoom = "ChatRoom";
        private const string Mobile = "Mobile";
        private const string Desktop = "Desktop";

        private static readonly IDictionary<string, string> _normalPlatform = new Dictionary<string, string>
        {
            { "x", DesktopNewAsia },
            { "a", DesktopChina },
            { "z", DesktopChina },
            { "[", DesktopChina },
            { "9", DesktopOthers },
            { ">", DesktopOthers },
            { "W001", DesktopGalaxy },
            { "W003", DesktopCompact },
            { "l", MobileStandard },
            { "]", MobileStandard },
            { "%", MobileEuro },
            { "W002", MobileGalaxy },
            { "W004", MobileCompact },
            { "c", MobileCompact },
            { "@", MobileCompact },
            { "7", MobileOthers },
            { "8", MobileOthers },
            { "#", MobileOthers },
            { "_", MobileOthers },
            { "$", MobileOthers },
            { ".", MobileOthers },
            { "}", MobileOthers },
            { "1", ESportsDesktop },
            { "4", ESportsDesktop },
            { "2", ESportsMobile },
            { "5", ESportsMobile },
            { "3", ESportsApp },
            { "6", ESportsApp },
            { "o", MobileApp },
            { "~", MobileApp },
            { "0", NativeApp },
            { "r", Tablet },
            { "t", DeepLink },
            { "m", Wap },
            { "A000", ChatRoom },
            { "A003", ChatRoom },
        };

        private static readonly IDictionary<string, string> _JollysportsPlatform = new Dictionary<string, string>
        {
            { "A001", Desktop },
            { "A002", Mobile },
        };

        public static string GetPlatformName(string code, int specialPlatform)
        {
            if (specialPlatform == SpecialPlatform.Jollysports || specialPlatform == SpecialPlatform.JollysportsCV)
            {
                return GetPlatformNameFromResources(code, _JollysportsPlatform);
            }

            return GetPlatformNameFromResources(code, _normalPlatform);
        }

        private static string GetPlatformNameFromResources(string code, IDictionary<string, string> platformList)
        {
            if (!string.IsNullOrWhiteSpace(code) && platformList.ContainsKey(code))
            {
                return ViewResources.ResourceManager.GetString(platformList[code]);
            }

            return ViewResources.BetfromOthers;
        }
    }
}