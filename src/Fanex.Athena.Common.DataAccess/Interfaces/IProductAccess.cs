using Fanex.Athena.Common.Model.DTO;

namespace Fanex.Athena.Common.DataAccess.Interfaces
{
    public interface IProductAccess
    {
        IList<Product> GetProductAccessList(int licenseeId);
    }
}
