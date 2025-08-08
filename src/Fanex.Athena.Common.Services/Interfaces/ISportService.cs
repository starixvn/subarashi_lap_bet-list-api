namespace Fanex.Athena.Common.Services.Interfaces
{
    using Model.DTO;

    public interface ISportService
    {
        IEnumerable<SabaSport> GetSabaSports(bool isHideSaba = false);

        SabaSport GetSabaSport(int id, bool isHideSaba = false);

        string BuildSportTypeXML(string sportIds);
    }
}