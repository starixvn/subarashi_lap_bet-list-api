namespace Fanex.Athena.Result.Model.MetaModel
{
    public class ResultKenoMetaModel
    {
        public int VendorProductId { get; set; }

        public string CustId { get; set; }

        public string LanguageCode { get; set; }

        public bool IsDesktop { get; set; } = true;
    }
}
