using Fanex.Athena.Common.Model.DTO;

namespace Fanex.Athena.BetList.Service.Interfaces
{
    public interface IBettingTicketCenterSportService
    {
        IEnumerable<SabaSport> GetSabaSports(bool isHideSaba = false);

        SabaSport GetSabaSport(int id, bool isHideSaba = false);

        string BuildSportTypeXML(string sportIds);
    }
}
