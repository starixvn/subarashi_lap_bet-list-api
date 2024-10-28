namespace Fanex.Athena.BetList.Model.MetaModel
{
    public class MixParlayMetaModel
    {
        public int Page { get; set; }

        public int Rows { get; set; }

        public int CurrencyId { get; set; }

        public string BaseCurrency { get; set; }

        public int BetType { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ProductCodeList { get; set; }

        public bool? SubsidiarySearch { get; set; } = false;

        public string SubsidiaryList { get; set; }

        public string SortBy { get; set; }

        public string SortOrder { get; set; }
    }
}
