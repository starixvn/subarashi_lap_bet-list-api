namespace Fanex.Athena.Common.Utility.Helpers
{
    public static class ValidateHelper
    {
        public static bool ValidateBaseCurrency(string baseCurrency, bool isAllowEmpty = true)
            => isAllowEmpty || !string.IsNullOrEmpty(baseCurrency);

    }
}
