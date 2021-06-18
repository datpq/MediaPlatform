using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.SDK.Data;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models;
using ITF.DataServices.SDK.Models.Baseline02;
using ITF.DataServices.SDK.Models.Cms;
using ITF.DataServices.SDK.Models.ItfOnline;
using ITF.DataServices.SDK.Models.ViewModels.Circuits;
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using ITF.DataServices.SDK.Models.ViewModels.LiveBlog;
using ITF.DataServices.SDK.Models.Xml;
using Ninject;
using NLog;
using Event = ITF.DataServices.SDK.Models.Event;
using Tournament = ITF.DataServices.SDK.Models.Tournament;

namespace ITF.DataServices.SDK.Services
{
    public class OlympicService : BaseService, IOlympicService
    {
        private readonly IXmlDataRepository _olympicsXmlOlyRepo;
        private readonly IXmlDataRepository _olympicsXmlPlyRepo;
        private readonly ISameStructureDataRepository _cmsRepo;
        private readonly IDataRepository _itfOnlineRepo;
        private readonly IDataRepository _baseline02Repo;

        public const string TccOl = "OL"; //TourCategoryCode Olympic
        public const string TccPo = "POL"; //TourCategoryCode Paralympic
        public const string TccSl = "SL"; //TourCategoryCode GrandSlam

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OlympicService(
            [Named("OlympicsXmlOlyRepo")] IXmlDataRepository olympicsXmlOlyRepo,
            [Named("OlympicsXmlPlyRepo")] IXmlDataRepository olympicsXmlPlyRepo,
            [Named("CmsRepo")] ISameStructureDataRepository cmsRepo,
            [Named("ItfOnlineRepo")] IDataRepository itfOnlineRepo,
            [Named("Baseline02Repo")] IDataRepository baseline02Repo,
            ICacheConfigurationService cacheConfigurationService) : base(cacheConfigurationService)
        {
            _olympicsXmlOlyRepo = olympicsXmlOlyRepo;
            _olympicsXmlPlyRepo = olympicsXmlPlyRepo;
            _cmsRepo = cmsRepo;
            _itfOnlineRepo = itfOnlineRepo;
            _baseline02Repo = baseline02Repo;
        }

        #region private members

        private IDataRepository GetRepoByTournamentType(string tournamentType)
        {
            return tournamentType == "OLY" ? _olympicsXmlOlyRepo : _olympicsXmlPlyRepo;
        }

        private static string GetTourCategoryCodeByTournamentType(string tournamentType)
        {
            return tournamentType == "OLY" ? TccOl : TccPo;
        }

        private static bool IsOlympics(string tournamentType)
        {
            return tournamentType == "OLY";
        }

        private IEnumerable<BaseOlympics> GetAllOlympicsByTournamentType(string tournamentType)
        {
            return tournamentType == "OLY"
                ? (IEnumerable<BaseOlympics>) _itfOnlineRepo.GetAll<Olympics>()
                : _itfOnlineRepo.GetAll<Paralympics>();
        }

        private Player GetPlayerByOdfOrDataExchangeId(int odfOrDataExchangeId)
        {
            var dataExchangePlayerId = odfOrDataExchangeId;
            if (!odfOrDataExchangeId.ToString().StartsWith("800"))
            {
                var playerLookup = _itfOnlineRepo.Get<ODFPlayerLookup>(x => x.OdfPlayerId == odfOrDataExchangeId);
                if (playerLookup != null)
                {
                    dataExchangePlayerId = playerLookup.DataExchangePlayerId;
                }
            }
            return _itfOnlineRepo.Get<Player>(x => x.DataExchangePlayerId == dataExchangePlayerId);
        }

        #endregion

        #region Player service

