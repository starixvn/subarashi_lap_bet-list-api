namespace Fanex.Athena.Common.Model.DTO
{
    public class Product
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public bool AllowCommission { get; set; } = true;

        public bool AllowWinloss { get; set; } = true;

        public bool IsOfferedSingleWallet { get; set; }
    }
}
