using Fanex.Athena.Common.Constant;

namespace Fanex.Athena.Common.Utility.Helpers
{
    public static class SabaHelper
    {
        public static bool IsHideSaba(int specialPlatform)
        {
            if (specialPlatform == SpecialPlatform.Jollysports || specialPlatform == SpecialPlatform.JollysportsCV)
            {
                return true;
            }
            return false;
        }
    }
}