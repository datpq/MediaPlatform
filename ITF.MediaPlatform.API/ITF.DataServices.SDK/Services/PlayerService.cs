using ITF.DataServices.SDK.Data;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models.ViewModels;
using ITF.DataServices.SDK.Models.Cms;
using ITF.DataServices.SDK.Models;
using Ninject;
using System.Linq;
using AutoMapper;
using NLog;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Configuration;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.SDK.Models.Cup;
using ITF.DataServices.SDK.Models.ViewModels.Btd;

namespace ITF.DataServices.SDK.Services
{
    public class PlayerService : CupService, IPlayerService
    {
        protected readonly IDataRepository ItfOnlineRepo;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public PlayerService(
            [Named("DavisCupRepo")] ICupDataRepository davisCupRepo,
            [Named("FedCupRepo")] ICupDataRepository fedCupRepo,
            [Named("CmsRepo")] ISameStructureDataRepository cmsRepo,
            [Named("ItfOnlineRepo")] IDataRepository itfOnlineRepo,
            ICacheConfigurationService cacheConfigurationService)
            : base(davisCupRepo, fedCupRepo, cmsRepo, cacheConfigurationService)
        {
            ItfOnlineRepo = itfOnlineRepo;
        }

        public ICollection<PlayerViewModelCoreOld> GetFeaturedPlayers(int cmsNodeId, Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{cmsNodeId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PlayerViewModelCoreOld>;
                    if (cacheValue != null) return cacheValue;
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var cupRepo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var featuredPlayers =
                (from relationship in CmsRepo.GetMany<ITFRelationships>(rs => rs.assetId == cmsNodeId)
                 join player in cupRepo.GetAll<Player>() on relationship.relatedAssetId equals player.PlayerID
                 join nationTranslated in cupRepo.GetAll<NationTranslated>(true) on player.NationalityCode equals
                 nationTranslated.NationCode
                 where relationship.relatedAssetTypeId == 26 // player
                 orderby relationship.sortOrder
                 select new PlayerViewModelCoreOld
                 {
                     PlayerId = player.DataExchangePlayerId ?? 0,
                     GivenName = player.GivenName,
                     FamilyName = player.FamilyName,
                     Gender = player.Gender,
                     NationCode = player.NationalityCode,
                     NationName = nationTranslated.GetNationByLanguage(language),
                     BirthDate = player.BirthDate?.FormatLongByLanguage(language),
                     HeadshotImgId = GetHeadshotImgId(player.PlayerID)
                 }).ToList();

                featuredPlayers.ForEach(x => x.HeadshotUrl = $"/media/{x.HeadshotImgId}/{x.HeadshotImgId}_headshot.jpg");

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, featuredPlayers, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return featuredPlayers;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public ICollection<PlayerViewModelCoreOld> GetCommitmentAwardEligiblePlayers(Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PlayerViewModelCoreOld>;
                    if (cacheValue != null) return cacheValue;
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var cupRepo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var result =
                (from player in cupRepo.GetMany<PlayerTeamCompetitionParticipationSummary>(
                        x => x.IsEligibleForCommitmentAward ?? false)
                 join nt in cupRepo.GetAll<NationTranslated>(true) on player.RepresentingNationCode equals nt.NationCode
                 select new PlayerViewModelCoreOld
                 {
                     BirthDate = player.BirthDate?.FormatLongByLanguage(language),
                     FamilyName = player.FamilyName,
                     GivenName = player.GivenName,
                     Gender = source == DataSource.Dc ? "M" : "F",
                     NationCode = player.RepresentingNationCode,
                     NationName = nt.GetNationByLanguage(language),
                     PlayerId = player.DataExchangePlayerId ?? 0
                 }).ToList();

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public ICollection<PlayerViewModelCoreOld> GetCommitmentAwardOneTieToPlayPlayers(Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PlayerViewModelCoreOld>;
                    if (cacheValue != null) return cacheValue;
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var homeAndAwayAppearances = source == DataSource.Dc
                    ? int.Parse(ConfigurationManager.AppSettings["Cup.HomeAndAwayAppearances.Dc"] ?? "19")
                    : int.Parse(ConfigurationManager.AppSettings["Cup.HomeAndAwayAppearances.Fc"] ?? "19");
                var totalAppearances = source == DataSource.Dc
                    ? int.Parse(ConfigurationManager.AppSettings["Cup.TotalAppearances.Dc"] ?? "49")
                    : int.Parse(ConfigurationManager.AppSettings["Cup.TotalAppearances.Fc"] ?? "39");

                var cupRepo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var result =
                (from playerTcps in cupRepo.GetMany<PlayerTeamCompetitionParticipationSummary>(
                        x => !(x.IsEligibleForCommitmentAward ?? false) && (x.IsPlayerActive ?? false)
                             && ((x.HomeAndAwayAppearances ?? 0) == homeAndAwayAppearances ||
                                 (x.TotalAppearances ?? 0) == totalAppearances))
                    join player in cupRepo.GetAll<Player>() on playerTcps.PlayerId equals player.PlayerID
                    join nt in cupRepo.GetAll<NationTranslated>(true) on playerTcps.RepresentingNationCode equals nt.NationCode
                    select new PlayerViewModelCoreOld
                    {
                        BirthDate = player.BirthDate?.FormatLongByLanguage(language),
                        FamilyName = player.FamilyName,
                        GivenName = player.GivenName,
                        Gender = player.Gender,
                        NationCode = playerTcps.RepresentingNationCode,
                        NationName = nt.GetNationByLanguage(language),
                        PlayerId = player.DataExchangePlayerId ?? 0
                    }).ToList();

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public PlayerViewModel GetPlayer(int playerId, Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{playerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as PlayerViewModel;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var playerExternal = repo.Get<PlayerExternal>(x => x.PlayerExternalID == playerId);
                if (playerExternal == null)
                {
                    Logger.Warn($"Player not found: {playerId}");
                    return null;
                }

                var internalPlayerId = playerExternal.PlayerID;

                var player =
                    (from pl in repo.GetMany<Player>(x => x.PlayerID == internalPlayerId)
                     from pb in repo.GetManySpecific<PlayerBiographyCup>(x => x.PlayerID == internalPlayerId)
                     let pt = repo.Get<PlayerTeamCompetitionParticipationSummary>(x => x.PlayerId == internalPlayerId)
                     let nt = repo.Get<NationTranslated>(x => x.NationCode == pb.NationalityCode, true)
                     select new
                     {
                         PlayerInternalId = pl.PlayerID,
                         PlayerId = playerId,
                         pl.GivenName,
                         pl.FamilyName,
                         pl.Gender,
                         NationCode = pl.NationalityCode,
                         pl.NationalityDesc,
                         NationName = nt?.GetNationByLanguage(language),
                         NationNameEN = nt?.GetNationByLanguage(Language.En),
                         NationNameES = nt?.GetNationByLanguage(Language.Es),
                         BirthDate = pl.BirthDate?.FormatLongByLanguage(language),
                         BirthDateEN = pl.BirthDate?.FormatLongByLanguage(Language.En),
                         BirthDateES = pl.BirthDate?.FormatLongByLanguage(Language.Es),
                         pb.BirthPlace,
                         pb.Residence,
                         Plays = pb.TennisHands,
                         RankSingles = pb.RankCurrentSinglesRollover,
                         RankDoubles = pb.RankCurrentDoublesRollover,
                         TotalNominations = pb.TotalCareerCupWeeks,
                         SinglesWin = pb.WinLossCareerSinglesCupWin,
                         SinglesLoss = pb.WinLossCareerSinglesCupLoss,
                         DoublesWin = pb.WinLossCareerDoublesCupWin,
                         DoublesLoss = pb.WinLossCareerDoublesCupLoss,
                         HeadshotImgId = GetHeadshotImgId(internalPlayerId),
                         CommitmentAward = pt?.IsEligibleForCommitmentAward
                     }).FirstOrDefault();
                if (player == null)
                {
                    Logger.Warn($"Player not found: {playerId}");
                    return null;
                }

                var result = Mapper.Map<PlayerViewModel>(player);
                result.HeadshotUrl = $"/media/{result.HeadshotImgId}/{result.HeadshotImgId}_headshot.jpg";

                var winS = player.SinglesWin ?? 0;
                var lossS = player.SinglesLoss ?? 0;
                var winD = player.DoublesWin ?? 0;
                var lossD = player.DoublesLoss ?? 0;

                result.WLSingles = (winS == 0 && lossS == 0) ? "-" : $"{winS}/{lossS}";
                result.WLDoubles = (winD == 0 && lossD == 0) ? "-" : $"{winD}/{lossD}";
                result.WLTotal = (winS == 0 && lossS == 0 && winD == 0 && lossD == 0) ? "-" : $"{winS + winD}/{lossS + lossD}";

                var yp = GetYearPlayed(source, internalPlayerId);
                result.FirstYearPlayed = yp.FirstYearPlayed;
                result.TiesPlayed = yp.TiesPlayed;

                //TODO
                //playerViewModel.DCCAward = 

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        /// <summary>
        /// Heavy function, CacheTime
        /// </summary>
        /// <param name="source"></param>
        /// <param name="internalPlayerId"></param>
        /// <returns></returns>
        private PlayerViewModel GetYearPlayed(DataSource source, int internalPlayerId)
        {
            var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
            var cacheTimeout = GetCacheTimeout(cachePrefix);
            var cacheKey = $"{cachePrefix}.{source}.{internalPlayerId}";
            var cacheValue = MemoryCache.Get(cacheKey) as PlayerViewModel;
            if (cacheValue != null)
            {
                return cacheValue;
            }
            var stopWatch = Stopwatch.StartNew();

            var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;
            var pams = repo.GetMany<PlayerActivityMatch>(x => x.PlayerID == internalPlayerId).ToList();
            var tieIds = pams.Select(x => x.TieID);
            var playerActivityMatches = (from pam in pams
                                         join tie in repo.GetMany<Tie>(x => tieIds.Contains(x.TieID)) on pam.TieID equals tie.TieID
                                         where pam.ResultCode.SqlNotEquals(RcN)
                                         orderby pam.EndDate descending, pam.TieRoundNumber descending//, pam.TieID, pam.RoundNumber
                                         select new
                                         {
                                             pam.TieID,
                                             TieResult = pam.TieResultShort,
                                             TieStartDate = pam.StartDate
                                         }).Distinct().ToList();

            var result = new PlayerViewModel
            {
                FirstYearPlayed = "NP",
                TiesPlayed = 0
            };

            if (playerActivityMatches.Any())
            {
                result.FirstYearPlayed = playerActivityMatches.Last().TieStartDate.Year.ToString();
                result.TiesPlayed = playerActivityMatches.Count(x => !string.IsNullOrEmpty(x.TieResult));
            }

            stopWatch.Stop();
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
            }

            MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

            return result;
        }

        public ICollection<PlayerViewModelCore> GetPlayersFromTie(string publicTieId, Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{publicTieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PlayerViewModelCore>;
                    if (cacheValue != null) return cacheValue;
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var cupRepo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var nationSquads = cupRepo.GetMany<NationSquad>(x => x.PublicTieId == publicTieId);
                var playerIds = new List<int>();

                nationSquads.ToList().ForEach(nationSquad =>
                {
                    if (nationSquad.Player1ID.HasValue) playerIds.Add(nationSquad.Player1ID.Value);
                    if (nationSquad.Player2ID.HasValue) playerIds.Add(nationSquad.Player2ID.Value);
                    if (nationSquad.Player3ID.HasValue) playerIds.Add(nationSquad.Player3ID.Value);
                    if (nationSquad.Player4ID.HasValue) playerIds.Add(nationSquad.Player4ID.Value);
                    if (nationSquad.Player5ID.HasValue) playerIds.Add(nationSquad.Player5ID.Value);
                });

                var result = (from playerId in playerIds
                    from player in cupRepo.GetMany<Player>(x => x.PlayerID == playerId)
                    join nt in cupRepo.GetAll<NationTranslated>(true) on player.NationalityCode equals nt.NationCode
                    select new PlayerViewModelCore()
                    {
                        PlayerId = player.DataExchangePlayerId ?? 0,
                        GivenName = player.GivenName,
                        FamilyName = player.FamilyName,
                        Gender = player.Gender,
                        NationCode = player.NationalityCode,
                        NationName = nt.GetNationByLanguage(language),
                        BirthDate = player.BirthDate?.FormatLongByLanguage(language),
                        HeadshotImgId = GetHeadshotImgId(player.PlayerID)
                    }).ToList();

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public PlayerActivityViewModelOld GetPlayerActivity(int playerId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{playerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey);
                    if (cacheValue != null)
                    {
                        return cacheValue as PlayerActivityViewModelOld;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var playerExternal = repo.Get<PlayerExternal>(x => x.PlayerExternalID == playerId);
                if (playerExternal == null)
                {
                    Logger.Warn("Player not found: {0}", playerId);
                    return null;
                }

                var internalPlayerId = playerExternal.PlayerID;
                var pams = repo.GetMany<PlayerActivityMatch>(x => x.PlayerID == internalPlayerId).Where(x => x.ResultCode.SqlNotEquals(RcN)).ToList();
                var tieIds = pams.Select(x => x.TieID).Distinct();

                var lResult = (from pam in pams
                               join tie in repo.GetMany<Tie>(x => tieIds.Contains(x.TieID)) on pam.TieID equals tie.TieID
                               orderby pam.EndDate descending, pam.TieRoundNumber descending, pam.TieID, pam.RoundNumber
                               group new
                               {
                                   pam.ResultCode,
                                   pam.MatchTypeCode,
                                   pam.SurfaceCode,
                                   pam.IndoorOutdoorFlag,
                                   pam.WinCount,
                                   pam.LossCount,

                                   Ties = new
                                   {
                                       TieId = pam.PublicTieId,
                                       TieResult = pam.TieResultShort,
                                       TieYear = pam.Year,
                                       EventDivision = pam.EventDivisionCode,
                                       EventZone = pam.EventZoneCode,
                                       EventSubGroup = pam.EventSubGroupCode,
                                       EventClass = pam.EventClassificationCode,
                                       EventDrawStructure = pam.EventDrawsheetStructureCode,
                                       EventRound = pam.TieRoundDesc,
                                       TieDate = $"{pam.StartDate.FormatShortByLanguage(language)} - {pam.EndDate.FormatMediumByLanguage(language)}",
                                       TieDateEN = $"{pam.StartDate.FormatShortByLanguage(Language.En)} - {pam.EndDate.FormatMediumByLanguage(Language.En)}",
                                       TieDateES = $"{pam.StartDate.FormatShortByLanguage(Language.Es)} - {pam.EndDate.FormatMediumByLanguage(Language.Es)}",
                                       TieVenue = pam.Venue,
                                       pam.SurfaceCode,
                                       IndoorOutdoor = pam.IndoorOutdoorFlag,
                                       Side1NationCode = (pam.EventDrawsheetStructureCode.SqlNotEquals(DcR) && pam.HostNationCode == tie.Side2NationCode) ? tie.Side2NationCode : tie.Side1NationCode,
                                       Side1NationName = (pam.EventDrawsheetStructureCode.SqlNotEquals(DcR) && pam.HostNationCode == tie.Side2NationCode) ? tie.Side2NationName : tie.Side1NationName,
                                       Side1Score = (pam.EventDrawsheetStructureCode.SqlNotEquals(DcR) && pam.HostNationCode == tie.Side2NationCode) ? tie.Side2Score : tie.Side1Score,
                                       Side2NationCode = (pam.EventDrawsheetStructureCode.SqlNotEquals(DcR) && pam.HostNationCode == tie.Side2NationCode) ? tie.Side1NationCode : tie.Side2NationCode,
                                       Side2NationName = (pam.EventDrawsheetStructureCode.SqlNotEquals(DcR) && pam.HostNationCode == tie.Side2NationCode) ? tie.Side1NationName : tie.Side2NationName,
                                       Side2Score = (pam.EventDrawsheetStructureCode.SqlNotEquals(DcR) && pam.HostNationCode == tie.Side2NationCode) ? tie.Side1Score : tie.Side2Score,
                                       pam.HostNationCode,
                                       pam.PlayStatusCode
                                   },

                                   Matches = new
                                   {
                                       PlayerId = pam.DataExchangePlayerId,
                                       pam.PlayerGivenName,
                                       pam.PlayerFamilyName,
                                       PlayerNationCode = pam.PlayerNationalityCode,
                                       PartnerPlayerId = pam.PartnerDataExchangePlayerId,
                                       PartnerGivenName = pam.PartnerPlayerGivenName,
                                       PartnerFamilyName = pam.PartnerPlayerFamilyName,
                                       PartnerNationCode = pam.PartnerPlayerNationalityCode,
                                       OpponentPlayerId = pam.OpponentDataExchangePlayerId,
                                       OpponentGivenName = pam.OpponentPlayerGivenName,
                                       OpponentFamilyName = pam.OpponentPlayerFamilyName,
                                       OpponentNationCode = pam.OpponentPlayerNationalityCode,
                                       OpponentPartnerPlayerId = pam.OpponentPartnerDataExchangePlayerId,
                                       OpponentPartnerGivenName = pam.OpponentPartnerPlayerGivenName,
                                       OpponentPartnerFamilyName = pam.OpponentPartnerPlayerFamilyName,
                                       OpponentPartnerNationCode = pam.OpponentPartnerPlayerNationalityCode,
                                       RubberNumber = pam.RoundNumber,
                                       MatchType = pam.MatchTypeCode,
                                       pam.ResultCode,
                                       pam.Score,
                                       pam.ScoreSet1Player,
                                       pam.ScoreSet1Opponent,
                                       ScoreSet1TBPlayer = GetTieBreakScore(pam.ScoreSet1LosingTieBreak, pam.ScoreSet1Player, pam.ScoreSet1Opponent),
                                       ScoreSet1TBOpponent = GetTieBreakScore(pam.ScoreSet1LosingTieBreak, pam.ScoreSet1Opponent, pam.ScoreSet1Player),
                                       pam.ScoreSet2Player,
                                       pam.ScoreSet2Opponent,
                                       ScoreSet2TBPlayer = GetTieBreakScore(pam.ScoreSet2LosingTieBreak, pam.ScoreSet2Player, pam.ScoreSet2Opponent),
                                       ScoreSet2TBOpponent = GetTieBreakScore(pam.ScoreSet2LosingTieBreak, pam.ScoreSet2Opponent, pam.ScoreSet2Player),
                                       pam.ScoreSet3Player,
                                       pam.ScoreSet3Opponent,
                                       ScoreSet3TBPlayer = GetTieBreakScore(pam.ScoreSet3LosingTieBreak, pam.ScoreSet3Player, pam.ScoreSet3Opponent),
                                       ScoreSet3TBOpponent = GetTieBreakScore(pam.ScoreSet3LosingTieBreak, pam.ScoreSet3Opponent, pam.ScoreSet3Player),
                                       pam.ScoreSet4Player,
                                       pam.ScoreSet4Opponent,
                                       ScoreSet4TBPlayer = GetTieBreakScore(pam.ScoreSet4LosingTieBreak, pam.ScoreSet4Player, pam.ScoreSet4Opponent),
                                       ScoreSet4TBOpponent = GetTieBreakScore(pam.ScoreSet4LosingTieBreak, pam.ScoreSet4Opponent, pam.ScoreSet4Player),
                                       pam.ScoreSet5Player,
                                       pam.ScoreSet5Opponent,
                                       ScoreSet5TBPlayer = GetTieBreakScore(pam.ScoreSet5LosingTieBreak, pam.ScoreSet5Player, pam.ScoreSet5Opponent),
                                       ScoreSet5TBOpponent = GetTieBreakScore(pam.ScoreSet5LosingTieBreak, pam.ScoreSet5Opponent, pam.ScoreSet5Player)
                                   }
                               } by 1 into g
                               select new
                               {
                                   TotalWinTotal = g.Sum(x => x.WinCount),
                                   TotalWinSingles = g.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount),
                                   TotalWinDoubles = g.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount),
                                   TotalLossTotal = g.Sum(x => x.LossCount),
                                   TotalLossSingles = g.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                                   TotalLossDoubles = g.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount),

                                   ClayWinTotal = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.WinCount),
                                   ClayWinSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScC).Sum(x => x.WinCount),
                                   ClayWinDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScC).Sum(x => x.WinCount),
                                   ClayLossTotal = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.LossCount),
                                   ClayLossSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScC).Sum(x => x.LossCount),
                                   ClayLossDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScC).Sum(x => x.LossCount),

                                   HardWinTotal = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.WinCount),
                                   HardWinSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScH).Sum(x => x.WinCount),
                                   HardWinDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScH).Sum(x => x.WinCount),
                                   HardLossTotal = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.LossCount),
                                   HardLossSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScH).Sum(x => x.LossCount),
                                   HardLossDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScH).Sum(x => x.LossCount),

                                   GrassWinTotal = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.WinCount),
                                   GrassWinSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScG).Sum(x => x.WinCount),
                                   GrassWinDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScG).Sum(x => x.WinCount),
                                   GrassLossTotal = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.LossCount),
                                   GrassLossSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScG).Sum(x => x.LossCount),
                                   GrassLossDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScG).Sum(x => x.LossCount),

                                   CarpetWinTotal = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.WinCount),
                                   CarpetWinSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScA).Sum(x => x.WinCount),
                                   CarpetWinDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScA).Sum(x => x.WinCount),
                                   CarpetLossTotal = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.LossCount),
                                   CarpetLossSingles = g.Where(x => x.MatchTypeCode == McS && x.SurfaceCode == ScA).Sum(x => x.LossCount),
                                   CarpetLossDoubles = g.Where(x => x.MatchTypeCode == McD && x.SurfaceCode == ScA).Sum(x => x.LossCount),

                                   UnknownWinTotal = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.WinCount),
                                   UnknownWinSingles = g.Where(x => x.MatchTypeCode == McS && string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.WinCount),
                                   UnknownWinDoubles = g.Where(x => x.MatchTypeCode == McD && string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.WinCount),
                                   UnknownLossTotal = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.LossCount),
                                   UnknownLossSingles = g.Where(x => x.MatchTypeCode == McS && string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.LossCount),
                                   UnknownLossDoubles = g.Where(x => x.MatchTypeCode == McD && string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.LossCount),

                                   IndoorWinTotal = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.WinCount),
                                   IndoorWinSingles = g.Where(x => x.MatchTypeCode == McS && x.IndoorOutdoorFlag == IfI).Sum(x => x.WinCount),
                                   IndoorWinDoubles = g.Where(x => x.MatchTypeCode == McD && x.IndoorOutdoorFlag == IfI).Sum(x => x.WinCount),
                                   IndoorLossTotal = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.LossCount),
                                   IndoorLossSingles = g.Where(x => x.MatchTypeCode == McS && x.IndoorOutdoorFlag == IfI).Sum(x => x.LossCount),
                                   IndoorLossDoubles = g.Where(x => x.MatchTypeCode == McD && x.IndoorOutdoorFlag == IfI).Sum(x => x.LossCount),

                                   OutdoorWinTotal = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.WinCount),
                                   OutdoorWinSingles = g.Where(x => x.MatchTypeCode == McS && x.IndoorOutdoorFlag == IfO).Sum(x => x.WinCount),
                                   OutdoorWinDoubles = g.Where(x => x.MatchTypeCode == McD && x.IndoorOutdoorFlag == IfO).Sum(x => x.WinCount),
                                   OutdoorLossTotal = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.LossCount),
                                   OutdoorLossSingles = g.Where(x => x.MatchTypeCode == McS && x.IndoorOutdoorFlag == IfO).Sum(x => x.LossCount),
                                   OutdoorLossDoubles = g.Where(x => x.MatchTypeCode == McD && x.IndoorOutdoorFlag == IfO).Sum(x => x.LossCount),

                                   Ties = from gt in g.GroupBy(x => x.Ties)
                                              //select new {gt.Key, Matches = from gm in gt.ToList() select gm.Matches }
                                          select new
                                          {
                                              gt.Key.TieId,
                                              gt.Key.TieResult,
                                              gt.Key.TieYear,
                                              gt.Key.EventDivision,
                                              gt.Key.EventZone,
                                              gt.Key.EventSubGroup,
                                              gt.Key.EventClass,
                                              gt.Key.EventDrawStructure,
                                              gt.Key.EventRound,
                                              gt.Key.TieDate,
                                              gt.Key.TieDateEN,
                                              gt.Key.TieDateES,
                                              gt.Key.TieVenue,
                                              gt.Key.SurfaceCode,
                                              gt.Key.IndoorOutdoor,
                                              gt.Key.Side1NationCode,
                                              gt.Key.Side1NationName,
                                              gt.Key.Side1Score,
                                              gt.Key.Side2NationCode,
                                              gt.Key.Side2NationName,
                                              gt.Key.Side2Score,
                                              gt.Key.HostNationCode,
                                              gt.Key.PlayStatusCode,
                                              Matches = from gm in gt.ToList() select gm.Matches
                                          }
                               }).FirstOrDefault();

                var result = Mapper.Map<PlayerActivityViewModelOld>(lResult);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.SetNullable(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public HeadToHeadPlayerToPlayerViewModel GetHeadToHeadPlayerToPlayer(int playerId, int opponentPlayerId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{playerId}.{opponentPlayerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as HeadToHeadPlayerToPlayerViewModel;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => x.DataExchangePlayerId == playerId
                               && (x.OpponentDataExchangePlayerId == opponentPlayerId || x.OpponentPartnerDataExchangePlayerId == opponentPlayerId) && x.MatchTypeCode.Equals("S"))
                                   //join tie in repo.GetAll<Tie>() on pam.TieID equals tie.TieID
                               from tie in repo.GetMany<Tie>(x => x.TieID == pam.TieID)
                               where pam.ResultCode.SqlNotEquals(RcN)
                               orderby pam.EndDate descending, pam.TieRoundNumber descending, pam.TieID, pam.RoundNumber
                               group new
                               {
                                   pam.WinCount,
                                   pam.LossCount,
                                   pam.MatchTypeCode,
                                   pam.SurfaceCode,
                                   pam.IndoorOutdoorFlag,
                                   PlayerId = playerId,
                                   pam.PlayerGivenName,
                                   pam.PlayerFamilyName,
                                   PlayerNationName = pam.PlayerNationalityName,
                                   PlayerNationCode = pam.PlayerNationalityCode,
                                   OppositionPlayerId = opponentPlayerId,
                                   OppositionPlayerGivenName = pam.OpponentDataExchangePlayerId == opponentPlayerId ? pam.OpponentPlayerGivenName : pam.OpponentPartnerPlayerGivenName,
                                   OppositionPlayerFamilyName = pam.OpponentDataExchangePlayerId == opponentPlayerId ? pam.OpponentPlayerFamilyName : pam.OpponentPartnerPlayerFamilyName,
                                   OppositionPlayerNationName = pam.OpponentDataExchangePlayerId == opponentPlayerId ? pam.OpponentPlayerNationalityName : pam.OpponentPartnerPlayerNationalityName,
                                   OppositionPlayerNationCode = pam.OpponentDataExchangePlayerId == opponentPlayerId ? pam.OpponentPlayerNationalityCode : pam.OpponentPartnerPlayerNationalityCode,
                                   Ties = new
                                   {
                                       tie.PublicTieId,
                                       ResultDesc = pam.TieResultShort,
                                       Result = pam.ResultCode,
                                       pam.Venue,
                                       pam.Year,
                                       StartDate = tie.StartDate?.ToString(StartDateFormat),
                                       EndDate = tie.EndDate?.ToString(StartDateFormat),
                                       pam.SurfaceCode,
                                       IndoorOutdoorCode = pam.IndoorOutdoorFlag,
                                       Group = pam.EventDivisionCode,
                                       Zone = pam.EventZoneCode,
                                       DrawType = pam.EventDrawsheetStructureCode,
                                       DrawClass = pam.EventClassificationCode,
                                       Round = pam.TieRoundDesc,
                                       SubGroupCode = pam.EventSubGroupCode,
                                       tie.PlayStatusCode
                                   },
                                   Rubbers = new
                                   {
                                       RubberNumber = pam.RoundNumber,
                                       pam.ResultCode,
                                       pam.Score,
                                       pam.MatchTypeCode,
                                       PartnerPlayerId = pam.PartnerDataExchangePlayerId,
                                       pam.PartnerPlayerGivenName,
                                       pam.PartnerPlayerFamilyName,
                                       PartnerPlayerNationCode = pam.PartnerPlayerNationalityCode,
                                       OppositionPlayerId = pam.OpponentDataExchangePlayerId,
                                       OppositionPlayerGivenName = pam.OpponentPlayerGivenName,
                                       OppositionPlayerFamilyName = pam.OpponentPlayerFamilyName,
                                       OppositionPlayerNationCode = pam.OpponentPlayerNationalityCode,
                                       OppositionPartnerPlayerId = pam.OpponentPartnerDataExchangePlayerId,
                                       OppositionPartnerPlayerGivenName = pam.OpponentPartnerPlayerGivenName,
                                       OppositionPartnerPlayerFamilyName = pam.OpponentPartnerPlayerFamilyName,
                                       OppositionPartnerPlayerNationCode = pam.OpponentPartnerPlayerNationalityCode,

                                       ScoreSet1Side1 = pam.ScoreSet1Player,
                                       ScoreSet1Side2 = pam.ScoreSet1Opponent,
                                       ScoreSet1LosingTB = pam.ScoreSet1LosingTieBreak,
                                       ScoreSet2Side1 = pam.ScoreSet2Player,
                                       ScoreSet2Side2 = pam.ScoreSet2Opponent,
                                       ScoreSet2LosingTB = pam.ScoreSet2LosingTieBreak,
                                       ScoreSet3Side1 = pam.ScoreSet3Player,
                                       ScoreSet3Side2 = pam.ScoreSet3Opponent,
                                       ScoreSet3LosingTB = pam.ScoreSet3LosingTieBreak,
                                       ScoreSet4Side1 = pam.ScoreSet4Player,
                                       ScoreSet4Side2 = pam.ScoreSet4LosingTieBreak,
                                       ScoreSet4LosingTB = pam.ScoreSet4LosingTieBreak,
                                       ScoreSet5Side1 = pam.ScoreSet5Player,
                                       ScoreSet5Side2 = pam.ScoreSet5Opponent,
                                       ScoreSet5LosingTB = pam.ScoreSet5LosingTieBreak
                                   }
                               } by 1 into g
                               from gl in g.ToList()
                               select new
                               {
                                   gl.PlayerId,
                                   gl.PlayerGivenName,
                                   gl.PlayerFamilyName,
                                   gl.PlayerNationName,
                                   gl.PlayerNationCode,
                                   gl.OppositionPlayerId,
                                   gl.OppositionPlayerGivenName,
                                   gl.OppositionPlayerFamilyName,
                                   gl.OppositionPlayerNationName,
                                   gl.OppositionPlayerNationCode,

                                   WinTotal = g.Sum(x => x.WinCount),
                                   WinSingles = g.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount),
                                   WinDoubles = g.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount),
                                   LossTotal = g.Sum(x => x.LossCount),
                                   LossSingles = g.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                                   LossDoubles = g.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount),

                                   WinClay = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.WinCount),
                                   LossClay = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.LossCount),

                                   WinHard = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.WinCount),
                                   LossHard = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.LossCount),

                                   WinGrass = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.WinCount),
                                   LossGrass = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.LossCount),

                                   WinCarpet = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.WinCount),
                                   LossCarpet = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.LossCount),

                                   WinUnknown = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.WinCount),
                                   LossUnknown = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.LossCount),

                                   WinIndoor = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.WinCount),
                                   LossIndoor = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.LossCount),

                                   WinOutdoor = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.WinCount),
                                   LossOutdoor = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.LossCount),

                                   Ties = from gt in g.GroupBy(x => x.Ties)
                                          select new
                                          {
                                              gt.Key.PublicTieId,
                                              gt.Key.ResultDesc,
                                              gt.Key.Result,
                                              gt.Key.Venue,
                                              gt.Key.Year,
                                              gt.Key.StartDate,
                                              gt.Key.EndDate,
                                              gt.Key.SurfaceCode,
                                              gt.Key.IndoorOutdoorCode,
                                              gt.Key.Group,
                                              gt.Key.Zone,
                                              gt.Key.DrawType,
                                              gt.Key.DrawClass,
                                              gt.Key.Round,
                                              gt.Key.SubGroupCode,
                                              gt.Key.PlayStatusCode,

                                              Rubbers = from gm in gt.ToList() select gm.Rubbers
                                          }
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"No Head2Head found between {playerId} and {opponentPlayerId}");
                    return null;
                }

                var result = Mapper.Map<HeadToHeadPlayerToPlayerViewModel>(lResult);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public HeadToHeadPlayerToNationViewModel GetHeadToHeadPlayerToNation(int playerId, string opponentNationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc)
                    throw new ArgumentException($"source '{source}' is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{playerId}.{opponentNationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as HeadToHeadPlayerToNationViewModel;
                    if (cacheValue != null) return cacheValue;
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled) Logger.Debug($"{cacheKey}, useCache={useCache}");

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == opponentNationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (!lstNationCodes.Any())
                {
                    Logger.Warn($"Nation not found: {opponentNationCode}");
                    return null;
                }

                var ntOpponent = repo.Get<NationTranslated>(x => x.NationCode == opponentNationCode, true);


                var lResult =
                    (from pam in repo.GetMany<PlayerActivityMatch>(
                        x => x.DataExchangePlayerId == playerId
                             && (!opponentNationCode.Equals(Srb) || x.StartDate.Year >= SrbYear)
                             && (lstNationCodes.Contains(x.OpponentPlayerNationalityCode) ||
                                 lstNationCodes.Contains(x.OpponentPartnerPlayerNationalityCode))
                         && new[] { PsPc, "PU", "PA" }.Contains(x.PlayStatusCode))
                     from tie in repo.GetMany<Tie>(x => x.TieID == pam.TieID)
                     where pam.ResultCode.SqlNotEquals(RcN)
                     orderby pam.EndDate descending, pam.TieRoundCode descending, pam.TieID, pam.RoundNumber
                    group new
                    {
                        pam.WinCount,
                        pam.LossCount,
                        pam.MatchTypeCode,
                        pam.SurfaceCode,
                        pam.IndoorOutdoorFlag,
                        PlayerId = playerId,
                        pam.PlayerGivenName,
                        pam.PlayerFamilyName,
                        PlayerNationName = pam.PlayerNationalityName,
                        PlayerNationCode = pam.PlayerNationalityCode,
                        OppositionNationCode = opponentNationCode.ToUpper(),
                        OppositionNationName = ntOpponent.GetNationByLanguage(language),
                        OppositionNationNameES = ntOpponent.GetNationByLanguage(Language.Es),
                        Ties = new
                        {
                            tie.PublicTieId,
                            ResultDesc = pam.TieResultShort,
                            Result = pam.ResultCode,
                            pam.Venue,
                            pam.Year,
                            StartDate = tie.StartDate?.ToString(StartDateFormat),
                            EndDate = tie.EndDate?.ToString(StartDateFormat),
                            pam.SurfaceCode,
                            IndoorOutdoorCode = pam.IndoorOutdoorFlag,
                            Group = pam.EventDivisionCode,
                            Zone = pam.EventZoneCode,
                            DrawType = pam.EventDrawsheetStructureCode,
                            DrawClass = pam.EventClassificationCode,
                            Round = pam.TieRoundDesc,
                            SubGroupCode = pam.EventSubGroupCode,
                            tie.PlayStatusCode
                        },
                        Rubbers = new
                        {
                            RubberNumber = pam.RoundNumber,
                            pam.ResultCode,
                            pam.Score,
                            pam.MatchTypeCode,
                            PartnerPlayerId = pam.PartnerDataExchangePlayerId,
                            pam.PartnerPlayerGivenName,
                            pam.PartnerPlayerFamilyName,
                            PartnerPlayerNationCode = pam.PartnerPlayerNationalityCode,
                            OppositionPlayerId = pam.OpponentDataExchangePlayerId,
                            OppositionPlayerGivenName = pam.OpponentPlayerGivenName,
                            OppositionPlayerFamilyName = pam.OpponentPlayerFamilyName,
                            OppositionPlayerNationCode = pam.OpponentPlayerNationalityCode,
                            OppositionPartnerPlayerId = pam.OpponentPartnerDataExchangePlayerId,
                            OppositionPartnerPlayerGivenName = pam.OpponentPartnerPlayerGivenName,
                            OppositionPartnerPlayerFamilyName = pam.OpponentPartnerPlayerFamilyName,
                            OppositionPartnerPlayerNationCode = pam.OpponentPartnerPlayerNationalityCode,

                            ScoreSet1Side1 = pam.ScoreSet1Player,
                            ScoreSet1Side2 = pam.ScoreSet1Opponent,
                            ScoreSet1LosingTB = pam.ScoreSet1LosingTieBreak,
                            ScoreSet2Side1 = pam.ScoreSet2Player,
                            ScoreSet2Side2 = pam.ScoreSet2Opponent,
                            ScoreSet2LosingTB = pam.ScoreSet2LosingTieBreak,
                            ScoreSet3Side1 = pam.ScoreSet3Player,
                            ScoreSet3Side2 = pam.ScoreSet3Opponent,
                            ScoreSet3LosingTB = pam.ScoreSet3LosingTieBreak,
                            ScoreSet4Side1 = pam.ScoreSet4Player,
                            ScoreSet4Side2 = pam.ScoreSet4LosingTieBreak,
                            ScoreSet4LosingTB = pam.ScoreSet4LosingTieBreak,
                            ScoreSet5Side1 = pam.ScoreSet5Player,
                            ScoreSet5Side2 = pam.ScoreSet5Opponent,
                            ScoreSet5LosingTB = pam.ScoreSet5LosingTieBreak
                        }
                    } by 1 into g
                    from gl in g.ToList()
                    select new
                    {
                        gl.PlayerId,
                        gl.PlayerGivenName,
                        gl.PlayerFamilyName,
                        gl.PlayerNationName,
                        gl.PlayerNationCode,
                        gl.OppositionNationCode,
                        gl.OppositionNationName,
                        gl.OppositionNationNameES,

                        WinTotal = g.Sum(x => x.WinCount),
                        WinSingles = g.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount),
                        WinDoubles = g.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount),
                        LossTotal = g.Sum(x => x.LossCount),
                        LossSingles = g.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                        LossDoubles = g.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount),

                        WinClay = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.WinCount),
                        LossClay = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.LossCount),

                        WinHard = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.WinCount),
                        LossHard = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.LossCount),

                        WinGrass = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.WinCount),
                        LossGrass = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.LossCount),

                        WinCarpet = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.WinCount),
                        LossCarpet = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.LossCount),

                        WinUnknown = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.WinCount),
                        LossUnknown = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.LossCount),

                        WinIndoor = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.WinCount),
                        LossIndoor = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.LossCount),

                        WinOutdoor = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.WinCount),
                        LossOutdoor = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.LossCount),

                        Ties = from gt in g.GroupBy(x => x.Ties)
                               select new
                               {
                                   gt.Key.PublicTieId,
                                   gt.Key.ResultDesc,
                                   gt.Key.Result,
                                   gt.Key.Venue,
                                   gt.Key.Year,
                                   gt.Key.StartDate,
                                   gt.Key.EndDate,
                                   gt.Key.SurfaceCode,
                                   gt.Key.IndoorOutdoorCode,
                                   gt.Key.Group,
                                   gt.Key.Zone,
                                   gt.Key.DrawType,
                                   gt.Key.DrawClass,
                                   gt.Key.Round,
                                   gt.Key.SubGroupCode,
                                   gt.Key.PlayStatusCode,

                                   Rubbers = from gm in gt.ToList() select gm.Rubbers
                               }
                    }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"No Head2Head found between {playerId} and {opponentNationCode}");
                    return null;
                }

                var result = Mapper.Map<HeadToHeadPlayerToNationViewModel>(lResult);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public PlayerViewModelCoreOld GetPlayerCore(int playerId, Language language = Language.En, DataSource source = DataSource.Itf, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc && source != DataSource.Itf) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{playerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as PlayerViewModelCoreOld;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = source == DataSource.Itf ? ItfOnlineRepo : (source == DataSource.Dc ? DavisCupRepo : FedCupRepo);

                var playerExternal = repo.Get<PlayerExternal>(x => x.PlayerExternalID == playerId);
                if (playerExternal == null)
                {
                    Logger.Warn($"Player not found: {playerId}");
                    return null;
                }

                var internalPlayerId = playerExternal.PlayerID;
                var lResult = (from pl in repo.GetMany<Player>(x => x.PlayerID == internalPlayerId)
                               join nt in repo.GetAll<NationTranslated>(true) on pl.NationalityCode equals nt.NationCode
                               select new
                               {
                                   PlayerInternalId = pl.PlayerID,
                                   PlayerId = playerId,
                                   pl.GivenName,
                                   pl.FamilyName,
                                   pl.Gender,
                                   NationCode = pl.NationalityCode,
                                   pl.NationalityDesc,
                                   NationName = nt.GetNationByLanguage(language),
                                   BirthDate = pl.BirthDate?.FormatLongByLanguage(language),
                                   HeadshotImgId = GetHeadshotImgId(internalPlayerId)
                               }).FirstOrDefault();

                var result = Mapper.Map<PlayerViewModelCoreOld>(lResult);
                if (result != null)
                {
                    result.HeadshotUrl = $"/media/{result.HeadshotImgId}/{result.HeadshotImgId}_headshot.jpg";
                }

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;

            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public int? GetHeadshotImgId(int internalPlayerId)
        {
            var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
            var cacheTimeout = GetCacheTimeout(cachePrefix);
            var cacheKey = $"{cachePrefix}.{internalPlayerId}";
            var cacheValue = MemoryCache.Get(cacheKey) as int?;
            if (cacheValue != null)
            {
                return cacheValue;
            }
            var stopWatch = Stopwatch.StartNew();

            var repo = CmsRepo;
            var lResult = repo.GetMany<ITFRelationships>(x => x.assetId == internalPlayerId
                                                              && x.AssetType.assetTypeName == "Player"
                                                              && x.RelatedAssetType.assetTypeName == "Image"
                                                              && x.CmsPropertyData.dataInt == 1
                                                              && x.CmsPropertyData.CmsPropertyType.Alias == "playerHeadShot").ToList();
            var result =
                (lResult.Any(x => x.sortOrder == null)
                ? lResult.OrderByDescending(x => x.relationshipId)
                : lResult.OrderBy(x => x.sortOrder)).FirstOrDefault()?.relatedAssetId ?? 0;

            stopWatch.Stop();
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}, result={result}");
            }

            MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

            return result;
        }

        public ICollection<PlayerViewModelCore> SearchPlayers(string searchText, Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{searchText}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PlayerViewModelCore>;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var result = (from player in
                    repo.GetMany<Player>(x => x.GivenName.Contains(searchText) || x.FamilyName.Contains(searchText))
                    join nt in repo.GetAll<NationTranslated>(true) on player.NationalityCode equals nt.NationCode
                    select new PlayerViewModelCore()
                    {
                        PlayerId = player.DataExchangePlayerId ?? 0,
                        GivenName = player.GivenName,
                        FamilyName = player.FamilyName,
                        Gender = player.Gender,
                        NationCode = player.NationalityCode,
                        NationName = nt.GetNationByLanguage(language),
                        BirthDate = player.BirthDate?.FormatLongByLanguage(language),
                        HeadshotImgId = GetHeadshotImgId(player.PlayerID)
                    }).ToList();

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        public static int? GetTieBreakScore(int? scoreLosingTieBreak, int? scorePlayer, int? scoreOpponent)
        {
            var result = scoreLosingTieBreak.HasValue ? (scorePlayer > scoreOpponent ? (scoreLosingTieBreak < 5 ? 7 : scoreLosingTieBreak + 2) : scoreLosingTieBreak) : null;
            return result;
        }

        #region BTD customization

        public BtdPlayerViewModel GetBtdPlayer(int playerId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{playerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as BtdPlayerViewModel;
                    if (cacheValue != null)
                    {
                        return cacheValue;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var playerInfo = Mapper.Map<PlayerViewModelBtd>(GetPlayer(playerId, language, source, useCache));
                if (playerInfo == null) return null;
                var playerActivityInfo = Mapper.Map<PlayerActivityViewModel>(GetPlayerActivity(playerId, language, source, useCache));

                var result = new BtdPlayerViewModel
                {
                    PlayerInfo = playerInfo,
                    PlayerActivityInfo = playerActivityInfo
                };

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        #endregion
    }
}
