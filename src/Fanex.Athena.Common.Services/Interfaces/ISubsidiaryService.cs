namespace Fanex.Athena.Common.Services.Interfaces
{
    public interface ISubsidiaryService
    {
        Task<IDictionary<int, int[]>> GetAgentList(string subsidiaryList);

        bool IsIgnoredSubsidiarySearch(bool subsidiarySearch, string subsidiaryList);
    }
}
