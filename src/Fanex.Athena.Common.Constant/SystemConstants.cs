namespace Fanex.Athena.Common.Constant;

using System.Collections.ObjectModel;

public static class SystemConstants
{
    public const int SheetSizeExcel = 60000;

    public const string DefaultBookedBetRefno = "100000000000";

    public static readonly ReadOnlyCollection<string> BaseCurrencyList = new(new List<string>
    {
        "EUR", "USD", "GBP",
        "MYR", "CNY", "THB",
        "IDR", "INH", "BDT",
        "INR", "PHP", "MP",
        "NPR", "TRY", "USDT",
        "JPY", "MXN", "NGN",
        "BRL", "PKR"
    });
}