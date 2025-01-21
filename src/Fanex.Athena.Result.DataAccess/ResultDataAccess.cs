using Fanex.Common.Extensions;
using Fanex.Result.Core.Base;
using Fanex.Result.Core.Bitcoin.DTO;
using Fanex.Result.Core.Racing.DTO;
using Fanex.Result.Core.Sportsbook.DTO;
using Fanex.Result.Core.VirtualRacing.DTO;
using System.Data;
using System.Globalization;
using Fanex.Athena.Common.DataAccess;
using Fanex.Athena.Common.DataAccess.Extensions;
using Fanex.Athena.Common.DataAccess.Interfaces;
using Fanex.Athena.Common.Model.DTO;
using Fanex.Athena.Common.Utility.Services.Interfaces;
using Fanex.Athena.Result.Model.MetaModel;

namespace Fanex.Athena.Result.DataAccess
{
    public class ResultDataAccess : BaseDataAccess, IResultDataAccess
    {
        private const string MatchidColumn = "matchid";
        private const string BetstatusColumn = "betstatus";
        private const string EventDateColumn = "eventdate";
        private const string KickOffTimeColumn = "kickofftime";
        private const string LcolorColumn = "lcolor";
        private const string IsFavorColumn = "IsFavor";
        private const string LeagueIdColumn = "leagueid";
        private const string LeagueGroupIdColumn = "leaguegroupid";
        private const string SportTypeColumn = "sporttype";
        private const string Status = "status";

        private readonly IBalanceUpDataAccess _balanceUpDataAccess;

        private readonly ICacheRefAdapter _cacheRefAdapter;

        private readonly IAppSettings _appSettings;

        public ResultDataAccess(
            IBalanceUpDataAccess balanceUpDataAccess,
            ICacheRefAdapter cacheRefAdapter,
            IAppSettings appSettings)
        {
            _balanceUpDataAccess = balanceUpDataAccess;
            _cacheRefAdapter = cacheRefAdapter;
            _appSettings = appSettings;
        }

