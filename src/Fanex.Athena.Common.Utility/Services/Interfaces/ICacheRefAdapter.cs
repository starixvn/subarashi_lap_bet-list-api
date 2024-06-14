namespace Fanex.Athena.Common.Utility.Services.Interfaces
{
    using RemotingRef;

    public interface ICacheRefAdapter
    {
        IRemotingRefService RemotingRefService { get; }

        string[] GetCombinationTypeCombo(string betTeam, string betCheck, decimal handicap1, decimal handicap2);

        string GetLeagueName(object leagueId);

        string GetLeagueName3rd(object leagueId);

        string GetSportName(object sportId, string languageCode = null);

        string[] GetBetBuilderSelectionNames(int[] selectionIds);
    }
}