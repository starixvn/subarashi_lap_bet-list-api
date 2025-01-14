using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Report.Common.Helpers;
using Fanex.Athena.Resources;
using Fanex.RemotingRef;

namespace Fanex.Athena.Common.Utility.Services
{
    public class CacheRefAdapter : ICacheRefAdapter
    {
        public IRemotingRefService RemotingRefService { get; }

        public CacheRefAdapter(IRemotingRefService remotingRefService)
        {
            RemotingRefService = remotingRefService;
        }

        public string GetSportName(object sportId, string languageCode = null)
        {
            if (!IsValidId(sportId))
            {
                return string.Empty;
            }

            return RemotingRefService.GetSportName(sportId.ToString(), GetCurrentIBCLanguageCode(languageCode), string.Empty);
        }

        public string GetTeamName(object teamId, int sportId = 0)
        {
            if (!IsValidId(teamId))
            {
                return string.Empty;
            }

            if (SportHelper.IsMatch3rdSports(sportId))
            {
                return RemotingRefService.GetTeamName3rd(teamId.ToString(), GetCurrentIBCLanguageCode(), string.Empty);
            }

            return RemotingRefService.GetTeamName(teamId.ToString(), GetCurrentIBCLanguageCode(), string.Empty);
        }

        public string GetBettype(object bettypeId)
        {
            if (!IsValidId(bettypeId))
            {
                return string.Empty;
            }

            return RemotingRefService.GetBetTypeName(bettypeId.ToString(), GetCurrentIBCLanguageCode());
        }

        public string GetLeagueName(object leagueId, int sportId = 0, string langCode = null)
        {
            if (!IsValidId(leagueId))
            {
                return string.Empty;
            }

            if (SportHelper.IsLeague3rdSports(sportId))
            {
                return RemotingRefService.GetLeagueName3rd(leagueId.ToString(), GetCurrentIBCLanguageCode(), string.Empty);
            }

            return RemotingRefService.GetLeagueName(leagueId.ToString(), GetCurrentIBCLanguageCode(langCode), string.Empty);
        }

        public string GetLeagueName3rd(object leagueId)
        {
            if (!IsValidId(leagueId))
            {
                return string.Empty;
            }

            return RemotingRefService.GetLeagueName3rd(leagueId.ToString(), GetCurrentIBCLanguageCode(), string.Empty);
        }

        public string[] GetCombinationTypeCombo(string betTeam, string betCheck, decimal handicap1, decimal handicap2)
        {
            return RemotingRefService.GetCombinationTypeCombo(
                betTeam,
                betCheck,
                Convert.ToInt32(handicap1),
                Convert.ToInt32(handicap2),
                GetCurrentIBCLanguageCode());
        }

        public string[] GetBetBuilderSelectionNames(int[] selectionIds)
        {
            return RemotingRefService.GetBetBuilderSelectionNames(selectionIds, GetCurrentIBCLanguageCode());
        }

        private static bool IsValidId(object id)
        {
            return id != null && id.ToString() != string.Empty && id.ToString() != "0";
        }

        private static string GetCurrentIBCLanguageCode(string languageCode = null)
        {
            return string.IsNullOrEmpty(languageCode) ? Thread.CurrentThread.CurrentUICulture.IetfLanguageTag : languageCode;
        }
    }
}