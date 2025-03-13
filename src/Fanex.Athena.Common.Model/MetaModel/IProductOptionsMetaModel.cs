using Fanex.Athena.Common.Model.DTO;

namespace Fanex.Athena.Common.Model.MetaModel
{
    public interface IProductOptionsMetaModel
    {
        IEnumerable<ProductOption> ProductOptions { get; set; }

        IEnumerable<ProductOption> OriginalProductOptions { get; set; }
    }
}
