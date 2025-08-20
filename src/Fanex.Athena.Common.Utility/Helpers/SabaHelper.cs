namespace Fanex.Athena.Common.Utility.Helpers
{
    using System.Text.RegularExpressions;
    using Constant;
    using Resources;

    public static class SabaHelper
    {
        private const int TIME_OUT = 10;

        public static bool IsHideSaba(int specialPlatform)
        {
            if (specialPlatform == SpecialPlatform.Jollysports || specialPlatform == SpecialPlatform.JollysportsCV)
            {
                return true;
            }
            return false;
        }

        public static string RemoveSabaText(string text)
        {
            var sabaText = ViewResources.Saba.Trim();
            var result = Regex.Replace(text, sabaText, string.Empty, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(TIME_OUT));
            // remove duplicate space
            return Regex.Replace(result, "  ", " ", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(TIME_OUT));
        }
    }
}