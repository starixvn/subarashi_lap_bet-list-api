namespace Fanex.Athena.Common.Utility.Helpers
{
    public static class ValidateHelper
    {
        public static bool ValidateBaseCurrency(string baseCurrency, bool isAllowEmpty = true)
        {
            return isAllowEmpty || !string.IsNullOrEmpty(baseCurrency);
        }

    }
}
