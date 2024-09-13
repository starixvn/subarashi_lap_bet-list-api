namespace Fanex.Athena.BetList.Constants
{
    public static class ExchangeConstant
    {
        public const string BetPlaced = "BetPlaced";
        public const string Stake = "Stake";
        public const string Asc = "asc";
        public const string Desc = "desc";
    }

    public enum SortByExchange
    {
        BetPlaced = 0,
        Stake = 1
    }

    public enum SortOrderExchange
    {
        Asc = 0,
        Desc = 1
    }
}
