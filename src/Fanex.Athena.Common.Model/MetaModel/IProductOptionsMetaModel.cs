namespace Fanex.Athena.Common.Model.MetaModel
{
    using DTO;

    public interface IProductOptionsMetaModel
    {
        IEnumerable<ProductOption> ProductOptions { get; set; }

        IEnumerable<ProductOption> OriginalProductOptions { get; set; }
    }
}
