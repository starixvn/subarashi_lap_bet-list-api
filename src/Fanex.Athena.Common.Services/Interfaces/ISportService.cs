using Fanex.Athena.Common.Model.DTO;

namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface ISportService
    {
        IEnumerable<SabaSport> GetSabaSports(bool isHideSaba = false);

        SabaSport GetSabaSport(int id, bool isHideSaba = false);
    }
}
