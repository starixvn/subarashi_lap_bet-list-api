namespace Fanex.Athena.BetList.DataAccess.Util
{
    using Common.Constant;
    using Data;
    using Fanex.Athena.Common.Model.MetaModel;
    using Model.MetaModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class BetListParameterBuilder
    {
        private static readonly IReadOnlyDictionary<string, Action<IObjectDb, BetListMetaModel>> NonBaseCurrencySpNameActionMap =
            new Dictionary<string, Action<IObjectDb, BetListMetaModel>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Lrf_BetList_MatchWL", (db, metalModel) => CreateMatchWLBetListRangeBettransParameters(db, metalModel) },
            { "Lrf_BetList_MatchWL_Old", (db, metalModel) => CreateMatchWLBetlistParameters(db, metalModel) },
            { "Shard_Lrf_BetList_CustomerWL_Mem", (db, metalModel) => CreateCustomerWlBetlistParameters(db, metalModel) },
            { "Shard_Lrf_BetList_LicenseeWL_Mem", (db, metalModel) => CreateLicenseeWLBetlistParameters(db, metalModel) },
            { "Shard_Lrf_BetList_Forecast_SelByMatch", (db, metalModel) => CreateRunningBetListByForeCastMatchParameters(db, metalModel) },
            { "Lrf_BetList_SelRunningByBettype", (db, metalModel) => CreateRunningBetListByBettypePaprameters(db, metalModel) },
            { "Lrf_BetList_SelRunningByMultipleBettypeMatch", (db, metalModel) => CreateMatchRunningBetLitByMultipleBettypeOfMatchParameters(db, metalModel) },
            { "Lrf_BetList_SelRunningByBettypeList", (db, metalModel) => CreateRunningBetListByMultipleBettypePaprameters(db, metalModel) },
            { "Lrf_TotalBets_Sel430", (db, metalModel) => CreateCorectScoreBetlistParameters(db, metalModel) },
            { "Shard_Lrf_BetList_SelRunningByBettypeMatch", (db, metalModel) => CreateMatchRunningBetLitByBettypeParameters(db, metalModel) },
            { "Lrf_BetList_LicenseeBonus_Detail", (db, metalModel) => CreateBonusDetailParameters(db, metalModel) },
            { "Lrf_BetList_PlatformWinLoss_Mem", (db, metalModel) => CreatePlatformWinLossBetListParameters(db, metalModel) },
            { "Shard_Lrf_BetList_SelRunningByDate_Mem", (db, metalModel) => CreateRunningBetListParameters(db, metalModel) },
            { "Lrf_BetList_RNG_Detail", CreateCasinoDetailParameters},
            { "SiteDB_Lrf_BetList_RNG_Detail", CreateCasinoDetailBettransParameters },
            { "Lrf_BetList_LC_Detail", CreateCasinoDetailParameters },
            { "SiteDB_Lrf_BetList_LC_Detail", CreateCasinoDetailBettransParameters },
            { "Lrf_BetList_Jackpot_Detail", JackpotWonDetailParameters },
            { "SiteDB_Lrf_BetList_Jackpot_Detail", JackpotWonDetailBettransParameters },
            { "Lrf_BetList_SabaVirtualSports_Mem", (db, metalModel) => CreateSabaVirtualSportseWLBetlistParameters(db, metalModel) },
            { "Lrf_BetList_GodOfFortuneWL_Mem", (db, metalModel) => CreateGodOfFortuneWLBetlistParameters(db, metalModel) },
            { "Lrf_BetList_LiveReport", (db, metalModel) => CreateLiveReportBetlistParameters(db, metalModel) },
            { "Lrf_BetList_LiveReport_SelWinlossNumberGame", (db, metalModel) => CreateLiveReportNumberGameBetlistParameters(db, metalModel) },
            { "Lrf_BetList_LiveReport_SelWinlossByMatch", (db, metalModel) => CreateLiveReportBetListByMatchParameters(db, metalModel) },
            { "Lrf_BetList_LiveReport_SelWinlossParlay", (db, metalModel) => CreateLiveReportParlayBetlistParameters(db, metalModel) },
            { "Lrf_BetList_LiveReport_SelWinlossOutright", (db, metalModel) => CreateLiveReportOutRightBetlistParameters(db, metalModel) },
            { "Lrf_BetList_LiveReport_SelWinlossVHRacing", (db, metalModel) => CreateLiveReportBetListVHRacingParameters(db, metalModel) },
        };

        public static bool IsExistedSpActionMap(string uspKey) => NonBaseCurrencySpNameActionMap.ContainsKey(uspKey);

        public static void CreateBetListParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            NonBaseCurrencySpNameActionMap[metaModel.UspKey](db, metaModel);
        }

        public static void CreateBetListDateRangeParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.FromDate, metaModel.FromDate);
            db.SetParameterValue(DbColumn.ToDate, metaModel.ToDate);
        }

        public static void CreateBetlistBUParameters(IObjectDb db, IBalanceUpMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
            db.SetParameterValue(DbColumn.BalanceUpDay, metaModel.BalanceUpDay);
        }

        public static void CreateBetListPagingParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.PageSize, metaModel.PageSize);
            db.SetParameterValue(DbColumn.PageIndex, metaModel.PageIndex);
            db.SetParameterValue(DbColumn.TotalRows, null);
        }

        private static void CreateMatchRunningBetLitByMultipleBettypeOfMatchParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.BetType, !string.IsNullOrEmpty(metaModel.MultipleBettype) ? metaModel.MultipleBettype : string.Empty);
            db.SetParameterValue(DbColumn.MatchID, metaModel.MatchId);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveIndicator ?? -1);
            db.SetParameterValue(DbColumn.BetTeam, metaModel.BetTeam);
            CreateBetListPagingParameters(db, metaModel);
        }

        private static void CreateRunningBetListByForeCastMatchParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.BetType, metaModel.BetType);
            db.SetParameterValue(DbColumn.MatchID, metaModel.MatchId);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveIndicator ?? -1);
            db.SetParameterValue(DbColumn.BetTeam, metaModel.BetTeam);
            db.SetParameterValue("BetChoice", metaModel.BetChoice);
            db.SetParameterValue("Handicap", metaModel.Handicap);
            CreateBetListPagingParameters(db, metaModel);
        }

        private static void CreateMatchRunningBetLitByBettypeParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.BetType, metaModel.BetType);
            db.SetParameterValue(DbColumn.MatchID, metaModel.MatchId);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveIndicator ?? -1);
            db.SetParameterValue(DbColumn.BetTeam, metaModel.BetTeam);
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            CreateBetListPagingParameters(db, metaModel);
        }

        private static void CreateRunningBetListParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.ExcludeFreeBet, metaModel.ExcludeFreeBet);
            CreateBetListDateRangeParameters(db, metaModel);
            CreateBetListPagingParameters(db, metaModel);
            CreateProductListParameters_New(db, metaModel.ProductCodeList);
        }

        private static void CreatePlatformWinLossBetListParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.DrawFilter, metaModel.DrawFilter);
            db.SetParameterValue(DbColumn.PlatformType, metaModel.PlatformType);
            CreateBetListDateRangeParameters(db, metaModel);
            CreateBetlistBUParameters(db, metaModel);
            CreateProductListParameters(db, metaModel.ProductCodeList);
            CreateBetListPagingParameters(db, metaModel);
            CreateSummaryOutputParameters(db);
        }

        private static void CreateMatchWLBetListRangeBettransParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            CreateMatchWLBetlistParameters(db, metaModel);
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
        }

        private static void CreateMatchWLBetlistParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.Date, metaModel.FromDate);
            db.SetParameterValue(DbColumn.MatchID, metaModel.MatchId);
            db.SetParameterValue(DbColumn.BetType, metaModel.BetType);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveIndicator ?? -1);
            CreateBetListSortingParameters(db, metaModel);
            CreateBetListPagingParameters(db, metaModel);
            CreateSummaryOutputParameters(db);

            var productList = metaModel.ProductCodeList.Split(',').ToList();
            int indexBA = productList.IndexOf("BA");
            if (indexBA > -1)
            {
                db.SetParameterValue("ShowBA", true);
                productList.RemoveAt(indexBA);
            }

            db.SetParameterValue(DbColumn.ProductList, string.Join(",", productList));
        }

        private static void CreateLiveReportBetListVHRacingParameters(IObjectDb db, BetListMetaModel betlistMetaModel)
        {
            var metaModel = betlistMetaModel as LiveReportBetListMetaModel;
            var defaultParams = new[] { "SUMWL", DbColumn.SumLicenseeWL };

            CreateBetListSortingParameters(db, metaModel);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            CreateBetListDateRangeParameters(db, metaModel);
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
            AddLiveReportPagingParams(db, metaModel);
            db.SetParameterValue(DbColumn.SportType, metaModel.SportType);
            db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
            foreach (var name in defaultParams)
            {
                db.SetParameterValue(name, null);
            }
        }

        private static void CreateLiveReportParlayBetlistParameters(IObjectDb db, BetListMetaModel betlistMetaModel)
        {
            var metaModel = betlistMetaModel as LiveReportBetListMetaModel;
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            CreateBetListSortingParameters(db, metaModel);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            CreateBetListDateRangeParameters(db, metaModel);
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
            AddLiveReportPagingParams(db, metaModel);
            db.SetParameterValue(DbColumn.BetType, metaModel.BetType);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveIndicator);
            CreateProductListParameters_New(db, metaModel.ProductCodeList);
        }

        private static void CreateLiveReportNumberGameBetlistParameters(IObjectDb db, BetListMetaModel betlistMetaModel)
        {
            var metaModel = betlistMetaModel as LiveReportBetListMetaModel;

            CreateBetListSortingParameters(db, metaModel);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            CreateBetListDateRangeParameters(db, metaModel);
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
            AddLiveReportPagingParams(db, metaModel);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveIndicator);
            db.SetParameterValue(DbColumn.SportType, metaModel.SportType);
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
        }

        private static void CreateLiveReportBetListByMatchParameters(IObjectDb db, BetListMetaModel betlistMetaModel)
        {
            var metaModel = betlistMetaModel as LiveReportBetListMetaModel;
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.MatchID, metaModel.MatchId);
            AddLiveReportPagingParams(db, metaModel);
            db.SetParameterValue(DbColumn.LiveIndicator, metaModel.LiveIndicator);
            db.SetParameterValue(DbColumn.FromDate, metaModel.FromDate);
            db.SetParameterValue(DbColumn.ToDate, metaModel.ToDate);
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
            CreateProductListParameters_New(db, metaModel.ProductCodeList);
        }

        private static void CreateLiveReportOutRightBetlistParameters(IObjectDb db, BetListMetaModel betlistMetaModel)
        {
            var metaModel = betlistMetaModel as LiveReportBetListMetaModel;
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            CreateBetListDateRangeParameters(db, metaModel);
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
            AddLiveReportPagingParams(db, metaModel);
            var supportedProducts = new[] { ProductCodes.SB, ProductCodes.BA };
            CreateProductListParameters(db, metaModel.ProductCodeList, supportedProducts);
        }

        private static void CreateLicenseeWLBetlistParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.DrawFilter, metaModel.DrawFilter);
            CreateBetListDateRangeParameters(db, metaModel);
            CreateBetlistBUParameters(db, metaModel);
            CreateProductListParameters(db, metaModel.ProductCodeList);
            CreateBetListPagingParameters(db, metaModel);
            CreateSummaryOutputParameters(db);
        }

        private static void CreateSabaVirtualSportseWLBetlistParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.DrawFilter, metaModel.DrawFilter);
            db.SetParameterValue(DbColumn.LeagueId, metaModel.LeagueId);
            db.SetParameterValue(DbColumn.SportType, metaModel.SportType);
            CreateBetListDateRangeParameters(db, metaModel);
            CreateBetlistBUParameters(db, metaModel);
            CreateBetListPagingParameters(db, metaModel);
            CreateSummaryOutputParameters(db);
        }

        private static void CreateGodOfFortuneWLBetlistParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.DrawFilter, metaModel.DrawFilter);
            CreateBetListDateRangeParameters(db, metaModel);
            CreateBetlistBUParameters(db, metaModel);
            CreateBetListPagingParameters(db, metaModel);
            CreateSummaryOutputParameters(db);
        }

        private static void CreateCorectScoreBetlistParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            CreateBetListPagingParameters(db, metaModel);
        }

        private static void CreateCustomerWlBetlistParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            CreateBetListDateRangeParameters(db, metaModel);
            CreateBetlistBUParameters(db, metaModel);
            CreateBetListPagingParameters(db, metaModel);
            CreateSummaryOutputParameters(db);
            CreateProductListParameters_New(db, metaModel.ProductCodeList);
        }

        private static void CreateRunningBetListByBettypePaprameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.BetType, metaModel.BetType);
            CreateBetListPagingParameters(db, metaModel);
        }

        private static void CreateRunningBetListByMultipleBettypePaprameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.BetType, metaModel.MultipleBettype);
            CreateBetListPagingParameters(db, metaModel);
        }

        private static void CreateCasinoDetailParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.RefNo, metaModel.TransId);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.TransType, metaModel.TransType);
            CreateExcludeBettransParam(db, metaModel);
            CreateBetlistBUParameters(db, metaModel);
        }

        private static void CreateCasinoDetailBettransParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.RefNo, metaModel.TransId);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.TransType, metaModel.TransType);
            CreateBetlistBUParameters(db, metaModel);
        }

        private static void CreateBonusDetailParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.TransId, metaModel.TransId);
            db.SetParameterValue(DbColumn.RefCode, metaModel.RefCode);
            db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
        }

        private static void JackpotWonDetailParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            JackpotWonDetailBettransParameters(db, metaModel);
            CreateExcludeBettransParam(db, metaModel);
        }

        private static void JackpotWonDetailBettransParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            db.SetParameterValue(DbColumn.TransId, metaModel.TransId);
            db.SetParameterValue(DbColumn.CustId, metaModel.CustId);
            db.SetParameterValue(DbColumn.BaseCurrency, metaModel.BaseCurrency);
        }

        private static void CreateLiveReportBetlistParameters(IObjectDb db, BetListMetaModel betlistMetaModel)
        {
            var metaModel = betlistMetaModel as LiveReportBetListMetaModel;
            db.SetParameterValue(nameof(metaModel.BaseCurrency), metaModel.BaseCurrency);
            db.SetParameterValue(DbColumn.AgentList, metaModel.AgentListXml);
            CreateBetListDateRangeParameters(db, metaModel);
            db.SetParameterValue(DbColumn.MoveBetsDay, metaModel.MoveBetsDay);
            db.SetParameterValue(DbColumn.SportType, metaModel.SportType);
            AddLiveReportPagingParams(db, metaModel);
            CreateBetListSortingParameters(db, metaModel);
            db.SetParameterValue(DbColumn.ProductList, metaModel.ProductCodeList);
            db.SetParameterValue(DbColumn.MatchId, metaModel.MatchId);
        }

        private static void CreateExcludeBettransParam(IObjectDb db, IShardDBMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.ExcludeBettrans, metaModel.UseShardDB);
        }

        #region Utility

        private static void AddLiveReportPagingParams(IObjectDb db, LiveReportBetListMetaModel metaModel)
        {
            db.SetParameterValue(DbColumn.PageSize, metaModel.IsExportExcel ? int.MaxValue - 1 : metaModel.PageSize);
            db.SetParameterValue(DbColumn.PageIndex, metaModel.IsExportExcel ? 1 : metaModel.PageIndex);
        }

        private static void CreateBetListSortingParameters(IObjectDb db, BetListMetaModel metaModel)
        {
            if (!string.IsNullOrWhiteSpace(metaModel.SortBy))
            {
                db.SetParameterValue(DbColumn.Sort, metaModel.SortBy + " " + metaModel.SortOrder);
            }
        }

        private static void CreateSummaryOutputParameters(IObjectDb db)
        {
            var defaultParams = new[] { DbColumn.SumWinLost, DbColumn.SumLicenseeWL, DbColumn.SumComm, DbColumn.SumPlayerComm };

            foreach (var name in defaultParams)
            {
                db.SetParameterValue(name, null);
            }
        }

        private static void CreateProductListParameters(IObjectDb db, string productCodeList, IEnumerable<string> supportedProducts = null)
        {
            if (string.IsNullOrWhiteSpace(productCodeList))
            {
                return;
            }

            IEnumerable<string> products = productCodeList.Split(',');

            if (supportedProducts != null)
            {
                products = products.Where(x => supportedProducts.Contains(x)).ToList();
            }

            foreach (var productCode in products)
            {
                db.SetParameterValue("show" + productCode, true);
            }
        }

        private static void CreateProductListParameters_New(IObjectDb db, string productCodeList)
        {
            var productList = productCodeList.Split(',').ToList();
            int indexBA = productList.IndexOf(ProductCodes.BA);
            if (indexBA > -1)
            {
                db.SetParameterValue(DbColumn.ShowBA, true);
                productList.RemoveAt(indexBA);
            }

            db.SetParameterValue(DbColumn.ProductList, string.Join(",", productList));
        }

        #endregion Utility
    }
}