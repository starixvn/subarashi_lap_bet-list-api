namespace Fanex.Athena.BetList.Model.MetaModel
{
    using Fanex.Athena.Common.Model.MetaModel;
    using Fanex.Athena.Common.Utility.Helpers;
    using Fanex.Core.Utils;
    using System;

    public class BetListMetaModel : BaseMetaModel,
        ICommonValueMetaModel,
        IBalanceUpMetaModel,
        IShardDBMetaModel,
        ICurrencyIdMetaModel,
        IMetaModelProductCodeList,
        ISpecialPlatformMetaModel,
        IMetaModelSubsidiarySearch,
        IPlayerComMetaModel,
        IAgentMetaModel
    {
        public BetListMetaModel()
        {
            FromDate = DateTimeHelper.Today;
            Date = DateTimeHelper.Today;
            ToDate = DateTimeHelper.Today;
            PageIndex = 1;
            PageSize = int.MaxValue - 1;
        }

        public bool HasPlayerCom { get; set; }

        public bool IsShowIpvn { get; set; }

        public bool IsShowIp { get; set; }

        public string UspKey { get; set; }

        public string ProductCodeList { get; set; } = string.Empty;

        public string OriginalProductCodeList { get; set; }

        public bool IsShowSumStake { get; set; }

        public bool IsServerPaging { get; set; }

        public string SortBy { get; set; }

        public string SortOrder { get; set; }

        public bool IsExportExcel { get; set; }

        public virtual bool IsConvert => !string.IsNullOrEmpty(BaseCurrency);

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime Date { get; set; }

        public int CustId { get; set; }

        public string SystemId { get; set; }

        public string ExternalId { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public int MatchId { get; set; }

        public int HomeId { get; set; }

        public int AwayId { get; set; }

        public int LeagueId { get; set; }

        public int BetType { get; set; }

        public int SportType { get; set; }

        public string MultipleBettype { get; set; }

        public string BetTeam { get; set; }

        public int? LiveIndicator { get; set; }

        public string RaceNo { get; set; }

        public int DrawFilter { get; set; }

        public long TransId { get; set; }

        public long TransType { get; set; }

        public int PlatformType { get; set; }

        public string BetChoice { get; set; }

        public string Handicap { get; set; }

        public long RefCode { get; set; }

        public string MatchCode { get; set; }

        public string VendorMatchId { get; set; }

        public string Distance { get; set; }

        public bool IsMobileView { get; set; }

        public bool ExcludeFreeBet { get; set; }

        public bool IsEnableSingleWallet { get; set; }

        public bool UseShardDB { get; set; }

        public int SpecialPlatform { get; set; }

        public int EventType { get; set; }

        public string SubsidiaryList { get; set; }

        public bool SubsidiarySearch { get; set; }

        public DateTime KickOffTime { get; set; }

        public bool IsHideSaba => SabaHelper.IsHideSaba(SpecialPlatform);

        public bool InBettransRange => Date > MoveBetsDay;

        public virtual bool IsValid
        {
            get
            {
                return FromDate <= ToDate
                    && !string.IsNullOrWhiteSpace(ProductCodeList);
            }
        }

        public BetListMetaModel ShallowCloneWith(Action<BetListMetaModel> binding)
        {
            var clone = (BetListMetaModel)MemberwiseClone();
            binding?.Invoke(clone);
            return clone;
        }
    }
}