        public async Task<IList<Outright>> GetOutRightAsync(ResultMetaModel metal)
        {
            const string UspName = "Lrf_Result_Outright";
            var uspParams = new
            {
                fromdate = metal.FromDate,
                todate = metal.ToDate,
                isshowall = !_appSettings.IsProduction
            };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    var data = new List<Outright>();

                    while (reader.Read())
                    {
                        var leagueId = reader.ToInt(LeagueIdColumn);
                        if (metal.SpecialLeagues == null || !metal.SpecialLeagues.Contains(leagueId))
                        {
                            data.Add(new Outright
                            {
                                EventDate = reader.ToDateTime(EventDateColumn),
                                SportType = reader.ToInt(SportTypeColumn),
                                LeagueId = leagueId,
                                TeamId = reader.ToInt("teamid"),
                                Status = reader.ToString("status")
                            });
                        }
                    }

                    return data;
                }
            }
        }

        public async Task<IList<Sport>> GetSportsAsync(DateTime dateValue, int licenseeId)
        {
            const string UspName = "Lrf_Result_SportSelByDate";

            using (var db = GetObjectDb(UspName))
            {
                var balanceUp = _balanceUpDataAccess.GetBalanceUp();
                db.SetParameterValue(EventDateColumn, dateValue);
                db.SetParameterValue("licenseeid", licenseeId);
                db.SetParameterValue("movebetsday", balanceUp.MoveBetsDay);
                db.SetParameterValue("isshowall", !_appSettings.IsProduction);
                using (var reader = await db.ExecuteReaderAsync())
                {
                    var data = new List<Sport>();
                    while (reader.Read())
                    {
                        var sportId = reader.ToInt(SportTypeColumn);
                        data.Add(new Sport
                        {
                            SportId = sportId,
                            SportName = _cacheRefAdapter.GetSportName(sportId)
                        });
                    }

                    return data;
                }
            }
        }

        public async Task<IList<League>> GetLeaguesAsync(DateTime dateValue, int sportId, IEnumerable<int> specialLeagues = null)
        {
            const string UspName = "Lrf_Result_LeagueSelByDate";
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            var uspParams = new
            {
                eventdate = dateValue,
                sporttype = sportId,
                isshowall = !_appSettings.IsProduction,
                movebetsday = balanceUp.MoveBetsDay
            };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    var data = new List<League>();
                    while (reader.Read())
                    {
                        var leagueId = Convert.ToInt32(reader[LeagueIdColumn]);
                        var sportType = Convert.ToInt32(reader[SportTypeColumn]);

                        if (specialLeagues == null || !specialLeagues.Contains(leagueId))
                        {
                            data.Add(new League
                            {
                                LeagueId = leagueId,
                                LeagueName = IsLeagues3rd(sportType) ? _cacheRefAdapter.GetLeagueName3rd(leagueId) : _cacheRefAdapter.GetLeagueName(leagueId)
                            });
                        }
                    }

                    return data;
                }
            }
        }

        private static bool IsLeagues3rd(int sportType)
            => sportType == (int)SportType.Bitcoin || sportType == (int)SportType.VirtualDogRacing || sportType == (int)SportType.VirtualHorseRacing || sportType == (int)SportType.BetbyBasketball;


        public async Task<IList<Racing>> GetVirtualSportMatchesAsync(ResultMetaModel metalModel)
        {
            const string UspName = "Lrf_Result_RaceSelByMeeting";
            var uspParams = new
            {
                fromdate = metalModel.FromDate,
                leagueid = metalModel.LeagueId,
                sporttype = metalModel.SportId,
                movebetsday = _balanceUpDataAccess.GetBalanceUp().MoveBetsDay
            };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    return ReadVirtualSportMatches(reader);
                }
            }
        }

        public async Task<IList<VirtualRacing>> GetBetradarSportMatchesAsync(ResultMetaModel metalModel)
        {
            const string UspName = "Lrf_Result_RaceSelByLeague";
            var uspParams = new
            {
                eventDate = metalModel.FromDate,
                leagueid = metalModel.LeagueId,
                sporttype = metalModel.SportId,
                movebetsday = _balanceUpDataAccess.GetBalanceUp().MoveBetsDay
            };

            using (var db = this.GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    return ReadBetradarSportMatches(reader);
                }
            }
        }

        public async Task<IList<T>> GetPopUpMatchesAsync<T>(ResultPopupMetaModel metaModel)
        {
            const string UspName = "Lrf_BetList_SelMatchResult";
            var uspParams = new
            {
                matchid = metaModel.MatchId,
                raceno = metaModel.RaceNo.HasValue ? metaModel.RaceNo.Value.ToString(CultureInfo.InvariantCulture) : null,
                bettype = metaModel.Bettype,
                betid = metaModel.BetId,
                transdesc = metaModel.TransDesc
            };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    var type = typeof(T);

                    if (type == typeof(Sportsbook))
                    {
                        return (IList<T>)ReadSportbookMatches(reader, metaModel);
                    }
                    else if (type == typeof(Racing))
                    {
                        return (IList<T>)ReadVirtualSportMatches(reader);
                    }
                    else
                    {
                        throw new NotSupportedException($"The type '{type.FullName}' is not supported.");
                    }
                }
            }
        }


        public async Task<IList<Parlay>> GetPopUpParalayMatchAsync(long refNo)
        {
            const string UspName = "Lrf_BetList_SelMatchResultParlay";
            var uspParams = new { refno = refNo };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    var results = new List<Parlay>();

                    while (reader.Read())
                    {
                        var leagueId = reader.ToInt(LeagueIdColumn);
                        var parlay = results.SingleOrDefault(x => x.LeagueId == leagueId);
                        if (parlay == null)
                        {
                            parlay = new Parlay
                            {
                                LeagueId = leagueId,
                                SportId = reader.ToInt(SportTypeColumn)
                            };

                            results.Add(parlay);
                        }

                        parlay.Sportbook.Add(ReadSportsbook(reader, refNo.ToString(CultureInfo.InvariantCulture), leagueId));
                    }

                    AddGameDetails(reader, results);

                    return results;
                }
            }
        }

        public async Task<Sportsbook> GetPopUpSingleMatchParlayAsync(int matchId, string marketId)
        {
            const string UspName = "Lrf_TicketResult_BetBuilder";

            using (var db = GetObjectDb(UspName))
            {
                db.SetParameterValue(MatchidColumn, matchId);
                db.SetParameterValue("marketId", marketId);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var result = ReadSportsbook(reader);

                        result.BetBuilderDetails = GetBetBuilderDetails(reader);

                        return result;
                    }

                    return null;
                }
            }
        }

        public async Task<IList<VirtualRacing>> GetPopUpBetradarAsync(int eventId, DateTime eventDate)
        {
            const string UspName = "Lrf_TicketResult_Race";
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            var result = new List<VirtualRacing>();

            using (var db = this.GetObjectDb(UspName))
            {
                db.SetParameterValue("EventId", eventId);
                db.SetParameterValue("EventDate", eventDate);
                db.SetParameterValue("MoveBetsDay", balanceUp.MoveBetsDay);
                db.SetParameterValue("BalanceUpDay", balanceUp.BalanceUpDay);

                using (var reader = await db.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        var data = new VirtualRacing
                        {
                            LeagueId = reader.ToInt("LeagueID"),
                            EventId = reader.ToInt("EventID"),
                            VendorMatchId = reader.ToInt("VendorMatchId"),
                            EventTime = reader.ToDateTime("EventTime"),
                            EventStatus = reader.ToString("EventStatus"),
                            BetStatus = reader.ToString("BetStatus"),
                            Distance = reader.ToString("Distance"),
                            Details = new List<VirtualRacingDetail>()
                        };

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                data.Details.Add(new VirtualRacingDetail
                                {
                                    Orid = reader.ToInt("Orid"),
                                    EventId = reader.ToInt("EventID"),
                                    Placing = reader.ToInt("Placing"),
                                    Lane = reader.ToString("Lane"),
                                    RacerId = reader.ToInt("RacerID")
                                });
                            }
                        }
                        result.Add(data);
                    }

                    return result;
                }
            }
        }

        public async Task<IList<T>> GetMatchesAsync<T>(ResultMetaModel metalModel)
        {
            const string UspName = "Lrf_Result_MatchSelByLeague";
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            var uspParams = new
            {
                fromdate = metalModel.FromDate,
                sporttype = metalModel.SportId,
                leagueid = metalModel.LeagueId,
                movebetsday = balanceUp.MoveBetsDay,
                isshowall = !_appSettings.IsProduction
            };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    var type = typeof(T);

                    if (type == typeof(Sportsbook))
                    {
                        return (IList<T>)ReadSportbookMatches(reader, specialLeagues: metalModel.SpecialLeagues);
                    }
                    else
                    {
                        throw new NotSupportedException($"The type '{type.FullName}' is not supported.");
                    }
                }
            }
        }

        public async Task<IList<T>> GetMatches3rdAsync<T>(ResultMetaModel metalModel)
        {
            const string UspName = "Lrf_Result_Match3rdSelByLeague";
            var balanceUp = _balanceUpDataAccess.GetBalanceUp();
            var uspParams = new
            {
                date = metalModel.FromDate,
                leagueid = metalModel.LeagueId,
                movebetsday = balanceUp.MoveBetsDay
            };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    var type = typeof(T);

                    if (type == typeof(Bitcoin))
                    {
                        return (IList<T>)ReadBitCoinMatches(reader);
                    }
                    else if (type == typeof(Sportsbook))
                    {
                        return (IList<T>)ReadSportbookMatches3rd(reader);
                    }
                    else
                    {
                        throw new NotSupportedException($"The type '{type.FullName}' is not supported.");
                    }
                }
            }
        }

        public async Task<Sportsbook> GetSportsbookGameDetailAsync(int matchId)
        {
            using (var db = GetObjectDb("Lrf_Result_MatchSelByLeague_Detail"))
            {
                db.SetParameterValue(MatchidColumn, matchId);
                using (var reader = await db.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var result = ReadSportsbook(reader);
                        result.GameDetails = GetGameDetails(reader).ToList();
                        return result;
                    }

                    return null;
                }
            }
        }

        private static void AddGameDetails(IDataReader reader, IEnumerable<Parlay> results)
        {
            var gameDetails = GetGameDetails(reader);

            if (!gameDetails.Any())
            {
                return;
            }

            foreach (var result in results)
            {
                foreach (var item in result.Sportbook)
                {
                    item.GameDetails.AddRange(gameDetails.Where(i => i.MatchId == item.MatchId));
                }
            }
        }

        private static Sportsbook ReadSportsbook(IDataReader reader, string refNo = "", int? leagueId = null)
        {
            return new Sportsbook
            {
                AwayId = reader.ToInt("awayID"),
                HomeId = reader.ToInt("homeID"),
                BetStatus = reader.ToString(BetstatusColumn),
                LeagueId = leagueId ?? reader.ToInt(LeagueIdColumn),
                LeagueGroupId = reader.GetVal(LeagueGroupIdColumn, 0),
                MatchId = reader.ToInt(MatchidColumn),
                EventStatus = reader.ToString("eventstatus"),
                FinalAwayScore = reader.ToNullableInt("finalawayscore"),
                HtAwayScore = reader.ToNullableInt("htawayscore"),
                Color = reader.GetVal(LcolorColumn, string.Empty),
                FinalHomeScore = reader.ToNullableInt("finalhomescore"),
                HtHomeScore = reader.ToNullableInt("hthomescore"),
                EventDate = reader.ToDateTime(EventDateColumn),
                IsNeutral = reader.ToBool("IsNeutral"),
                KickOffTime = reader.ToDateTime(KickOffTimeColumn),
                MatchCode = reader.ToString("matchcode"),
                GoalRecord = reader.ToString("goalrecord"),
                IsHTCal = reader.ToBool("IsHTCal"),
                Calculate = reader.ToBool("Calculate"),
                RefNo = refNo,
                LiveAwayScore = reader.GetVal<int?>("LiveAwayScore"),
                LiveHomeScore = reader.GetVal<int?>("LiveHomeScore"),
                SettlementStatus = reader.ToByte("SettlementStatus"),
                Bettype = reader.GetVal("bettype", 0),
                BetID = reader.GetVal("betcheck", string.Empty),
                NoHTScore = reader.HasColumn("NoHTScore") && reader.ToBool("NoHTScore"),
                MainType = reader.GetVal("MainType", byte.MinValue),
                SubType = reader.GetVal("SubType", 0),
                SessionTime = reader.GetVal("SessionTime", 0)
            };
        }

        private static Sportsbook ReadSportsbook3rd(IDataReader reader)
        {
            return new Sportsbook
            {
                AwayId = reader.ToInt("awayID"),
                HomeId = reader.ToInt("homeID"),
                BetStatus = reader.ToString(BetstatusColumn),
                LeagueId = reader.ToInt("League3rdId"),
                MatchId = reader.ToInt("Match3rdId"),
                EventStatus = reader.ToString("eventstatus"),
                FinalAwayScore = reader.ToNullableInt("finalawayscore"),
                HtAwayScore = reader.ToNullableInt("htawayscore"),
                FinalHomeScore = reader.ToNullableInt("finalhomescore"),
                HtHomeScore = reader.ToNullableInt("hthomescore"),
                EventDate =reader.ToDateTime(EventDateColumn),
                MatchCode = reader.ToString("matchcode")
            };
        }

        #region Binding
        private static IList<Fanex.Result.Core.Sportsbook.DTO.GameDetail> GetGameDetails(IDataReader reader)
        {
            var gameDetails = new List<Fanex.Result.Core.Sportsbook.DTO.GameDetail>();

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var resultStatusId = reader["ResultStatusID"];
                    var settlementStatusId = reader["SettlementStatusID"];

                    gameDetails.Add(new Fanex.Result.Core.Sportsbook.DTO.GameDetail
                    {
                        MatchId = int.Parse(reader[MatchidColumn].ToString()),
                        ResourceId = reader["ResourceId"].ToString(),
                        HomeValue = reader["HomeValue"].ToString(),
                        AwayValue = reader["AwayValue"].ToString(),
                        ResultStatusID = resultStatusId == DBNull.Value ? 0 : int.Parse(resultStatusId.ToString()),
                        SettlementStatusID = settlementStatusId == DBNull.Value ? 0 : int.Parse(settlementStatusId.ToString()),
                        BettypeId = reader.HasColumn("BetType") ? reader.ToInt("BetType") : 0,
                        OtherInfo = reader["OtherInfo"].ToString()
                    });
                }
            }

            return gameDetails;
        }

        private static List<BetBuilderDetail> GetBetBuilderDetails(IDataReader reader)
        {
            var betBuilderDetail = new List<BetBuilderDetail>();

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    betBuilderDetail.Add(new BetBuilderDetail
                    {
                        SelectionType = int.Parse(reader["SelectionType"].ToString()),
                        Result = reader["Result"].ToString(),
                    });
                }
            }

            return betBuilderDetail;
        }

        private static IList<Sportsbook> ReadSportbookMatches(IDataReader reader, ResultPopupMetaModel metaModel = null, IEnumerable<int> specialLeagues = null)
        {
            var results = new List<Sportsbook>();

            while (reader.Read())
            {
                var leagueId = reader.ToInt(LeagueIdColumn);

                if (specialLeagues == null || !specialLeagues.Contains(leagueId))
                {
                    var result = ReadSportsbook(reader, leagueId: leagueId);

                    if (metaModel != null)
                    {
                        result.BetID = IsFastMarket(metaModel.Bettype) ? metaModel.Handicap1.ToString() : (metaModel.BetId ?? 0).ToString();
                        result.Bettype = metaModel.Bettype;
                        result.Handicap2 = metaModel.Handicap2;
                    }
                    else
                    {
                        result.BetID = "0";
                    }

                    results.Add(result);
                }
            }

            var gameDetails = GetGameDetails(reader);
            if (gameDetails.Count > 0)
            {
                foreach (var result in results)
                {
                    result.GameDetails = gameDetails.Where(i => i.MatchId == result.MatchId).ToList();
                }
            }

            return results;
        }

        private static bool IsFastMarket(int bettype)
        {
            const int FastMarketMinBetType = 221;
            const int FastMarketMaxBetType = 227;
            return bettype >= FastMarketMinBetType && bettype <= FastMarketMaxBetType;
        }

        private static IList<Racing> ReadVirtualSportMatches(IDataReader reader)
        {
            var dataList = new List<Racing>();

            while (reader.Read())
            {
                dataList.Add(
                                new Racing
                                {
                                    LeagueId = reader.ToInt("meetingid"),
                                    EngineId = reader.ToString("HorseId"),
                                    EngineNo = reader.ToString("HorseNo"),
                                    IsFavor = reader[IsFavorColumn] != DBNull.Value && reader.ToString(IsFavorColumn).Equals("1"),
                                    IsSCR = reader["IsSCR"] != DBNull.Value && reader.ToString("IsSCR").Equals("1"),
                                    Place = reader.ToInt("place"),
                                    PlaceOdds = reader.ToDecimal("placeodds"),
                                    WinOdds = reader.ToDecimal("odds"),
                                    RaceName = (int)reader.ToNullableInt("racename", 0),
                                    KickOffTime = reader.ToDateTime(KickOffTimeColumn),
                                    Favourite = reader[IsFavorColumn] != DBNull.Value ? reader.ToString(IsFavorColumn) : string.Empty,
                                    RowCount = reader.ToInt("rowcount"),
                                    SCROfMatch = reader.ToString("SCR"),
                                    TFOfMatch = reader.ToString("TF"),
                                    Status = reader.HasColumn(Status) ? reader[Status]?.ToString() : null
                                });
            }

            return dataList;
        }

        private static IList<VirtualRacing> ReadBetradarSportMatches(IDataReader reader)
        {
            var dataList = new List<VirtualRacing>();

            while (reader.Read())
            {
                dataList.Add(new VirtualRacing
                {
                    LeagueId = reader.ToInt("LeagueID"),
                    EventId = reader.ToInt("EventID"),
                    VendorMatchId = reader.ToInt("VendorMatchId"),
                    EventTime = reader.ToDateTime("EventTime"),
                    EventStatus = reader.ToString("EventStatus"),
                    BetStatus = reader.ToString("BetStatus"),
                    Distance = reader.ToString("Distance"),
                    Details = new List<VirtualRacingDetail>()
                });
            }

            var dataDetails = GetVirtualRacingDetails(reader);

            if (dataDetails.Count > 0)
            {
                foreach (var data in dataList)
                {
                    data.Details = dataDetails.Where(_ => _.EventId == data.EventId).ToList();
                }
            }

            return dataList;
        }

        private static IList<VirtualRacingDetail> GetVirtualRacingDetails(IDataReader reader)
        {
            var gameDetails = new List<VirtualRacingDetail>();

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    gameDetails.Add(new VirtualRacingDetail
                    {
                        Orid = reader.ToInt("Orid"),
                        EventId = reader.ToInt("EventID"),
                        Placing = reader.ToInt("Placing"),
                        Lane = reader.ToString("Lane"),
                        RacerId = reader.ToInt("RacerID")
                    });
                }
            }

            return gameDetails;
        }


        public async Task<IList<T>> GetPopUpMatches3rdAsync<T>(int matchId)
        {
            const string UspName = "Lrf_BetList_Match3rdResultSel";
            var uspParams = new
            {
                matchId
            };

            using (var db = GetObjectDb(UspName))
            {
                using (var reader = await db.ExecuteReaderAsync(uspParams))
                {
                    var type = typeof(T);

                    if (type == typeof(Bitcoin))
                    {
                        return (IList<T>)ReadBitCoinMatches(reader);
                    }
                    else if (type == typeof(Sportsbook))
                    {
                        return (IList<T>)ReadSportbookMatches3rd(reader);
                    }
                    else
                    {
                        throw new NotSupportedException($"The type '{type.FullName}' is not supported.");
                    }
                }
            }
        }

        private static IList<Bitcoin> ReadBitCoinMatches(IDataReader reader)
        {
            var results = new List<Bitcoin>();

            while (reader.Read())
            {
                results.Add(
                        new Bitcoin
                        {
                            League3rdId = reader.ToInt("League3rdId"),
                            Match3rdId = reader.ToInt("Match3rdId"),
                            BetStatus = reader[BetstatusColumn] != DBNull.Value ? reader.ToString(BetstatusColumn) : string.Empty,
                            EventStatus = reader.ToString("EventStatus"),
                            EventDate = reader.ToDateTime("EventDate")
                        });
            }

            var gameDetails = GetBitCoinGameDetails(reader);
            if (gameDetails.Count > 0)
            {
                foreach (var result in results)
                {
                    result.GameResults = gameDetails.Where(i => i.Match3rdId == result.Match3rdId).ToList();
                }
            }

            return results.OrderBy(x => x.League3rdId).ThenBy(x => x.Match3rdId).ToList();
        }

        private static IList<Fanex.Result.Core.Bitcoin.DTO.GameDetail> GetBitCoinGameDetails(IDataReader reader)
        {
            var gameDetails = new List<Fanex.Result.Core.Bitcoin.DTO.GameDetail>();

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    gameDetails.Add(new Fanex.Result.Core.Bitcoin.DTO.GameDetail
                    {
                        Match3rdId = int.Parse(reader["Match3rdId"].ToString()),
                        GameResult = reader["GameResult"].ToString()
                    });
                }
            }

            return gameDetails;
        }

        private static IList<Sportsbook> ReadSportbookMatches3rd(IDataReader reader)
        {
            var results = new List<Sportsbook>();

            while (reader.Read())
            {
                var result = ReadSportsbook3rd(reader);
                results.Add(result);
            }

            var gameDetails = GetGame3rdDetails(reader);
            if (gameDetails.Count > 0)
            {
                foreach (var result in results)
                {
                    result.GameDetails = gameDetails.Where(i => i.MatchId == result.MatchId).ToList();
                }
            }

            return results;
        }

        private static IList<Fanex.Result.Core.Sportsbook.DTO.GameDetail> GetGame3rdDetails(IDataReader reader)
        {
            var gameDetails = new List<Fanex.Result.Core.Sportsbook.DTO.GameDetail>();

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    gameDetails.Add(new Fanex.Result.Core.Sportsbook.DTO.GameDetail
                    {
                        MatchId = int.Parse(reader["Match3rdId"].ToString()),
                        HomeValue = reader["Custom1"].ToString(),
                        AwayValue = reader["Custom2"].ToString(),
                        SettlementStatusID = int.Parse(reader["SettlementStatusID"].ToString()),
                        ResourceId = reader["ResourceId"].ToString()
                    });
                }
            }

            return gameDetails;
        }

        #endregion Binding
    }
}