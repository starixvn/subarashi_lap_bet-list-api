namespace Fanex.Athena.BetList.Model.MetaModel
{
    using Fanex.Athena.Common.Constant;
    using Fanex.Athena.Common.Model.DTO;
    using Fanex.Athena.Common.Model.MetaModel;
    using Fanex.Athena.Common.Utility.Helpers;
    using Fanex.Core.Utils;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class LastBetsMonitoringMetaModel : IValidationMetaModel, IMetaModelShowBetFromIP, IMetaModelProductCodeList, ISpecialPlatformMetaModel
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

        public string ProductCodeList { get; set; } = ProductCodes.SB;

        public int SpecialPlatform { get; set; }

        public bool IsHideSaba => SabaHelper.IsHideSaba(SpecialPlatform);

        public IEnumerable<RiskLevel> RiskLevels { get; set; } = new List<RiskLevel>();

        public string OriginalProductCodeList { get; set; }

        public string SportIds { get; set; }

        public bool IsValid => ValidateHelper.ValidateBaseCurrency(BasedCurrency);
    }
}