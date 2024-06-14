namespace Fanex.Athena.BetList.Model.MetaModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.Athena.Common.Model.DTO;

    public class LastBetsMonitoringMetaModel
    {
        private const int DefaultPageSize = 10;

        public LastBetsMonitoringMetaModel()
        {
            PageSize = DefaultPageSize;
        }

        public string SportTypeList { get; set; }

        public string LeagueList { get; set; }

        public int MatchId { get; set; }

        public string CustomerClass { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string BetType { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Level { get; set; }

        public int LiveStatus { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string OddsType { get; set; }

        public int TimeZone { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Currency { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string BasedCurrency { get; set; }

        public int StakeFrom { get; set; }

        public int StakeTo { get; set; }

        public int PageSize { get; set; }

        public long TransId { get; set; }

        public long TransLogId { get; set; }

        public string BetFrom { get; set; }

        public string AgentListXml { get; set; }

        public bool IsShowIpvn { get; set; }

        public bool IsShowIp { get; set; }

        public string ProductCodeList { get; set; }

        public int SpecialPlatform { get; set; }

        public IEnumerable<RiskLevel> RiskLevels { get; set; } = new List<RiskLevel>();
    }
}