        public OlympicsPlayersViewModel GetLatestPlayers(string tournamentType, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as OlympicsPlayersViewModel;
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

                var repo = GetRepoByTournamentType(tournamentType);
                var isOlympics = IsOlympics(tournamentType);

                var olympics = GetAllOlympicsByTournamentType(tournamentType).OrderByDescending(x => x.Year).FirstOrDefault();

                if (olympics == null)
                {
                    Logger.Warn($"Olympics or Paralympics not found: {tournamentType}");
                    return null;
                }

                /*
                var lResult = (from na in repo.GetAll<LatestNation>()
                               join pl in repo.GetAll<LatestPlayer>() on na.CountryCode equals pl.CountryCode
                               group new
                               {
                                   Players = new
                                   {
                                       pl.Doubles,
                                       pl.Gender,
                                       PlayerFamilyName = pl.FamilyName,
                                       PlayerGivenName = pl.GivenName,
                                       PlayerId = pl.Id,
                                       PlayerNationalityCode = na.CountryCode,
                                       PlayerNationalityName = na.NameEN,
                                       pl.Singles,
                                       QuadDoubles = !isOlympics ? pl.QuadDoubles : null,
                                       QuadSingles = !isOlympics ? pl.QuadSingles : null,
                                       MixedDoubles = isOlympics ? pl.MixedDoubles : null,
                                       olympics.Year
                                   }
                               } by 1 into g
                               select new
                               {
                                   Players = from gp in g.ToList() select gp.Players,
                                   olympics.HostCity,
                                   olympics.HostNationCode,
                                   olympics.HostNationName,
                                   olympics.TournamentName,
                                   olympics.Year
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"LatestPlayers not found: {tournamentType}");
                    return null;
                }
                */

                var lResult = new
                {
                    Players = (from na in repo.GetAll<LatestNation>()
                               join pl in repo.GetAll<LatestPlayer>() on na.CountryCode equals pl.CountryCode
                               select new
                               {
                                   pl.Doubles,
                                   pl.Gender,
                                   PlayerFamilyName = pl.FamilyName,
                                   PlayerGivenName = pl.GivenName,
                                   PlayerId = pl.Id,
                                   PlayerNationalityCode = na.CountryCode,
                                   PlayerNationalityName = na.NameEN,
                                   pl.Singles,
                                   QuadDoubles = !isOlympics ? pl.QuadDoubles : null,
                                   QuadSingles = !isOlympics ? pl.QuadSingles : null,
                                   MixedDoubles = isOlympics ? pl.MixedDoubles : null,
                                   olympics.Year
                               }),
                    olympics.HostCity,
                    olympics.HostNationCode,
                    olympics.HostNationName,
                    olympics.TournamentName,
                    olympics.Year
                };

                var result = Mapper.Map<OlympicsPlayersViewModel>(lResult);

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public ICollection<OlympicsPlayersViewModel> GetAllOlympicsPlayers(string tournamentType, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<OlympicsPlayersViewModel>;
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

                var repo = _itfOnlineRepo;
                var isOlympics = IsOlympics(tournamentType);
                var lastYear = GetAllOlympicsByTournamentType(tournamentType).OrderByDescending(x => x.Year).FirstOrDefault()?.Year ?? 2016;
                var tourCategoryCode = GetTourCategoryCodeByTournamentType(tournamentType);

                var allOlympics = GetAllOlympicsByTournamentType(tournamentType).Where(x => x.Year != lastYear);

                /*
                var lResult = (from ol in allOlympics
                               join pamX in repo.GetMany<PlayerActivityMatch>(x => x.TourCategoryCode == tourCategoryCode) on ol.Year equals pamX.Year
                               join pam in repo.GetMany<PlayerActivityMatch>(
                                   x => x.TourCategoryCode == tourCategoryCode && x.MatchTypeCode == "S") on new {pamX.Year, pamX.PlayerID} equals new { pam.Year, pam.PlayerID } into pamSj
                               from pamS in pamSj.DefaultIfEmpty()
                               join pam in repo.GetMany<PlayerActivityMatch>(
                                   x => x.TourCategoryCode == tourCategoryCode && x.MatchTypeCode == "D") on new { pamX.Year, pamX.PlayerID } equals new { pam.Year, pam.PlayerID } into pamDj
                               from pamD in pamDj.DefaultIfEmpty()
                               join pam in repo.GetMany<PlayerActivityMatch>(
                                   x => x.TourCategoryCode == tourCategoryCode && x.MatchTypeCode == "Mx") on new { pamX.Year, pamX.PlayerID } equals new { pam.Year, pam.PlayerID } into pamMj
                               from pamM in pamMj.DefaultIfEmpty()
                               join ev in repo.GetAll<Event>() on pamX.EventID equals ev.EventID
                               group new
                               {
                                   ol,
                                   Players = new
                                   {
                                       Gender = pamS?.PlayerGender ?? pamD?.PlayerGender ?? pamM?.PlayerGender,
                                       PlayerFamilyName =  pamS?.PlayerFamilyName ?? pamD?.PlayerFamilyName ?? pamM?.PlayerFamilyName,
                                       PlayerGivenName = pamS?.PlayerGivenName ?? pamD?.PlayerGivenName ?? pamM?.PlayerGivenName,
                                       PlayerId = pamS?.DataExchangePlayerId ?? pamD?.DataExchangePlayerId ?? pamM?.DataExchangePlayerId,
                                       PlayerNationalityCode = pamS?.PlayerNationalityCode ?? pamD?.PlayerNationalityCode ?? pamM?.PlayerNationalityCode,
                                       PlayerNationalityName = pamS?.PlayerNationalityName ?? pamD?.PlayerNationalityName ?? pamM?.PlayerNationalityName,
                                       QuadDoubles = !isOlympics && ev.PlayerTypeCode == "Q" ? pamD?.PlayerEntryClassificationCode : null,
                                       Doubles = ((!isOlympics && ev.PlayerTypeCode != "Q") || isOlympics) ? pamD == null ? string.Empty : pamD.PlayerEntryClassificationCode : null,
                                       QuadSingles = !isOlympics && ev.PlayerTypeCode == "Q" ? pamS?.PlayerEntryClassificationCode : null,
                                       Singles = ((!isOlympics && ev.PlayerTypeCode != "Q") || isOlympics) ? pamS == null ? string.Empty : pamS.PlayerEntryClassificationCode : null,
                                       MixedDoubles = isOlympics ? pamM == null ? string.Empty : pamM.PlayerEntryClassificationCode : null,
                                       Year = pamS?.Year ?? pamD?.Year ?? pamM?.Year,
                                   }
                               } by ol into g
                               orderby g.Key.Year
                               select new
                               {
                                   Players = from gp in g.Distinct().ToList() select gp.Players,
                                   g.Key.HostCity,
                                   g.Key.HostNationCode,
                                   g.Key.HostNationName,
                                   g.Key.TournamentName,
                                   g.Key.Year
                               }).ToList();
                               */

                var allPams = (from pam in repo.GetMany<PlayerActivityMatch>(x => x.TourCategoryCode == tourCategoryCode)
                               from ev in repo.GetMany<Event>(x => x.EventID == pam.EventID)
                               select new
                               {
                                   Gender = pam.PlayerGender,
                                   pam.PlayerFamilyName,
                                   pam.PlayerGivenName,
                                   PlayerId = pam.DataExchangePlayerId,
                                   pam.PlayerNationalityCode,
                                   pam.PlayerNationalityName,
                                   pam.Year,
                                   pam.MatchTypeCode,
                                   pam.PlayerEntryClassificationCode,
                                   ev.PlayerTypeCode
                               }).ToList();

                var lResult = (from ol in allOlympics
                               join pam in allPams on ol.Year equals pam.Year
                               group new
                               {
                                   ol,
                                   Players = new
                                   {
                                       pam.Gender,
                                       pam.PlayerFamilyName,
                                       pam.PlayerGivenName,
                                       pam.PlayerId,
                                       //pam.PlayerNationalityCode,
                                       //pam.PlayerNationalityName,
                                       pam.Year
                                   }
                               } by ol into g
                               orderby g.Key.Year
                               select new
                               {
                                   Players = from gp in g.Distinct().ToList() select gp.Players,
                                   g.Key.HostCity,
                                   g.Key.HostNationCode,
                                   g.Key.HostNationName,
                                   g.Key.TournamentName,
                                   g.Key.Year
                               }).ToList();

                var result = Mapper.Map<ICollection<OlympicsPlayersViewModel>>(lResult);

                result.ToList().ForEach(x =>
                {
                    x.Players.ForEach(y =>
                    {
                        allPams.Where(z => z.Year == x.Year && z.PlayerId == y.PlayerId).ToList().ForEach(z =>
                        {
                            //first row
                            if (y.PlayerNationalityCode == null)
                            {
                                if (!isOlympics)
                                {
                                    if (z.PlayerTypeCode == "Q")
                                    {
                                        y.QuadSingles = z.MatchTypeCode == "S" ? z.PlayerEntryClassificationCode : string.Empty;
                                        y.QuadDoubles = z.MatchTypeCode == "D" ? z.PlayerEntryClassificationCode : string.Empty;
                                    }
                                    else
                                    {
                                        y.Singles = z.MatchTypeCode == "S" ? z.PlayerEntryClassificationCode : string.Empty;
                                        y.Doubles = z.MatchTypeCode == "D" ? z.PlayerEntryClassificationCode : string.Empty;
                                    }
                                }
                                else
                                {
                                    y.Singles = z.MatchTypeCode == "S" ? z.PlayerEntryClassificationCode : string.Empty;
                                    y.Doubles = z.MatchTypeCode == "D" ? z.PlayerEntryClassificationCode : string.Empty;
                                    y.MixedDoubles = z.MatchTypeCode == "MX" ? z.PlayerEntryClassificationCode : string.Empty;
                                }
                            }
                            else
                            {
                                switch (z.MatchTypeCode)
                                {
                                    case "S":
                                        if (z.PlayerTypeCode == "Q")
                                        {
                                            y.QuadSingles = z.PlayerEntryClassificationCode;
                                        }
                                        else
                                        {
                                            y.Singles = z.PlayerEntryClassificationCode;
                                        }
                                        break;
                                    case "D":
                                        if (z.PlayerTypeCode == "Q")
                                        {
                                            y.QuadDoubles = z.PlayerEntryClassificationCode;
                                        }
                                        else
                                        {
                                            y.Doubles = z.PlayerEntryClassificationCode;
                                        }
                                        break;
                                    case "MX":
                                        y.MixedDoubles = z.PlayerEntryClassificationCode;
                                        break;
                                }
                            }

                            y.PlayerNationalityCode = z.PlayerNationalityCode; //Last value is correct
                            y.PlayerNationalityName = z.PlayerNationalityName; //Last value is correct
                        });
                    });
                });

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public ICollection<PlayerViewModel> GetPlayersForSearch(string tournamentType, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PlayerViewModel>;
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

                var tourCategoryCode = GetTourCategoryCodeByTournamentType(tournamentType);

                var lResult = (from pam in _itfOnlineRepo.GetMany<PlayerActivityMatch>(x => x.TourCategoryCode == tourCategoryCode)
                               select new
                               {
                                   PlayerId = pam.DataExchangePlayerId,
                                   pam.PlayerFamilyName,
                                   pam.PlayerGivenName,
                                   Gender = pam.PlayerGender,
                                   pam.PlayerNationalityCode,
                                   pam.PlayerNationalityName
                               }).Distinct().ToList();

                var repo = GetRepoByTournamentType(tournamentType);
                var lResultXml = (from pl in repo.GetMany<LatestPlayer>(x => !lResult.Exists(y => y.PlayerId == x.Id))
                                  join na in repo.GetAll<LatestNation>() on pl.CountryCode equals na.CountryCode
                                  select new
                                  {
                                      PlayerId = pl.Id,
                                      PlayerFamilyName = pl.FamilyName,
                                      PlayerGivenName = pl.GivenName,
                                      pl.Gender,
                                      PlayerNationalityCode = pl.CountryCode,
                                      PlayerNationalityName = na.NameEN
                                  }).Distinct().ToList();

                var result = Mapper.Map<ICollection<PlayerViewModel>>(lResult);
                var resultXml = Mapper.Map<ICollection<PlayerViewModel>>(lResultXml);
                ((List<PlayerViewModel>)result).AddRange(resultXml);

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        #region Player profile service

        public HeadToHeadViewModel GetPlayerHeadToHead(string tournamentType, int playerId, int opponentPlayerId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}.{playerId}.{opponentPlayerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as HeadToHeadViewModel;
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

                var player = GetPlayerByOdfOrDataExchangeId(playerId);
                var opponentPlayer = GetPlayerByOdfOrDataExchangeId(opponentPlayerId);
                if (player == null)
                {
                    Logger.Warn($"Player not found: {playerId}");
                    return null;
                }
                if (opponentPlayer == null)
                {
                    Logger.Warn($"OpponentPlayer not found: {opponentPlayerId}");
                    return null;
                }

                var repo = _itfOnlineRepo;

                var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => x.DataExchangePlayerId == player.DataExchangePlayerId
                               && x.OpponentDataExchangePlayerId == opponentPlayer.DataExchangePlayerId
                               && new[] { "WT", "MT", "WCT" }.Contains(x.TennisCategoryCode)
                               && x.MatchTypeCode == "S")
                               select pam).ToList();
                               //select new
                               //{
                               //    pam.HostNationCode,
                               //    pam.HostNationName,
                               //    pam.ResultCode,
                               //    pam.RoundCode,
                               //    pam.Score,
                               //    pam.SurfaceCode,
                               //    pam.SurfaceDesc,
                               //    pam.TourCategoryCode,
                               //    pam.TourCategoryDesc,
                               //    pam.TournamentName,
                               //    pam.Venue,
                               //    pam.Year
                               //}).ToList();

                var result = new HeadToHeadViewModel
                {
                    Matches = Mapper.Map<ICollection<HeadToHeadMatchActivityViewModel>>(lResult),
                    Player = GetPlayerProfile(tournamentType, playerId, useCache),
                    OpponentPlayer = GetPlayerProfile(tournamentType, opponentPlayerId, useCache)
                };

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public ICollection<TournamentViewModel> GetPlayerActivity(string tournamentType, int playerId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}.{playerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<TournamentViewModel>;
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

                var player = GetPlayerByOdfOrDataExchangeId(playerId);
                if (player == null)
                {
                    Logger.Warn($"Player not found: {playerId}");
                    return null;
                }

                var repo = _itfOnlineRepo;
                var tourCategoryCode = GetTourCategoryCodeByTournamentType(tournamentType);

                var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => x.PlayerID == player.PlayerID && x.TourCategoryCode == tourCategoryCode)
                               from ev in repo.GetMany<Event>(x => x.EventID == pam.EventID)
                               from tm in repo.GetMany<Tournament>(x => x.TournamentID == pam.TournamentID)
                               //join ev in repo.GetAll<Event>() on pam.EventID equals ev.EventID
                               //join tm in repo.GetAll<Tournament>() on ev.TournamentID equals tm.TournamentID
                               orderby tm.Year
                               group new
                               {
                                   Events = new
                                   {
                                       ev.EndDate,
                                       ev.EventClassificationCode,
                                       ev.EventClassificationDesc,
                                       EventId = ev.EventID,
                                       ev.HostNationCode,
                                       ev.HostNationName,
                                       ev.IndoorOutdoorFlag,
                                       ev.MatchTypeCode,
                                       ev.MatchTypeDesc,
                                       ev.Name,
                                       ev.PlayerTypeCode,
                                       ev.PlayerTypeDesc,
                                       ev.StartDate,
                                       ev.SurfaceDesc,
                                       ev.Venue,
                                       ev.Year
                                   },
                                   PlayerActivityMatches = new
                                   {
                                       pam.OpponentPartnerPlayerFamilyName,
                                       pam.OpponentPartnerPlayerGivenName,
                                       OpponentPartnerPlayerID = pam.OpponentPartnerDataExchangePlayerId,
                                       pam.OpponentPartnerPlayerNationalityCode,
                                       pam.OpponentPlayerFamilyName,
                                       pam.OpponentPlayerGivenName,
                                       OpponentPlayerID = pam.OpponentDataExchangePlayerId,
                                       pam.OpponentPlayerNationalityCode,
                                       pam.PartnerPlayerFamilyName,
                                       pam.PartnerPlayerGivenName,
                                       PartnerPlayerID = pam.PartnerDataExchangePlayerId,
                                       pam.PartnerPlayerNationalityCode,
                                       pam.PlayerEntryClassificationCode,
                                       pam.ResultCode,
                                       pam.RoundCode,
                                       pam.RoundNumber,
                                       pam.Score,
                                       pam.TournamentName
                                   }
                               } by new
                               {
                                   tm.TournamentID,
                                   tm.EndDate,
                                   tm.HostNationName,
                                   tm.Name,
                                   tm.StartDate,
                                   tm.TourCategoryDesc,
                                   tm.Venue,
                                   tm.Year
                               } into g
                               select new
                               {
                                   g.Key.EndDate,
                                   g.Key.HostNationName,
                                   g.Key.Name,
                                   g.Key.StartDate,
                                   g.Key.TourCategoryDesc,
                                   g.Key.Venue,
                                   g.Key.Year,
                                   Events = from gl in g.ToList()
                                            group gl by gl.Events into ge
                                            select new
                                            {
                                                ge.Key.EndDate,
                                                ge.Key.EventClassificationCode,
                                                ge.Key.EventClassificationDesc,
                                                ge.Key.EventId,
                                                ge.Key.HostNationCode,
                                                ge.Key.HostNationName,
                                                ge.Key.IndoorOutdoorFlag,
                                                ge.Key.MatchTypeCode,
                                                ge.Key.MatchTypeDesc,
                                                ge.Key.Name,
                                                ge.Key.PlayerTypeCode,
                                                ge.Key.PlayerTypeDesc,
                                                ge.Key.StartDate,
                                                ge.Key.SurfaceDesc,
                                                ge.Key.Venue,
                                                ge.Key.Year,
                                                PlayerActivityMatches = from gel in ge.ToList() select gel.PlayerActivityMatches
                                            }
                               }).ToList();

                var result = Mapper.Map<ICollection<TournamentViewModel>>(lResult);

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public BasePlayerProfileViewModel GetPlayerProfile(string tournamentType, int playerId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}.{playerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as BasePlayerProfileViewModel;
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

                var player = GetPlayerByOdfOrDataExchangeId(playerId);
                if (player == null)
                {
                    Logger.Warn($"Player not found: {playerId}");
                    return null;
                }

                var repo = _itfOnlineRepo;
                var isOlympics = IsOlympics(tournamentType);
                BasePlayerProfileViewModel result;

                if (isOlympics)
                {
                    result = player.Gender == "F"
                        ? Mapper.Map<PlayerProfileViewModel>(repo.Get<PlayerBiographyWomens>(x => x.PlayerID == player.PlayerID))
                        : Mapper.Map<PlayerProfileViewModel>(repo.Get<PlayerBiographyMens>(x => x.PlayerID == player.PlayerID));
                }
                else
                {
                    result = Mapper.Map<WheelchairPlayerProfileViewModel>(repo.Get<PlayerBiographyWheelchair>(x => x.PlayerID == player.PlayerID));
                }
                result.HeadshotAssetId = GetPlayerImageAssetId(player.PlayerID);
                result.PlayerID = playerId;

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        #region result service

        public ICollection<OlympicTennisViewModel> GetOlympicTennis(
            Language language = Language.En, DataSource source = DataSource.Itf, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Itf) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<OlympicTennisViewModel>;
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

                var repo = _itfOnlineRepo;
                var tournaments = repo.GetMany<Tournament>(x => x.TourCategoryCode == TccOl).ToList();
                var tournamentIds = tournaments.Select(x => x.TournamentID).ToList();

                var lResult = (from o in repo.GetAll<Olympics>()
                               join t in tournaments on o.Year equals t.Year
                               join e in repo.GetMany<Event>(x => tournamentIds.Contains(x.TournamentID)) on t.TournamentID equals e.TournamentID
                               join m in repo.GetMany<Match>(x => tournamentIds.Contains(x.TournamentID)) on e.EventID equals m.EventID
                               let nt = repo.Get<NationTranslated>(x => x.NationCode == o.HostNationCode, true)
                               orderby o.Year, t.TennisCategoryCode, t.TournamentID, MtcOrders.OrderOf(e.MatchTypeCode), e.EventClassificationCode,
                               m.DrawsheetRoundNumber, m.DrawsheetPositionMatch, m.DrawsheetPositionSide1, m.DrawsheetPositionSide2
                               group new
                               {
                                   Events = new
                                   {
                                       PlayerType = e.MatchTypeCode == McX ? "Mixed Doubles" : e.PlayerTypeDesc,
                                       MatchType = e.MatchTypeDesc,
                                       EventType = e.EventClassificationDesc,
                                       e.Venue,
                                       Surface = e.SurfaceDesc,
                                       IndoorOutdoor = e.IndoorOutdoorFlag.GetIndoorOutdoor()
                                   },
                                   Rounds = new
                                   {
                                       RoundNumber = m.DrawsheetRoundNumber,
                                       RoundDesc = m.DrawsheetRoundDesc
                                   },
                                   Matches = new
                                   {
                                       m.Side1Player1GivenName,
                                       m.Side1Player1FamilyName,
                                       Side1Player1NationCode = m.Side1Player1NationalityCode,
                                       m.Side1Player2GivenName,
                                       m.Side1Player2FamilyName,
                                       Side1Player2NationCode = m.Side1Player2NationalityCode,
                                       m.Side2Player1GivenName,
                                       m.Side2Player1FamilyName,
                                       Side2Player1NationCode = m.Side2Player1NationalityCode,
                                       m.Side2Player2GivenName,
                                       m.Side2Player2FamilyName,
                                       Side2Player2NationCode = m.Side2Player2NationalityCode,
                                       m.WinningSide,
                                       m.Score
                                   }
                               } by new
                               {
                                   o.Year,
                                   o.HostCity,
                                   o.HostNationCode
                               } into g
                               select new
                               {
                                   g.Key.Year,
                                   g.Key.HostCity,
                                   g.Key.HostNationCode,
                                   HostNation = repo.Get<NationTranslated>(x => x.NationCode == g.Key.HostNationCode, true)?.GetNationByLanguage(language),
                                   Events = from gl in g.ToList()
                                            group gl by gl.Events into ge
                                            select new
                                            {
                                                ge.Key.PlayerType,
                                                ge.Key.MatchType,
                                                ge.Key.EventType,
                                                ge.Key.Venue,
                                                ge.Key.Surface,
                                                ge.Key.IndoorOutdoor,
                                                Rounds = from gel in ge.ToList()
                                                         group gel by gel.Rounds into gr
                                                         select new
                                                         {
                                                             gr.Key.RoundNumber,
                                                             gr.Key.RoundDesc,
                                                             Matches = from grl in gr.ToList() select grl.Matches
                                                         }
                                            }
                               }).ToList();

                var result = Mapper.Map<ICollection<OlympicTennisViewModel>>(lResult);

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

        public ICollection<GrandSlamViewModel> GetGrandSlam(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Itf, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Itf) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<GrandSlamViewModel>;
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

                var lstNationCodes = _itfOnlineRepo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var repo = _baseline02Repo;
                var tournaments = repo.GetMany<Models.Baseline02.Tournament>(x => x.TourCategoryCode == TccSl && lstNationCodes.Contains(x.HostNationCode)).ToList();
                var tournamentIds = tournaments.Select(x => x.TournamentID).ToList();
                var tournamentDetails = repo.GetMany<Models.Baseline02.TournamentDetail>(x => tournamentIds.Contains(x.TournamentID)).ToList();
                var tournamentDetailIds = tournamentDetails.Select(x => x.TournamentDetailID).ToList();
                var events = repo.GetMany<Models.Baseline02.Event>(x => tournamentDetailIds.Contains(x.TournamentDetailID) && x.EventClassificationCode == EccM).ToList();
                var eventIds = events.Select(x => x.EventID).ToList();

                var matchDoubles = repo.GetMany<MatchDoubles>(x => x.RoundCode == "FR" && eventIds.Contains(x.EventID.Value)).ToList();
                var matchPlayerDoubleIds = matchDoubles
                    .Select(x => new[] {x.Side1MatchPlayer1ID, x.Side1MatchPlayer2ID, x.Side2MatchPlayer1ID, x.Side2MatchPlayer2ID})
                    .SelectMany(x => x).Where(x => x.HasValue).Distinct().ToList();
                var matchSingles = repo.GetMany<MatchSingles>(x => x.RoundCode == "FR" && eventIds.Contains(x.EventID.Value)).ToList();
                var matchPlayerSingleIds = matchSingles
                    .Select(x => new[] { x.MatchPlayer1ID, x.MatchPlayer2ID })
                    .SelectMany(x => x).Where(x => x.HasValue).Distinct().ToList();
                var matchPlayerIds = matchPlayerDoubleIds.Union(matchPlayerSingleIds).ToList();
                var players = repo.GetMany<MatchPlayer>(x => matchPlayerIds.Contains(x.MatchPlayerID)).Select(x => new
                {
                    x.MatchPlayerID,
                    x.GivenName,
                    x.FamilyName,
                    x.NationalityCode
                }).ToList();
                var matchPlayerEventInfos = repo.GetMany<MatchPlayerEventInfo>(x => eventIds.Contains(x.EventID)).ToList();
                var matchEventPlayerIds = matchPlayerEventInfos.Select(x => x.MatchPlayerID).Distinct().ToList();

                var lResult = (from t in tournaments
                               join td in tournamentDetails on t.TournamentID equals td.TournamentID
                               join e in events on td.TournamentDetailID equals e.TournamentDetailID
                               join md in matchDoubles on e.EventID equals md.EventID into mdL from md in mdL.DefaultIfEmpty()
                               join ms in matchSingles on e.EventID equals ms.EventID into msL from ms in msL.DefaultIfEmpty()
                               join mp in matchPlayerEventInfos on e.EventID equals mp.EventID
                               join p in repo.GetMany<MatchPlayer>(x => matchEventPlayerIds.Contains(x.MatchPlayerID)) on mp.MatchPlayerID equals p.MatchPlayerID
                               let pl11 = players.FirstOrDefault(x => x.MatchPlayerID == (e.MatchTypeCode == McS ? ms?.MatchPlayer1ID : md?.Side1MatchPlayer1ID))
                               let pl21 = players.FirstOrDefault(x => x.MatchPlayerID == (e.MatchTypeCode == McS ? ms?.MatchPlayer2ID : md?.Side2MatchPlayer1ID))
                               let pl12 = e.MatchTypeCode == McS ? null : players.FirstOrDefault(x => x.MatchPlayerID == md?.Side1MatchPlayer2ID)
                               let pl22 = e.MatchTypeCode == McS ? null : players.FirstOrDefault(x => x.MatchPlayerID == md?.Side2MatchPlayer2ID)
                               let winningSide = e.MatchTypeCode == McS ? ms?.WinningSide : md?.WinningSide
                               let side1Set1Score = e.MatchTypeCode == McS ? ms?.Player1Set1Score : md?.Side1Set1Score
                               let side2Set1Score = e.MatchTypeCode == McS ? ms?.Player2Set1Score : md?.Side2Set1Score
                               let losingSet1TieBreakScore = e.MatchTypeCode == McS ? ms?.LosingSet1TieBreakScore : md?.LosingSet1TieBreakScore
                               let scoreSet1 = side1Set1Score != null && side2Set1Score != null ? (winningSide == 1 ? $"{side1Set1Score}-{side2Set1Score}" : $"{side2Set1Score}-{side1Set1Score}")
                                    + (losingSet1TieBreakScore == null ? string.Empty : $"({losingSet1TieBreakScore})") : null
                               let side1Set2Score = e.MatchTypeCode == McS ? ms?.Player1Set2Score : md?.Side1Set2Score
                               let side2Set2Score = e.MatchTypeCode == McS ? ms?.Player2Set2Score : md?.Side2Set2Score
                               let losingSet2TieBreakScore = e.MatchTypeCode == McS ? ms?.LosingSet2TieBreakScore : md?.LosingSet2TieBreakScore
                               let scoreSet2 = side1Set2Score != null && side2Set2Score != null ? (winningSide == 1 ? $"{side1Set2Score}-{side2Set2Score}" : $"{side2Set2Score}-{side1Set2Score}")
                                    + (losingSet2TieBreakScore == null ? string.Empty : $"({losingSet2TieBreakScore})") : null
                               let side1Set3Score = e.MatchTypeCode == McS ? ms?.Player1Set3Score : md?.Side1Set3Score
                               let side2Set3Score = e.MatchTypeCode == McS ? ms?.Player2Set3Score : md?.Side2Set3Score
                               let losingSet3TieBreakScore = e.MatchTypeCode == McS ? ms?.LosingSet3TieBreakScore : md?.LosingSet3TieBreakScore
                               let scoreSet3 = side1Set3Score != null && side2Set3Score != null ? (winningSide == 1 ? $"{side1Set3Score}-{side2Set3Score}" : $"{side2Set3Score}-{side1Set3Score}")
                                    + (losingSet3TieBreakScore == null ? string.Empty : $"({losingSet3TieBreakScore})") : null
                               let side1Set4Score = e.MatchTypeCode == McS ? ms?.Player1Set4Score : md?.Side1Set4Score
                               let side2Set4Score = e.MatchTypeCode == McS ? ms?.Player2Set4Score : md?.Side2Set4Score
                               let losingSet4TieBreakScore = e.MatchTypeCode == McS ? ms?.LosingSet4TieBreakScore : md?.LosingSet4TieBreakScore
                               let scoreSet4 = side1Set4Score != null && side2Set4Score != null ? (winningSide == 1 ? $"{side1Set4Score}-{side2Set4Score}" : $"{side2Set4Score}-{side1Set4Score}")
                                    + (losingSet4TieBreakScore == null ? string.Empty : $"({losingSet4TieBreakScore})") : null
                               let side1Set5Score = e.MatchTypeCode == McS ? ms?.Player1Set5Score : md?.Side1Set5Score
                               let side2Set5Score = e.MatchTypeCode == McS ? ms?.Player2Set5Score : md?.Side2Set5Score
                               let losingSet5TieBreakScore = e.MatchTypeCode == McS ? ms?.LosingSet5TieBreakScore : md?.LosingSet5TieBreakScore
                               let scoreSet5 = side1Set5Score != null && side2Set5Score != null ? (winningSide == 1 ? $"{side1Set5Score}-{side2Set5Score}" : $"{side2Set5Score}-{side1Set5Score}")
                                    + (losingSet5TieBreakScore == null ? string.Empty : $"({losingSet5TieBreakScore})") : null
                               orderby t.TournamentYear, t.TennisCategoryCode, p.FamilyName
                               where md != null || ms != null
                               group new
                               {
                                   Events = new
                                   {
                                       PlayerType = e.PlayerTypeCode == "W" ? "Women" : "Men",
                                       MatchType = e.MatchTypeCode == McX ? "Mixed Doubles" : e.MatchTypeCode == McD ? "Doubles" : "Singles",
                                       Surface = e.SurfaceCode.GetSurfaceDesc(),
                                       IndoorOutdoor = e.IndoorOutdoorCode.GetIndoorOutdoor(),
                                       FinalWinningSide = winningSide,
                                       FinalSide1Player1GivenName = pl11.GivenName,
                                       FinalSide1Player1FamilyName = pl11.FamilyName,
                                       FinalSide1Player1NationCode = pl11.NationalityCode,
                                       FinalSide2Player1GivenName = pl21.GivenName,
                                       FinalSide2Player1FamilyName = pl21.FamilyName,
                                       FinalSide2Player1NationCode = pl21.NationalityCode,
                                       FinalSide1Player2GivenName = pl12?.GivenName,
                                       FinalSide1Player2FamilyName = pl12?.FamilyName,
                                       FinalSide1Player2NationCode = pl12?.NationalityCode,
                                       FinalSide2Player2GivenName = pl22?.GivenName,
                                       FinalSide2Player2FamilyName = pl22?.FamilyName,
                                       FinalSide2Player2NationCode = pl22?.NationalityCode,
                                       FinalScore = string.Join(" ", scoreSet1, scoreSet2, scoreSet3, scoreSet4, scoreSet5).TrimEnd()
                                   },
                                   Players = new
                                   {
                                       PlayerGivenName = p.GivenName,
                                       PlayerFamilyName = p.FamilyName,
                                       PlayerNationCode = p.NationalityCode,
                                       PlayerId = p.MatchPlayerID,
                                       mp.Seeding,
                                       mp.FinalPositionCode
                                   }
                               } by new
                               {
                                   Year = t.TournamentYear,
                                   t.HostNationCode,
                                   t.Name,
                                   t.MediaName,
                                   Venue = t.MediaVenue,
                                   Dates = t.StartDate.FormatLongByLanguage(language) + " - " + t.EndDate.FormatLongByLanguage(language)
                               } into g
                               select new
                               {
                                   g.Key.Year,
                                   g.Key.HostNationCode,
                                   g.Key.Name,
                                   g.Key.MediaName,
                                   g.Key.Venue,
                                   g.Key.Dates,
                                   Events = from gl in g.ToList()
                                            group gl by gl.Events into ge
                                            select new
                                            {
                                                ge.Key.PlayerType,
                                                ge.Key.MatchType,
                                                ge.Key.Surface,
                                                ge.Key.IndoorOutdoor,
                                                ge.Key.FinalWinningSide,
                                                ge.Key.FinalSide1Player1GivenName,
                                                ge.Key.FinalSide1Player1FamilyName,
                                                ge.Key.FinalSide1Player1NationCode,
                                                ge.Key.FinalSide2Player1GivenName,
                                                ge.Key.FinalSide2Player1FamilyName,
                                                ge.Key.FinalSide2Player1NationCode,
                                                ge.Key.FinalSide1Player2GivenName,
                                                ge.Key.FinalSide1Player2FamilyName,
                                                ge.Key.FinalSide1Player2NationCode,
                                                ge.Key.FinalSide2Player2GivenName,
                                                ge.Key.FinalSide2Player2FamilyName,
                                                ge.Key.FinalSide2Player2NationCode,
                                                ge.Key.FinalScore,
                                                Players = from gel in ge.ToList() select gel.Players
                                            }
                               }).ToList();

                var result = Mapper.Map<ICollection<GrandSlamViewModel>>(lResult);

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

        public ICollection<OlympicsViewModel> GetAllOlympics(string tournamentType, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<OlympicsViewModel>;
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

                var olympics = GetAllOlympicsByTournamentType(tournamentType).OrderBy(x => x.Year);
                var result = Mapper.Map<ICollection<OlympicsViewModel>>(olympics);

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public DrawsheetViewModel GetDrawsheetByEventId(string tournamentType, int eventId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}.{eventId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as DrawsheetViewModel;
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

                var repo = _itfOnlineRepo;
                var lResult = (repo.GetMany<Event>(x => x.EventID == eventId) as IQueryable<Event>).Include(x => x.Matches).FirstOrDefault();
                if (lResult == null)
                {
                    Logger.Warn($"Event not found: {eventId}");
                    return null;
                }

                var result = Mapper.Map<DrawsheetViewModel>(lResult);

                var lResultThirdFourth = (repo.GetMany<Event>(x => x.TournamentID == lResult.TournamentID
                && x.EventClassificationCode == "PP3-4"
                && x.MatchTypeCode == lResult.MatchTypeCode
                && x.PlayerTypeCode == lResult.PlayerTypeCode) as IQueryable<Event>).Include(x => x.Matches).FirstOrDefault();
                result.ThirdFourthEvent = Mapper.Map<DrawsheetViewModel>(lResultThirdFourth);

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public ICollection<EventViewModel> GetOlympicEventsByYear(string tournamentType, int year, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{tournamentType}.{year}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<EventViewModel>;
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

                var repo = _itfOnlineRepo;
                var tourCategoryCode = GetTourCategoryCodeByTournamentType(tournamentType);
                var lResult = (from ev in repo.GetMany<Event>(x => x.Tournament.TourCategoryCode == tourCategoryCode && x.Year == year && x.EventClassificationCode == "M")
                               select ev).ToList();

                var result = Mapper.Map<ICollection<EventViewModel>>(lResult);

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        #region LiveBlog

        public LiveBlogDataViewModel GetLiveBlogData(string siteLanguage, string fileName, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{siteLanguage}.{fileName}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as LiveBlogDataViewModel;
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

                var xmlFilePath = $"App_Data/liveblog/{siteLanguage}/{fileName}.xml";
                var repo = _olympicsXmlOlyRepo; //_olympicsXmlPlyRepo
                var lResult = repo.Deserialize<Models.ViewModels.LiveBlog.Data>(xmlFilePath);
                if (lResult == null)
                {
                    Logger.Warn($"LiveBlogData not found: {siteLanguage}/{fileName}");
                    return null;
                }
                var result = Mapper.Map<LiveBlogDataViewModel>(new
                {
                    lResult.Date, lResult.Editor,
                    Blogs = lResult.Blogs.Blog,
                    Headlines = lResult.Headlines.Headline
                });

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        #region Translations

        public Dictionary<string, string> GetTranslationsByIso(string languageIsoCode, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{languageIsoCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as Dictionary<string, string>;
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

                var result = new Dictionary<string, string>();
                var repo = _cmsRepo;
                (repo.GetMany<CmsDictionary>(
                    x => x.CmsLanguageTexts.FirstOrDefault(y => y.UmbracoLanguage.LanguageISOCode == languageIsoCode) != null) as IQueryable<CmsDictionary>)
                    .Include(x => x.CmsLanguageTexts.Select(y => y.UmbracoLanguage)).ToList().ForEach(
                    x =>
                    {
                        var umbracoLanguage = x.CmsLanguageTexts.FirstOrDefault(y => y.UmbracoLanguage.LanguageISOCode == languageIsoCode);
                        if (umbracoLanguage != null)
                        {
                            result.Add(x.Key, umbracoLanguage.Value);
                        }
                    });

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        #region HearAward

        public CupHeartAwardData VoteUp(string code, int playerId, string nationCode, string name)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"VoteUp.Begin(code={code}, playerId={playerId}, nationCode={nationCode}, name={name})");
                }

                var repo = _cmsRepo;
                var cupHeartAwardData = repo.Get<CupHeartAwardData>(x => x.Code == code && x.PlayerId == playerId);
                if (cupHeartAwardData == null)
                {
                    cupHeartAwardData = new CupHeartAwardData
                    {
                        Code = code,
                        PlayerId = playerId,
                        Name = name,
                        NationCode = nationCode,
                        Votes = 1,
                        LastUpdated = DateTime.UtcNow
                    };
                    repo.Add(cupHeartAwardData);
                }
                else
                {
                    cupHeartAwardData.LastUpdated = DateTime.UtcNow;
                    cupHeartAwardData.Votes++;
                }
                repo.Commit();

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"VoteUp.End(Votes={cupHeartAwardData.Votes}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()})");
                }

                return cupHeartAwardData;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Service Error");
                throw;
            }
        }

        #endregion

        #region Umbraco

        private ITFRelationships GetItfRelationships(int playerId)
        {
            var itfRelationShips = _cmsRepo.GetMany<ITFRelationships>(
                x => x.assetId == playerId
                     && x.AssetType.assetTypeName.Equals("Player")
                     && x.RelatedAssetType.assetTypeName.Equals("Image")
                     && x.CmsPropertyData.dataInt != null && x.CmsPropertyData.dataInt == 1
                     && x.CmsPropertyData.CmsPropertyType.Alias.Equals("playerHeadShot")).ToList();

            var result = itfRelationShips.Any(x => x.sortOrder == null)
                ? itfRelationShips.OrderByDescending(x => x.relationshipId).FirstOrDefault()
                : itfRelationShips.OrderBy(x => x.sortOrder).FirstOrDefault();

            return result;
        }

        private ITFContentProvider GetContentProviderByAssetId(int assetId)
        {
            var repo = _cmsRepo;
            var result = (from rs in repo.GetMany<ITFRelationships>(x => x.assetId == assetId
                          && x.AssetType.assetTypeName.Equals("Image")
                          && x.RelatedAssetType.assetTypeName.Equals("Photographer"))
                          from cp in repo.GetMany<ITFContentProvider>(x => x.Id == rs.relatedAssetId)
                          select cp).FirstOrDefault();
            return result;
        }

        public int? GetPlayerImageAssetId(int playerId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{playerId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as int?;
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

                var result = (int?)0;
                var itfRelationShips = GetItfRelationships(playerId);
                if (itfRelationShips != null)
                {
                    result = itfRelationShips.relatedAssetId;
                }

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        private ICollection<int> GetGalleryImageIds(int assetId)
        {
            var repo = _cmsRepo;
            var gallery = repo.GetMany<ITFRelationships>(y => y.AssetType.assetTypeName.Equals("Gallery")
            && y.RelatedAssetType.assetTypeName.Equals("Image")
            && y.assetId == assetId).ToList();

            var sortedGallery = gallery.Where(y => y.sortOrder != null).OrderBy(y => y.sortOrder).ToList();
            sortedGallery.AddRange(gallery.Where(x => x.sortOrder == null));

            var result = sortedGallery.Select(x => x.relatedAssetId.GetValueOrDefault()).ToList();
            return result;
        }

        public ICollection<PhotosGalleryViewModel> GetGalleriesByWebScopeId(int webScopeNodeId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{webScopeNodeId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<PhotosGalleryViewModel>;
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

                var repo = _cmsRepo;
                var lResult = repo.GetMany<ITFGallery>(x => x.ITFWebScopes.Any(y => y.webScopeNodeId == webScopeNodeId))
                    .OrderByDescending(x => x.CreatedDate).ToList();

                var result = Mapper.Map<ICollection<PhotosGalleryViewModel>>(lResult);
                result.ToList().ForEach(x =>
                {
                    x.PhotoId = GetGalleryImageIds(x.GalleryId).FirstOrDefault();
                });

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public ImageAssetViewModel GetAssetProvider(int assetId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{assetId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ImageAssetViewModel;
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

                var repo = _cmsRepo;
                var contentProvider = GetContentProviderByAssetId(assetId);

                var lResult = new
                {
                    AssetId = assetId,
                    Descriptions = (from md in repo.GetMany<ITFMediaDescription>(x => x.umbracoMediaNodeId == assetId)
                                    select new
                                    {
                                        AlternativeText = md.altText,
                                        CultureCode = md.cultureCode.Trim(),
                                        MediaAssetId = assetId,
                                        Title = md.title
                                    }).ToList(),
                    Photographer = contentProvider?.ContentProvider,
                    PhotographerId = contentProvider?.Id ?? 0
                };

                var result = Mapper.Map<ImageAssetViewModel>(lResult);

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public ICollection<RelatedAssetViewModel> GetRelatedMediaAssets(int assetId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{assetId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<RelatedAssetViewModel>;
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

                var repo = _cmsRepo;
                var result = (from rs in repo.GetMany<ITFRelationships>(x => x.assetId == assetId
                               && new short?[] {3, 5, 6}.Contains(x.relatedAssetTypeId))
                               orderby rs.relatedAssetTypeId, rs.sortOrder
                               select new RelatedAssetViewModel
                               {
                                   AssetId = rs.relatedAssetId ?? 0,
                                   AssetType = rs.relatedAssetTypeId == 3 ? "Image" : (rs.relatedAssetTypeId == 5 ? "PDF" : "Audio"),
                                   TitleEn = repo.Get<ITFMediaDescription>(x => x.umbracoMediaNodeId == rs.relatedAssetId && x.cultureCode == "en")?.title,
                                   TitleEs = repo.Get<ITFMediaDescription>(x => x.umbracoMediaNodeId == rs.relatedAssetId && x.cultureCode == "es")?.title
                               }).ToList();

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public GalleryAssetViewModel GetGalleryByAssetId(int assetId, int limitImages, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{assetId}.{limitImages}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as GalleryAssetViewModel;
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

                var repo = _cmsRepo;
                var galleryImages = GetGalleryImageIds(assetId);
                if (limitImages != 0)
                {
                    galleryImages = galleryImages.Take(limitImages).ToList();
                }

                var result = new GalleryAssetViewModel
                {
                    Images = (from gi in galleryImages
                              let cp = GetContentProviderByAssetId(gi)
                              select new ImageAssetViewModel
                              {
                                  AssetId = gi,
                                  Photographer = cp?.ContentProvider,
                                  PhotographerId = cp?.Id ?? 0,
                                  Descriptions = (from md in repo.GetMany<ITFMediaDescription>(x => x.umbracoMediaNodeId == gi)
                                                  select new MediaAssetDescriptionViewModel
                                                  {
                                                      AlternativeText = md.altText,
                                                      CultureCode = md.cultureCode.Trim(),
                                                      MediaAssetId = gi,
                                                      Title = md.title
                                                  }).ToList()
                              }).ToList()
                };

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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

        public HtmlViewModel GetHtmlByNodeId(int nodeId, bool useCache = true)
        {
            try
            {
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{nodeId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as HtmlViewModel;
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

                var repo = _cmsRepo;
                var lResult = repo.Get<ITFHtml>(x => x.NodeId == nodeId);
                if (lResult == null)
                {
                    Logger.Warn($"HTML not found: {nodeId}");
                    return null;
                }

                var result = new HtmlViewModel {html = lResult.HTML};

                var cacheTimeout = GetCacheTimeout(cachePrefix);
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
