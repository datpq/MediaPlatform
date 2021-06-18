using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models.ViewModels;
using Ninject;
using NLog;
using System.Linq;
using ITF.DataServices.SDK.Data;
using System.Diagnostics;
using ITF.DataServices.SDK.Models;
using System;
using AutoMapper;
using System.Collections.Generic;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.SDK.Models.Cms;
using ITF.DataServices.SDK.Models.Cup;
using ITF.DataServices.SDK.Models.ViewModels.Btd;
using ITF.DataServices.SDK.Models.WorldNet;

namespace ITF.DataServices.SDK.Services
{
    public class NationService : CupService, INationService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly IDataRepository WorldNetRepo;
        private readonly IPlayerService _playerService;
        private readonly IEventService _eventService;
        private readonly ICmsService _cmsService;

        public NationService(
            [Named("DavisCupRepo")] ICupDataRepository davisCupRepo,
            [Named("FedCupRepo")] ICupDataRepository fedCupRepo,
            [Named("CmsRepo")] ISameStructureDataRepository cmsRepo,
            [Named("WorldNetRepo")] IDataRepository worldNetRepo,
            IPlayerService playerService, IEventService eventService, ICmsService cmsService, ICacheConfigurationService cacheConfigurationService)
            : base(davisCupRepo, fedCupRepo, cmsRepo, cacheConfigurationService)
        {
            WorldNetRepo = worldNetRepo;
            _playerService = playerService;
            _eventService = eventService;
            _cmsService = cmsService;
        }

        #region CupService members

        public NationProfileWebViewModelOld GetNationProfileWeb(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as NationProfileWebViewModelOld;
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

                var lResult = (from e in WorldNetRepo.GetMany<Baseline360Export>(x => x.OrgCode == nationCode && new [] { "President", "Secretary" }.Contains(x.RoleCode))
                               select new
                               {
                                   OfficialName = e.Organisation,
                                   WebsiteURL = e.OrgWWW,
                                   AdminName = $"{e.RoleCode}: {e.Title} {e.FirstName} {e.LastName}"
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"GetNationProfileWeb not found: nationCode={nationCode}");
                    return null;
                }

                var result = _cmsService.GetCmsNationProfileWeb(nationCode, language, source, useCache);
                result = Mapper.Map(lResult, result);

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

        public NationViewModelOld GetNation(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as NationViewModelOld;
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var nation = (from nt in repo.GetMany<NationTranslated>(x => x.NationCode == nationCode, true)
                              let nr = repo.GetSpecific<NationRank>(x => x.NationCode == nationCode, true)
                              select new
                              {
                                  Nation = nt.GetNationByLanguage(language),
                                  NationI = nt.GetNationByLanguage(language)?.Substring(0, 1),
                                  NationEN = nt.GetNationByLanguage(Language.En),
                                  NationENI = nt.GetNationByLanguage(Language.En)?.Substring(0, 1),
                                  NationES = nt.GetNationByLanguage(Language.Es),
                                  NationESI = nt.GetNationByLanguage(Language.Es)?.Substring(0, 1),
                                  nr?.Rank
                              }).FirstOrDefault();

                if (nation == null)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var champions = (from tie in GetChampionTies(source)
                                 where ((lstNationCodes.Contains(tie.Side1NationCode) && tie.WinningSide == 1)
                                   || (lstNationCodes.Contains(tie.Side2NationCode) && tie.WinningSide == 2))
                                 orderby tie.Year
                                 select tie.Year).ToList();

                var firstYearPlayed = nationCode == Srb ? SrbYear :
                    (from nat in repo.GetMany<NationActivityTie>(x => lstNationCodes.Contains(x.NationCode))
                     where nat.PlayStatusCode.SqlNotEquals(PsNp)
                     orderby nat.Year
                     select nat.Year).FirstOrDefault();

                var yearsPlayed = (from nat in repo.GetMany<NationActivityTie>(x => lstNationCodes.Contains(x.NationCode))
                                   where nat.PlayStatusCode.SqlNotEquals(PsNp) && (nationCode != Srb || nat.Year >= SrbYear)
                                   select nat.Year).Distinct().Count();

                var teamWinLoss = (from nat in repo.GetMany<NationActivityTie>(x => lstNationCodes.Contains(x.NationCode))
                                   where nat.PlayStatusCode.SqlNotEquals(PsNp) && (nationCode != Srb || nat.Year >= SrbYear)
                                   group nat by 1 into g
                                   select new
                                   {
                                       TiesPlayed = g.Count(),
                                       WinCount = g.Sum(x => x.WinCount),
                                       LossCount = g.Sum(x => x.LossCount)
                                   }).FirstOrDefault();

                var year = DateTime.Now.Year;
                var teamWgWinLoss = (from nat in repo.GetMany<NationActivityTie>(
                                        x => EdcWg.Contains(x.EventDivisionCode)
                                        && ((nationCode != Srb && x.Year > NonSrbYearWG)|| x.Year >= SrbYear)
                                        && lstNationCodes.Contains(x.NationCode)
                                        && x.Year <= year && x.EventClassificationCode == "M")
                                     group nat by 1 into g
                                     select new
                                     {
                                         WGYearsPlayed = g.Select(x => x.Year).Distinct().Count(),
                                         WGWin = g.Sum(x => x.WinCount),
                                         WGLoss = g.Sum(x => x.LossCount)
                                     }).FirstOrDefault();

                var yesterday = DateTime.Today.AddDays(-1);
                var teamLastTie = (from nat in repo.GetMany<NationActivityTie>(x => x.NationCode == nationCode && x.EndDate <= yesterday && x.ResultCode != null)
                                   orderby nat.StartDate descending
                                   select new
                                   {
                                       LastTieResultShort = nat.TieResultShort,
                                       LastTieEventDivisionDesc = nat.EventDivisionDesc,
                                       LastTieRoundDesc = nat.RoundDesc,
                                       LastTieEventClassificationCode = nat.EventClassificationCode,
                                       LastTieEventClassificationDesc = nat.EventClassificationDesc,
                                       LastTieYear = nat.Year
                                   }).FirstOrDefault();

                var result = new NationViewModelOld
                {
                    NationCode = nationCode,
                    Ranking = nation.Rank.ToString(),
                    Champion = champions.Count,
                    FirstYearPlayed = firstYearPlayed,
                    YearsPlayed = yearsPlayed,
                    TiesPlayed = teamWinLoss == null ? "0" :
                        $"{teamWinLoss.TiesPlayed} ({teamWinLoss.WinCount} - {teamWinLoss.LossCount})",
                    YearsInWG = teamWgWinLoss == null ? "0" :
                        $"{teamWgWinLoss.WGYearsPlayed} ({teamWgWinLoss.WGWin} - {teamWgWinLoss.WGLoss})",
                    LastTiePlayed = teamLastTie == null ? string.Empty :
                        $"{teamLastTie.LastTieResultShort} {teamLastTie.LastTieEventDivisionDesc} {(teamLastTie.LastTieEventClassificationCode.Contains("PO") ? teamLastTie.LastTieEventClassificationDesc : teamLastTie.LastTieRoundDesc)}, {teamLastTie.LastTieYear}",
                    ChampionYears = champions.Count > 0 ? $"({string.Join(", ", champions)})" : null
                };

                result = Mapper.Map(nation, result);

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

        public ICollection<NationCoreViewModelOld> GetNations(Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationCoreViewModelOld>;
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
                var lResult = (from ng in repo.GetAll<NationGrouping>()
                               join nt in repo.GetAll<NationTranslated>(true) on ng.NationCode equals nt.NationCode
                               join n in repo.GetMany<Nation>(x => x.CurrentNationFlag == "Y") on nt.NationCode equals n.NationCode
                               orderby ng.NationCode
                               select new
                               {
                                   nt.NationCode,
                                   Nation = nt.GetNationByLanguage(language),
                                   NationI = nt.GetNationIByLanguage(language),
                                   NationEN = nt.GetNationByLanguage(Language.En),
                                   NationENI = nt.GetNationIByLanguage(Language.En),
                                   NationES = nt.GetNationByLanguage(Language.Es),
                                   NationESI = nt.GetNationIByLanguage(Language.Es)
                               }).Distinct().ToList();

                var result = Mapper.Map<ICollection<NationCoreViewModelOld>>(lResult);

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

        public HeadToHeadNationToNationViewModel GetHeadToHeadNationToNation(string nationCode, string opponentNationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}.{opponentNationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey);
                    if (cacheValue != null)
                    {
                        return cacheValue as HeadToHeadNationToNationViewModel;
                    }
                }

                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"{cacheKey}, useCache={useCache}");
                }

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;

                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                var lstOpponentNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == opponentNationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (!lstNationCodes.Any() || !lstOpponentNationCodes.Any())
                {
                    Logger.Warn($"Nation not found: {nationCode} or {opponentNationCode}");
                    return null;
                }
                var nt = repo.Get<NationTranslated>(x => x.NationCode == nationCode, true);
                var ntOpponent = repo.Get<NationTranslated>(x => x.NationCode == opponentNationCode, true);

                var lResult = (from nat in repo.GetMany<NationActivityTie>(x => new[] { PsPc, "PU", "PA" }.Contains(x.PlayStatusCode)
                               && ((nationCode != Srb && opponentNationCode != Srb) || x.StartDate.Year >= SrbYear)
                               && lstNationCodes.Contains(x.NationCode) && lstOpponentNationCodes.Contains(x.OpponentNationCode))
                               orderby nat.Year descending, nat.StartDate descending
                               group nat by 1 into g
                               select new
                               {
                                   NationCode = nationCode.ToUpper(),
                                   NationName = nt.GetNationByLanguage(language),
                                   NationNameES = nt.GetNationByLanguage(Language.Es),
                                   OppositionNationCode = opponentNationCode.ToUpper(),
                                   OppositionNationName = ntOpponent.GetNationByLanguage(language),
                                   OppositionNationNameES = ntOpponent.GetNationByLanguage(Language.Es),
                                   WinTotal = g.Sum(x => x.WinCount),
                                   WinClay = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.WinCount),
                                   WinHard = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.WinCount),
                                   WinGrass = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.WinCount),
                                   WinCarpet = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.WinCount),
                                   WinUnknown = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.WinCount),
                                   WinIndoor = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.WinCount),
                                   WinOutdoor = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.WinCount),
                                   LossTotal = g.Sum(x => x.LossCount),
                                   LossClay = g.Where(x => x.SurfaceCode == ScC).Sum(x => x.LossCount),
                                   LossHard = g.Where(x => x.SurfaceCode == ScH).Sum(x => x.LossCount),
                                   LossGrass = g.Where(x => x.SurfaceCode == ScG).Sum(x => x.LossCount),
                                   LossCarpet = g.Where(x => x.SurfaceCode == ScA).Sum(x => x.LossCount),
                                   LossUnknown = g.Where(x => string.IsNullOrEmpty(x.SurfaceCode)).Sum(x => x.LossCount),
                                   LossIndoor = g.Where(x => x.IndoorOutdoorFlag == IfI).Sum(x => x.LossCount),
                                   LossOutdoor = g.Where(x => x.IndoorOutdoorFlag == IfO).Sum(x => x.LossCount),
                                   Ties = from d in g.ToList()
                                          select new
                                          {
                                              d.PublicTieId,
                                              ResultDesc = d.TieResultShort,
                                              Result = d.ResultCode,
                                              d.Venue,
                                              d.Year,
                                              StartDate = d.StartDate.ToString(StartDateFormat),
                                              EndDate = d.EndDate?.ToString(StartDateFormat),
                                              d.SurfaceCode,
                                              IndoorOutdoorCode = d.IndoorOutdoorFlag,
                                              Group = d.EventDivisionCode,
                                              Zone = d.EventZoneCode,
                                              DrawType = d.EventDrawsheetStructureCode,
                                              DrawClass = d.EventClassificationCode,
                                              Round = d.RoundDesc,
                                              SubGroupCode = d.EventSubGroupCode,
                                              d.PlayStatusCode
                                          }
                               }).FirstOrDefault();

                var result = Mapper.Map<HeadToHeadNationToNationViewModel>(lResult);

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

        public ICollection<NationWinLossRecordsViewModel> GetNationWinLossRecords(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationWinLossRecordsViewModel>;
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (!lstNationCodes.Any())
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var lResult = (from nat in repo.GetMany<NationActivityTie>(
                                x => x.OpponentNationCode != null && new[] { PsPc, "PU", "PA", PsNp }.Contains(x.PlayStatusCode)
                                && (nationCode != Srb || x.StartDate.Year >= SrbYear) && lstNationCodes.Contains(x.NationCode))
                               orderby nat.EndDate descending, nat.RoundNumber
                               group nat by nat.Year into g
                               select new
                               {
                                   Year = g.Key,
                                   Ties = from d in g.ToList()
                                          select new
                                          {
                                              d.OpponentNationCode,
                                              d.OpponentNationName,
                                              d.Score,
                                              d.ResultCode,
                                              Round = d.RoundDesc,
                                              StartDate = d.StartDate.FormatMediumByLanguage(language),
                                              StartDateEN = d.EndDate?.FormatMediumByLanguage(Language.En),
                                              StartDateES = d.EndDate?.FormatMediumByLanguage(Language.Es),
                                              EndDate = d.EndDate?.FormatMediumByLanguage(language),
                                              EndDateEN = d.EndDate?.FormatMediumByLanguage(Language.En),
                                              EndDateES = d.EndDate?.FormatMediumByLanguage(Language.Es),
                                              InternalTieId = d.TieID,
                                              TieId = d.PublicTieId,
                                              d.SurfaceCode,
                                              IndoorOutdoor = d.IndoorOutdoorFlag,
                                              d.HostNationCode,
                                              GroupCode = d.EventSubZoneCode,
                                              ZoneCode = d.EventZoneCode,
                                              d.EventSubGroupCode,
                                              DivisionCode = d.EventDivisionCode,
                                              ClassificationCode = d.EventClassificationCode,
                                          }
                               }).ToList();

                if (lResult.Count == 0)
                {
                    Logger.Warn($"NationWinLoss not found: {nationCode}");
                    return null;
                }

                var result = Mapper.Map<ICollection<NationWinLossRecordsViewModel>>(lResult);

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

        public ICollection<NationPlayersWinLossRecords> GetNationPlayersWinLossRecords(string nationCode, int year,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}.{year}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationPlayersWinLossRecords>;
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => lstNationCodes.Contains(x.PlayerNationalityCode) && x.Year == year && x.TieID != null)
                               group pam by new { pam.DataExchangePlayerId, pam.PlayerFamilyName, pam.PlayerGivenName } into gp
                               select new
                               {
                                   PlayerId = gp.Key.DataExchangePlayerId,
                                   gp.Key.PlayerFamilyName,
                                   gp.Key.PlayerGivenName,
                                   SinglesWins = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount),
                                   SinglesLosses = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                                   DoublesWins = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount),
                                   DoublesLosses = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount),
                                   SinglesWinLoss = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount) + " - " + gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                                   DoublesWinLoss = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount) + " - " + gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount)
                               }).OrderByDescending(x => x.SinglesWins + x.DoublesWins).ThenBy(x => x.SinglesLosses + x.DoublesLosses).ToList();

                if (lResult.Count == 0)
                {
                    Logger.Warn($"NationPlayersWinLossRecords not found: {nationCode}/{year}");
                    return null;
                }

                var result = Mapper.Map<ICollection<NationPlayersWinLossRecords>>(lResult);
                result.Where(x => x.SinglesWins + x.SinglesLosses == 0).ToList().ForEach(x => x.SinglesWinLoss = "-");
                result.Where(x => x.DoublesWins + x.DoublesLosses == 0).ToList().ForEach(x => x.DoublesWinLoss = "-");

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

        public ICollection<NationPlayersCareerRecords> GetNationPlayersCareerRecords(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationPlayersCareerRecords>;
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => lstNationCodes.Contains(x.PlayerNationalityCode) && x.TieID != null)
                               group pam by new { pam.DataExchangePlayerId, pam.PlayerFamilyName, pam.PlayerGivenName } into gp
                               orderby gp.Key.PlayerFamilyName, gp.Key.PlayerGivenName
                               select new
                               {
                                   PlayerId = gp.Key.DataExchangePlayerId,
                                   gp.Key.PlayerFamilyName,
                                   gp.Key.PlayerGivenName,
                                   SinglesWins = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount),
                                   SinglesWin = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount),
                                   SinglesLosses = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                                   SinglesLoss = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                                   DoublesWins = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount),
                                   DoublesWin = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount),
                                   DoublesLosses = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount),
                                   DoublesLoss = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount),
                                   TotalWin = gp.Where(x => McT.Contains(x.MatchTypeCode)).Sum(x => x.WinCount),
                                   TotalLoss = gp.Where(x => McT.Contains(x.MatchTypeCode)).Sum(x => x.LossCount),
                                   SinglesWinLoss = gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.WinCount) + " - " + gp.Where(x => x.MatchTypeCode == McS).Sum(x => x.LossCount),
                                   DoublesWinLoss = gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.WinCount) + " - " + gp.Where(x => x.MatchTypeCode == McD).Sum(x => x.LossCount),
                                   TotalWinLoss = gp.Where(x => McT.Contains(x.MatchTypeCode)).Sum(x => x.WinCount) + " - " + gp.Where(x => McT.Contains(x.MatchTypeCode)).Sum(x => x.LossCount),
                                   FirstYearPlayed = gp.Min(x => x.Year),
                                   TiesPlayed = gp.Select(x => x.TieID).Distinct().Count(),
                                   YearsPlayed = gp.Select(x => x.Year).Distinct().Count()
                               }).ToList();

                if (lResult.Count == 0)
                {
                    Logger.Warn($"NationPlayersCareerRecords not found: {nationCode}");
                    return null;
                }

                var result = Mapper.Map<ICollection<NationPlayersCareerRecords>>(lResult);
                result.Where(x => x.SinglesWins + x.SinglesLosses == 0).ToList().ForEach(x => x.SinglesWinLoss = "-");
                result.Where(x => x.DoublesWins + x.DoublesLosses == 0).ToList().ForEach(x => x.DoublesWinLoss = "-");
                result.Where(x => x.TotalWin + x.TotalLoss == 0).ToList().ForEach(x => x.TotalWinLoss = "-");

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

        public NationStatsRecordsViewModel GetNationStatsRecords(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as NationStatsRecordsViewModel;
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var result = new NationStatsRecordsViewModel();

                var lResultAy = (from t in repo.GetMany<NationPlayersAgeYoungest>(x => lstNationCodes.Contains(x.NationCode))
                               orderby t.PlayerAgeInYears, t.PlayerAgeInDays
                               select new
                               {
                                   YoungestPlayerId = t.DataExchangePlayerId,
                                   YoungestPlayerName = t.PlayerDisplayName,
                                   YoungestPlayerDO = t.PlayerBirthDate.FormatMediumByLanguage(language),
                                   YoungestPlayerDOBEN = t.PlayerBirthDate.FormatMediumByLanguage(Language.En),
                                   YoungestPlayerDOBES = t.PlayerBirthDate.FormatMediumByLanguage(Language.Es),
                                   YoungestPlayerAgeYearsPlayed = t.PlayerAgeInYears,
                                   YoungestPlayerAgeDaysPlayed = t.PlayerAgeInDays,
                                   YoungestPlayerPlayedDate = t.StartDate.FormatMediumByLanguage(language),
                                   YoungestPlayerPlayedDateEN = t.StartDate.FormatMediumByLanguage(Language.En),
                                   YoungestPlayerPlayedDateES = t.StartDate.FormatMediumByLanguage(Language.Es)
                               }).FirstOrDefault();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                if (lResultAy != null)
                {
                    result = Mapper.Map(lResultAy, result);
                }
                else
                {
                    result.YoungestPlayerName = Na;
                }

                var lResultAo = (from t in repo.GetMany<NationPlayersAgeOldest>(x => lstNationCodes.Contains(x.NationCode))
                                orderby t.PlayerAgeInYears descending, t.PlayerAgeInDays descending
                                select new
                                {
                                    OldestPlayerId = t.DataExchangePlayerId,
                                    OldestPlayerName = t.PlayerDisplayName,
                                    OldestPlayerDO = t.PlayerBirthDate.FormatMediumByLanguage(language),
                                    OldestPlayerDOBEN = t.PlayerBirthDate.FormatMediumByLanguage(Language.En),
                                    OldestPlayerDOBES = t.PlayerBirthDate.FormatMediumByLanguage(Language.Es),
                                    OldestPlayerAgeYearsPlayed = t.PlayerAgeInYears,
                                    OldestPlayerAgeDaysPlayed = t.PlayerAgeInDays,
                                    OldestPlayerPlayedDate = t.StartDate.FormatMediumByLanguage(language),
                                    OldestPlayerPlayedDateEN = t.StartDate.FormatMediumByLanguage(Language.En),
                                    OldestPlayerPlayedDateES = t.StartDate.FormatMediumByLanguage(Language.Es)
                                }).FirstOrDefault();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                if (lResultAo != null)
                {
                    result = Mapper.Map(lResultAo, result);
                }
                else
                {
                    result.OldestPlayerName = Na;
                }

                var lResultLrd = (from t in repo.GetMany<NationLongestRubberInDuration>(x => lstNationCodes.Contains(x.NationCode))
                                 let winningSide = string.IsNullOrEmpty(t.WinningPlayer2DisplayName) ? t.WinningPlayer1DisplayName : $"{t.WinningPlayer1DisplayName}/{t.WinningPlayer2DisplayName}"
                                 let losingSide = string.IsNullOrEmpty(t.LosingPlayer2DisplayName) ? t.LosingPlayer1DisplayName : $"{t.LosingPlayer1DisplayName}/{t.LosingPlayer2DisplayName}"
                                 orderby t.Hours descending, t.Minutes descending
                                 select new
                                 {
                                     LongestRubberDurationTime = $"{t.Hours}:{t.Minutes}",
                                     LongestRubberDurationResult = $"Rubber {t.RubberNumber}: {winningSide} defeated {losingSide}",
                                     LongestRubberDurationTieId = t.PublicTieId
                                 }).FirstOrDefault();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                if (lResultLrd != null)
                {
                    result = Mapper.Map(lResultLrd, result);
                }

                var lResultLtd = (from t in repo.GetMany<NationLongestTieInDurationHeader>(x => lstNationCodes.Contains(x.NationCode))
                                 let tieResult = t.WinCode == "l" ? "lost to " : "defeated "
                                 orderby t.Hours descending, t.Minutes descending
                                 select new
                                 {
                                     LongestTieDurationTime = $"{t.Hours}:{t.Minutes}",
                                     LongestTieDurationResult = $"{t.NationName} {tieResult} {t.OpponentNationName} ({t.NationScore} - {t.OpponentNationScore})",
                                     LongestTieDurationTieId = t.PublicTieId
                                 }).FirstOrDefault();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                if (lResultLtd != null)
                {
                    result = Mapper.Map(lResultLtd, result);
                }

                //var maxTieBreakTotal = repo.GetMany<NationLongestTieBreakInPoints>(x => lstNationCodes.Contains(x.NationCode)).Select(x => x.TieBreakTotal).OrderByDescending(x => x).FirstOrDefault();
                var lResultLtb = (//from t in repo.GetMany<NationLongestTieBreakInPoints>(x => lstNationCodes.Contains(x.NationCode) && x.TieBreakTotal == maxTieBreakTotal)
                                  from t in repo.GetMany<NationLongestTieBreakInPoints>(x => lstNationCodes.Contains(x.NationCode)).GroupBy(x => x.TieBreakTotal).OrderByDescending(x => x.Key).FirstOrDefault()
                                  let winningSide = string.IsNullOrEmpty(t.WinningPlayer2DisplayName) ? t.WinningPlayer1DisplayName : $"{t.WinningPlayer1DisplayName}/{t.WinningPlayer2DisplayName}"
                                  let losingSide = string.IsNullOrEmpty(t.LosingPlayer2DisplayName) ? t.LosingPlayer1DisplayName : $"{t.LosingPlayer1DisplayName}/{t.LosingPlayer2DisplayName}"
                                  orderby t.TieBreakTotal descending, t.Year
                                  select new
                                  {
                                      TieId = t.PublicTieId,
                                      TotalPoints = t.TieBreakTotal,
                                      TieBreakScore = $"{t.TieBreakWinningScore}/{t.TieBreakLosingScore}",
                                      RubberResult = $"{winningSide} defeated {losingSide}"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.LongestTieBreak = Mapper.Map<ICollection<NationStatsLongestTieBreak>>(lResultLtb);

                //var maxNumberOfGamesFinalSet = repo.GetMany<NationLongestFinalSet>(x => lstNationCodes.Contains(x.NationCode)).Select(x => x.NumberOfGames).OrderByDescending(x => x).FirstOrDefault();
                var lResultLfs = (//from t in repo.GetMany<NationLongestFinalSet>(x => lstNationCodes.Contains(x.NationCode) && x.NumberOfGames == maxNumberOfGamesFinalSet)
                                  from t in repo.GetMany<NationLongestFinalSet>(x => lstNationCodes.Contains(x.NationCode)).GroupBy(x => x.NumberOfGames).OrderByDescending(x => x.Key).FirstOrDefault()
                                  let winningSide = string.IsNullOrEmpty(t.WinningPlayer2DisplayName) ? t.WinningPlayer1DisplayName : $"{t.WinningPlayer1DisplayName}/{t.WinningPlayer2DisplayName}"
                                  let losingSide = string.IsNullOrEmpty(t.LosingPlayer2DisplayName) ? t.LosingPlayer1DisplayName : $"{t.LosingPlayer1DisplayName}/{t.LosingPlayer2DisplayName}"
                                  orderby t.NumberOfGames descending, t.Year
                                  select new
                                  {
                                      TieId = t.PublicTieId,
                                      TotalGames = t.NumberOfGames,
                                      SetScore = $"{t.WinningScore}/{t.LosingScore}",
                                      RubberResult = $"{winningSide} defeated {losingSide}"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.LongestFinalSet = Mapper.Map<ICollection<NationStatsLongestFinalSet>>(lResultLfs);

                var maxNumberOfGamesInRubber = repo.GetMany<NationMostGamesInRubber>(x => lstNationCodes.Contains(x.NationCode)).Select(x => x.NumberOfGames).OrderByDescending(x => x).FirstOrDefault();
                var lResultMgr = (from t in repo.GetMany<NationMostGamesInRubber>(x => lstNationCodes.Contains(x.NationCode) && x.NumberOfGames == maxNumberOfGamesInRubber)
                                  //from t in repo.GetMany<NationMostGamesInRubber>(x => lstNationCodes.Contains(x.NationCode)).GroupBy(x => x.NumberOfGames).OrderByDescending(x => x.Key).FirstOrDefault()
                                  let winningSide = t.MatchTypeCode == McD ? $"{t.WinningPlayer1DisplayName}/{t.WinningPlayer2DisplayName}" : t.WinningPlayer1DisplayName
                                  let losingSide = t.MatchTypeCode == McD ? $"{t.LosingPlayer1DisplayName}/{t.LosingPlayer2DisplayName}" : t.LosingPlayer1DisplayName
                                  orderby t.NumberOfGames descending, t.Year
                                  select new
                                  {
                                      TieId = t.PublicTieId,
                                      TotalGames = t.NumberOfGames,
                                      RubberScore = t.Score,
                                      RubberResult = $"{winningSide} defeated {losingSide}"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.MostGamesInRubber = Mapper.Map<ICollection<NationStatsMostGamesInRubber>>(lResultMgr);

                var maxNumberOfGamesInSet = repo.GetMany<NationMostGamesInSet>(x => lstNationCodes.Contains(x.NationCode)).Select(x => x.NumberOfGames).OrderByDescending(x => x).FirstOrDefault();
                var lResultMgs = (from t in repo.GetMany<NationMostGamesInSet>(x => lstNationCodes.Contains(x.NationCode) && x.NumberOfGames == maxNumberOfGamesInSet)
                                  //from t in repo.GetMany<NationMostGamesInSet>(x => lstNationCodes.Contains(x.NationCode)).GroupBy(x => x.NumberOfGames).OrderByDescending(x => x.Key).FirstOrDefault()
                                  let winningSide = string.IsNullOrEmpty(t.WinningPlayer2DisplayName) ? t.WinningPlayer1DisplayName : $"{t.WinningPlayer1DisplayName}/{t.WinningPlayer2DisplayName}"
                                  let losingSide = string.IsNullOrEmpty(t.LosingPlayer2DisplayName) ? t.LosingPlayer1DisplayName : $"{t.LosingPlayer1DisplayName}/{t.LosingPlayer2DisplayName}"
                                  orderby t.NumberOfGames descending, t.Year
                                  select new
                                  {
                                      TieId = t.PublicTieId,
                                      TotalGames = t.NumberOfGames,
                                      RubberScore = t.Score,
                                      RubberResult = $"{winningSide} defeated {losingSide}"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.MostGamesInSet = Mapper.Map<ICollection<NationStatsMostGamesInSet>>(lResultMgs);

                var maxNumberOfGamesInTieHeader = repo.GetMany<NationMostGamesInTieHeader>(x => lstNationCodes.Contains(x.NationCode)).Select(x => x.NumberOfGames).OrderByDescending(x => x).FirstOrDefault();
                if (maxNumberOfGamesInTieHeader != 0) {
                var lResultMgt = (//from t in repo.GetMany<NationMostGamesInTieHeader>(x => lstNationCodes.Contains(x.NationCode) && x.NumberOfGames == maxNumberOfGamesInTieHeader)
                                  from t in repo.GetMany<NationMostGamesInTieHeader>(x => lstNationCodes.Contains(x.NationCode)).GroupBy(x => x.NumberOfGames).OrderByDescending(x => x.Key).FirstOrDefault()
                                  let tieResult = t.WinCode == "l" ? "lost to " : "defeated "
                                  orderby t.NumberOfGames descending, t.Year
                                  select new
                                  {
                                      TieId = t.PublicTieId,
                                      TotalGames = t.NumberOfGames,
                                      TieResult = $"{t.NationName} {tieResult} {t.OpponentNationName} ({t.NationScore} - {t.OpponentNationScore})"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.MostGamesInTie = Mapper.Map<ICollection<NationStatsMostGamesInTie>>(lResultMgt);
                }

                var lResultMdv = (from t in repo.GetMany<NationMostDecisiveVictoryInTieHeader>(x => lstNationCodes.Contains(x.NationCode))
                                  orderby t.SetsWon descending, t.SetsLost, t.GamesWon - t.GamesLost descending , t.Year, t.PublicTieId
                                  select new
                                  {
                                      TieId = t.PublicTieId,
                                      SetsWinLoss = $"{t.SetsWon} - {t.SetsLost}",
                                      GamesWinLoss = $"{t.GamesWon} - {t.GamesLost}",
                                      TieResult = $"{t.NationName} defeated {t.OpponentNationName} ({t.NationScore} - {t.OpponentNationScore})"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.MostDecisiveVictoryInTie = Mapper.Map<ICollection<NationStatsMostDecisiveVictoryInTie>>(lResultMdv);

                var lResultLwt = (from t in repo.GetMany<NationLongestWinningRunInTiesHeader>(x => lstNationCodes.Contains(x.NationCode), true)
                                  from td in repo.GetMany<NationLongestWinningRunInTiesDetail>(x => x.RunID == t.RunID)
                                  orderby t.NumberOfTies descending, t.RunID, td.StartDate
                                  select new
                                  {
                                      t.NumberOfTies,
                                      TieId = td.PublicTieId,
                                      TieStartDate = td.StartDate.FormatMediumByLanguage(language),
                                      TieEndDate = td.EndDate.FormatMediumByLanguage(language),
                                      TieEvent = td.DivisionCode
                                      + (string.IsNullOrEmpty(td.ZoneCode) ? string.Empty : ", " + td.ZoneCode)
                                      + (string.IsNullOrEmpty(td.SubZoneCode) ? string.Empty : " (" + td.SubZoneCode + ")")
                                      + (string.IsNullOrEmpty(td.RoundDesc) ? string.Empty : ", " + td.RoundDesc),
                                      TieResult = $"{td.WinningNationName} {td.WinningNationScore} - {td.LosingOpponentNationScore} {td.LosingOpponentNationName}"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.LongestWinRunNumber = lResultLwt.FirstOrDefault()?.NumberOfTies;
                result.LongestWinRun = Mapper.Map<ICollection<NationStatsLongestWinRun>>(lResultLwt);

                var lResultCtn = (from t in repo.GetMany<NationComebacksFromTwoNilDownHeader>(x => lstNationCodes.Contains(x.NationCode), true)
                                  from tie in repo.GetMany<Tie>(x => x.TieID == t.TieID)
                                  orderby t.StartDate descending
                                  select new
                                  {
                                      TieId = tie.PublicTieId,
                                      TieStartDate = t.StartDate.FormatMediumByLanguage(language),
                                      TieEndDate = t.EndDate.FormatMediumByLanguage(language),
                                      TieEvent = string.Join(", ", new[] { t.DivisionCode, t.ZoneCode }.Where(x => !string.IsNullOrEmpty(x)))
                                      + (string.IsNullOrEmpty(t.SubZoneCode) ? string.Empty : " (" + t.SubZoneCode + ")")
                                      + (string.IsNullOrEmpty(t.RoundDesc) ? string.Empty : ", " + t.RoundDesc),
                                      TieResult = $"{t.NationName} {t.NationScore} - {t.OpponentNationScore} {t.OpponentNationName}"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.ComebackTwoNilDown = Mapper.Map<ICollection<NationStatsComebackTwoNilDown>>(lResultCtn);

                var lResultCto = (from t in repo.GetMany<NationComebacksFromTwoOneDownHeader>(x => lstNationCodes.Contains(x.NationCode), true)
                                  from tie in repo.GetMany<Tie>(x => x.TieID == t.TieID)
                                  orderby t.StartDate descending
                                  select new
                                  {
                                      TieId = tie.PublicTieId,
                                      TieStartDate = t.StartDate.FormatMediumByLanguage(language),
                                      TieEndDate = t.EndDate.FormatMediumByLanguage(language),
                                      TieEvent = string.Join(", ", new [] {t.DivisionCode, t.ZoneCode}.Where(x => !string.IsNullOrEmpty(x)))
                                      + (string.IsNullOrEmpty(t.SubZoneCode) ? string.Empty : " (" + t.SubZoneCode + ")")
                                      + (string.IsNullOrEmpty(t.RoundDesc) ? string.Empty : ", " + t.RoundDesc),
                                      TieResult = $"{t.NationName} {t.NationScore} - {t.OpponentNationScore} {t.OpponentNationName}"
                                  }).ToList();
                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                result.ComebackTwoOneDown = Mapper.Map<ICollection<NationStatsComebackTwoOneDown>>(lResultCto);

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

        public ICollection<NationRankingViewModel> GetNationRankings(int top = 0,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{top}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationRankingViewModel>;
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

                var lResult = from nr in repo.GetAllSpecific<NationRank>()
                              join nt in repo.GetAll<NationTranslated>(true) on nr.NationCode equals nt.NationCode
                              orderby nr.Rank
                              select new
                              {
                                  nr.Rank,
                                  nr.NationCode,
                                  Nation = nt.GetNationByLanguage(language),
                                  NationEN = nt.GetNationByLanguage(Language.En),
                                  NationES = nt.GetNationByLanguage(Language.Es),
                                  Points = nr.RankingPoints,
                                  Played = nr.TotalTiesPlayed,
                                  RankEqual = nr.RankEqualFlag,
                                  Movement = nr.RankPrev.HasValue && nr.RankPrev.Value > 0 ? (nr.RankPrev.Value - nr.Rank).ToString() : "New",
                                  RankDate = nr.RankDate?.ToString(EndDateFormat)
                              };

                lResult = top != 0 ? lResult.Take(top).ToList() : lResult.ToList();
                var result = Mapper.Map<ICollection<NationRankingViewModel>>(lResult);

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

        public NationAllTimeRecordsViewModel GetNationAllTimeRecords(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as NationAllTimeRecordsViewModel;
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var result = new NationAllTimeRecordsViewModel();

                var lstWherePredicates = new[] {
                    (Func<PlayerActivityMatch, bool>)(x => true),//MostTotalWins
                    x => x.MatchTypeCode == McS,//MostSinglesWins
                    x => x.MatchTypeCode == McD//MostDoublesWins
                };
                for (var i = 0; i < lstWherePredicates.Length; i++)
                {
                    var whereMatchCode = lstWherePredicates[i];
                    var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => lstNationCodes.Contains(x.PlayerNationalityCode) && (nationCode != Srb || x.Year >= SrbYear))
                                   join p in repo.GetAll<Player>() on pam.DataExchangePlayerId equals p.DataExchangePlayerId // MORE PERFORMANT
                                   //from p in repo.GetMany<Player>(x => x.DataExchangePlayerId == pam.DataExchangePlayerId)
                                   where pam.PlayStatusCode.SqlNotEquals(PsNp)
                                   group pam by new
                                   {
                                       PlayerId = pam.DataExchangePlayerId,
                                       FullName = p.GivenName + " " + p.FamilyName
                                   } into g
                                   select new
                                   {
                                       g.Key.PlayerId,
                                       g.Key.FullName,
                                       Wins = g.Where(whereMatchCode).Sum(x => x.WinCount),
                                       Losses = g.Where(whereMatchCode).Sum(x => x.LossCount)
                                   }).OrderByDescending(x => x.Wins).ThenBy(x => x.Losses).Take(3).ToList();

                    //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");

                    string mostWins = null;
                    if (lResult.Count > 0)
                    {
                        mostWins = string.Join("|", lResult.Where(x => x.Wins >= lResult[0].Wins).Select(
                            x => $"{x.FullName} ({x.Wins} - {x.Losses})#{x.PlayerId}"));
                    }
                    if (i == 0)
                    {
                        result.MostTotalWins = mostWins;
                    }
                    else if (i == 1)
                    {
                        result.MostSinglesWins = mostWins;
                    }
                    else if (i == 2)
                    {
                        result.MostDoublesWins = mostWins;
                    }
                }

                var lstSelectPredicates = new[] {
                    (Func<PlayerActivityMatch, object>)(x => x.TieID),//MostTiesPlayed
                    x => x.Year,//MostYearsPlayed
                };
                for (var i = 0; i < lstSelectPredicates.Length; i++)
                {
                    var predicate = lstSelectPredicates[i];
                    var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => lstNationCodes.Contains(x.PlayerNationalityCode) && (nationCode != Srb || x.Year >= SrbYear))
                                   join p in repo.GetAll<Player>() on pam.DataExchangePlayerId equals p.DataExchangePlayerId // MORE PERFORMANT
                                   //from p in repo.GetMany<Player>(x => x.DataExchangePlayerId == pam.DataExchangePlayerId)
                                   where pam.PlayStatusCode.SqlNotEquals(PsNp)
                                   group pam by new
                                   {
                                       PlayerId = pam.DataExchangePlayerId,
                                       FullName = p.GivenName + " " + p.FamilyName
                                   } into g
                                   select new
                                   {
                                       g.Key.PlayerId,
                                       g.Key.FullName,
                                       MostPlayed = g.Select(predicate).Distinct().Count()
                                   }).OrderByDescending(x => x.MostPlayed).Take(3).ToList();

                    //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");

                    string mostPlayed = null;
                    if (lResult.Count > 0)
                    {
                        mostPlayed = string.Join("|", lResult.Where(x => x.MostPlayed >= lResult[0].MostPlayed).Select(
                            x => $"{x.FullName} ({x.MostPlayed})#{x.PlayerId}"));
                    }
                    if (i == 0)
                    {
                        result.MostTiesPlayed = mostPlayed;
                    }
                    else if (i == 1)
                    {
                        result.MostYearsPlayed = mostPlayed;
                    }
                }

                var bestDoubleResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => lstNationCodes.Contains(x.PlayerNationalityCode) && x.MatchTypeCode == "D" && (nationCode != Srb || x.Year >= SrbYear))
                                        join ppl in repo.GetAll<Player>() on pam.DataExchangePlayerId equals ppl.DataExchangePlayerId // MORE PERFORMANT
                                        join ppa in repo.GetAll<Player>() on pam.PartnerDataExchangePlayerId equals ppa.DataExchangePlayerId // MORE PERFORMANT
                                        //from ppl in repo.GetMany<Player>(x => x.DataExchangePlayerId == pam.DataExchangePlayerId)
                                        //from ppa in repo.GetMany<Player>(x => x.DataExchangePlayerId == pam.DataExchangePlayerId)
                                        where pam.PlayStatusCode.SqlNotEquals(PsNp)
                                        && string.Compare((ppl.FamilyName + " " + ppl.GivenName), ppa.FamilyName + " " + ppa.GivenName, StringComparison.Ordinal) < 0
                                        group new { pam, ppl, ppa } by new
                                        {
                                            PlayerId = pam.DataExchangePlayerId,
                                            ParnerId = pam.PartnerDataExchangePlayerId,
                                            PlayerFullName = ppl.GivenName + " " + ppl.FamilyName,
                                            PartnerFullName = ppa.GivenName + " " + ppa.FamilyName
                                        } into g
                                        select new
                                        {
                                            g.Key.PlayerId,
                                            g.Key.ParnerId,
                                            g.Key.PlayerFullName,
                                            g.Key.PartnerFullName,
                                            Wins = g.Sum(x => x.pam.WinCount),
                                            Losses = g.Sum(x => x.pam.LossCount)
                                        }).OrderByDescending(x => x.Wins).ThenBy(x => x.Losses).Take(5).ToList();

                //Logger.Debug($"PERF.{CurrentLineNumber}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");

                result.BestDoublesPair = string.Join("|", bestDoubleResult.Where(x => x.Wins >= bestDoubleResult[0].Wins).Select(
                            x => $"{x.PlayerFullName}/{x.PartnerFullName} ({x.Wins} - {x.Losses})#{x.PlayerId}/{x.ParnerId}"));

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

        public ICollection<NationsGroupViewModel> GetNationsGroup(int year,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{year}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationsGroupViewModel>;
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
                var lResult = (from ng in (from ng in repo.GetMany<NationGrouping>(x => x.Year == year)
                                           select new { ng.NationCode, ng.ZoneCode, ng.DivisionCode })
                                          .Union(from ng in repo.GetMany<TeamNomination>(x => x.CalendarYear == year)
                                                 select new { ng.NationCode, ng.ZoneCode, ng.DivisionCode })
                               join nt in repo.GetAll<NationTranslated>(true) on ng.NationCode equals nt.NationCode
                               select new
                               {
                                   nt.NationCode,
                                   Nation = nt.GetNationByLanguage(language),
                                   NationI = nt.GetNationIByLanguage(language),
                                   NationEN = nt.GetNationByLanguage(Language.En),
                                   NationENI = nt.GetNationIByLanguage(Language.En),
                                   NationES = nt.GetNationByLanguage(Language.Es),
                                   NationESI = nt.GetNationIByLanguage(Language.Es),
                                   ng.DivisionCode,
                                   ZoneCode = ng.ZoneCode.GetZoneCode(ng.DivisionCode),
                                   ZoneCodeMaster = ng.ZoneCode.GetZoneCode(ng.DivisionCode).Replace("EUR", "EPA").Replace("AFR", "EPA")
                               }).ToList();

                var result = Mapper.Map<ICollection<NationsGroupViewModel>>(lResult);

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

        public ICollection<PlayerViewModelCore> GetNationPlayers(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }

                var lResult = (from pam in repo.GetMany<PlayerActivityMatch>(x => lstNationCodes.Contains(x.PlayerNationalityCode) && (nationCode != Srb || x.Year >= SrbYear))
                               join pl in repo.GetAll<Player>() on pam.DataExchangePlayerId equals pl.DataExchangePlayerId
                               orderby pl.FamilyName, pl.GivenName
                               select new
                               {
                                   PlayerId = pl.DataExchangePlayerId,
                                   pl.GivenName,
                                   pl.FamilyName
                               }).Distinct().ToList();

                var result = Mapper.Map<ICollection<PlayerViewModelCore>>(lResult);

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

        public ICollection<PlayerViewModelCore> GetNationRecentPlayers(string nationCode, int recentYears,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}.{recentYears}";
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
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }
                var lYears = (from tie in repo.GetMany<Tie>(x => (lstNationCodes.Contains(x.Side1NationCode) || lstNationCodes.Contains(x.Side2NationCode)) && (nationCode != Srb || x.Year >= SrbYear))
                              orderby tie.Year descending
                              select tie.Year).Distinct().Take(recentYears).ToList();

                var lRecentYears = (from tie in repo.GetMany<Tie>(x => (lstNationCodes.Contains(x.Side1NationCode) || lstNationCodes.Contains(x.Side2NationCode)) && (nationCode != Srb || x.Year >= SrbYear)
                                    && lYears.Contains(x.Year))
                                          //join pam in repo.GetAll<PlayerActivityMatch>() on tie.TieID equals pam.TieID
                                      from pam in repo.GetMany<PlayerActivityMatch>(x => x.TieID == tie.TieID)
                                      where pam.ResultCode.SqlNotEquals(RcN)
                                      orderby pam.EndDate descending, pam.TieRoundNumber descending, pam.TieID, pam.RoundNumber
                                      group new { tie, pam } by tie.Year into g
                                      select new
                                      {
                                          Year = g.Key,
                                          Players = from gl in g.ToList()
                                                    select new
                                                    {
                                                        PlayerID = lstNationCodes.Contains(gl.pam.PlayerTieNationCode) ? gl.pam.DataExchangePlayerId : gl.pam.OpponentDataExchangePlayerId,
                                                        PartnerPlayerID = lstNationCodes.Contains(gl.pam.PlayerTieNationCode) ? gl.pam.PartnerDataExchangePlayerId : gl.pam.OpponentPartnerDataExchangePlayerId
                                                    }
                                      });

                var lRecentPlayers = lRecentYears.SelectMany(x => x.Players).Distinct().ToList();

                var result = new List<PlayerViewModelCore>();
                lRecentPlayers.ForEach(x =>
                {
                    new[] { x.PlayerID, x.PartnerPlayerID }.ToList().ForEach(y =>
                    {
                        if (y.HasValue && result.All(z => z.PlayerId != y.Value))
                        {
                            result.Add(Mapper.Map<PlayerViewModelCore>(_playerService.GetPlayerCore(y.Value, language, source, useCache)));
                        }
                    });
                });

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

        private IEnumerable<Tie> GetChampionTies(DataSource source)
        {
            var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;
            var now = DateTime.Now;
            return repo.GetMany<Tie>(
                x => x.DrawsheetRoundCode == DrcF && EdcWg.Contains(x.EventDivisionCode) && x.EndDate < now
                     //&& (x.ResultStatusCode == null || x.ResultStatusCode != RscW)
                     && (source != DataSource.Fc || x.EventClassificationCode == EccM)
                     && !ExcludingTieIds.Contains(x.TieID)
                     && x.PlayStatusCode != null && x.PlayStatusCode != PsIp
                     && x.Year != 1901 && x.Year != 1910
                     && x.Side1NationCode != null && x.Side2NationCode != null);
        }

        public ICollection<ChampionViewModel> GetChampions(Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<ChampionViewModel>;
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
                var lResult = (from tie in GetChampionTies(source)
                               join ntC in repo.GetAll<NationTranslated>(true)
                                    on tie.WinningSide == 1 ? tie.Side1NationCode : tie.Side2NationCode equals ntC.NationCode
                               join ntF in repo.GetAll<NationTranslated>(true)
                                    on tie.WinningSide == 2 ? tie.Side1NationCode : tie.Side2NationCode equals ntF.NationCode
                               orderby tie.Year descending
                               select new
                               {
                                   TieId = tie.PublicTieId,
                                   tie.Year,
                                   ChampionNationName = ntC.GetNationByLanguage(language),
                                   ChampionNationCode = ntC.NationCode,
                                   ChampionScore = tie.WinningSide == 1 ? tie.Side1Score : tie.Side2Score,
                                   FinalistNationName = ntF.GetNationByLanguage(language),
                                   FinalistNationCode = ntF.NationCode,
                                   FinalistScore = tie.WinningSide == 2 ? tie.Side1Score : tie.Side2Score,
                                   tie.Venue
                               }).ToList();

                var result = Mapper.Map<ICollection<ChampionViewModel>>(lResult);

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

        public ICollection<NationCoreViewModel> GetNodeRelatedNations(int nodeId, Language language = Language.En,
            DataSource source = DataSource.Dc, bool useCache = true)
        {

            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nodeId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationCoreViewModel>;
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

                var result = 
                    (from relationship in
                        CmsRepo.GetMany<ITFRelationships>(relationship => relationship.assetId == nodeId
                                                                          && relationship.relatedAssetTypeId == 27)
                        from nation in
                        CmsRepo.GetMany<ITFNationBaseline>(nation => nation.NationId == relationship.relatedAssetId)
                        join nationTranslated in repo.GetAll<NationTranslated>(true) on nation.NationCode equals
                        nationTranslated.NationCode
                        orderby relationship.sortOrder
                        select new NationCoreViewModel
                        {
                            NationCode = nation.NationCode,
                            Nation = nationTranslated.GetNationByLanguage(language),
                            NationI = nationTranslated.GetNationIByLanguage(language)
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

        public ICollection<NationCoreViewModel> SearchNations(string searchText, Language language = Language.En,
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
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<NationCoreViewModel>;
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

                var result =
                (from nationTranslated in repo.GetMany<NationTranslated>(x => x.NationCode.Contains(searchText) ||
                                                                              (language == Language.En
                                                                                  ? x.NationName
                                                                                  : x.NationNameSpanish).Contains(
                                                                                  searchText))
                    select new NationCoreViewModel
                    {
                        NationCode = nationTranslated.NationCode,
                        Nation = nationTranslated.GetNationByLanguage(language),
                        NationI = nationTranslated.GetNationIByLanguage(language)
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

        #endregion

        #region BTD customization

        public const int RecentYears = 3;

        public BtdNationViewModel GetBtdNation(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as BtdNationViewModel;
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

                var nation = Mapper.Map<NationViewModel>(GetNation(nationCode, language, source)); //useCache = true
                if (nation == null)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }
                var result = new BtdNationViewModel
                {
                    Nation = nation,
                    NationProfile = Mapper.Map<NationProfileWebViewModel>(GetNationProfileWeb(nationCode, language, source)), //useCache = true
                    NationAllTimeRecords = GetNationAllTimeRecords(nationCode, language, source) //useCache = true
                };

                result.NextTie = _eventService.GetTie(result.NationProfile.NextTieId, language, source, useCache);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"Get nation core and statistic info, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                stopWatch.Restart();

                result.RecentPlayers = GetNationRecentPlayers(nationCode, RecentYears, language, source, useCache);

                stopWatch.Stop();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"Recent Players all info: Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;
                var lstNationCodes = repo.GetMany<NationHistory>(x => x.NationCode == nationCode, true).Select(x => x.HistoricNationCode).ToList();
                if (lstNationCodes.Count == 0)
                {
                    Logger.Warn($"Nation not found: {nationCode}");
                    return null;
                }
                stopWatch.Restart();
                var lRecentResults = (from tie in repo.GetMany<Tie>(x => (lstNationCodes.Contains(x.Side1NationCode) || lstNationCodes.Contains(x.Side2NationCode))
                                      && (nationCode != Srb || x.Year >= SrbYear) && x.WinningSide != null)
                                      orderby tie.Year descending
                                      group tie by tie.Year into g
                                      select new
                                      {
                                          Year = g.Key,
                                          Results = from gy in g.ToList()
                                                    orderby gy.DrawsheetRoundNumber
                                                    select new
                                                    {
                                                        TieId = gy.PublicTieId,
                                                        TieYear = gy.Year,
                                                        EventDivision = gy.EventDivisionCode,
                                                        EventRound = gy.DrawsheetRoundDesc,
                                                        Side1NationCode = lstNationCodes.Contains(gy.Side1NationCode) ? gy.Side1NationCode : gy.Side2NationCode,
                                                        Side1NationName = lstNationCodes.Contains(gy.Side1NationCode) ? gy.Side1NationName : gy.Side2NationName,
                                                        Side1Score = lstNationCodes.Contains(gy.Side1NationCode) ? gy.Side1Score : gy.Side2Score,
                                                        Side2NationCode = lstNationCodes.Contains(gy.Side1NationCode) ? gy.Side2NationCode : gy.Side1NationCode,
                                                        Side2NationName = lstNationCodes.Contains(gy.Side1NationCode) ? gy.Side2NationName : gy.Side1NationName,
                                                        Side2Score = lstNationCodes.Contains(gy.Side1NationCode) ? gy.Side2Score : gy.Side1Score,
                                                        Score = lstNationCodes.Contains(gy.Side1NationCode) ? gy.Score : gy.ScoreReversed,
                                                        ResultCode = lstNationCodes.Contains(gy.Side1NationCode) ?
                                                        (gy.Side1Score > gy.Side2Score ? RcW : RcL) : (gy.Side2Score > gy.Side1Score ? RcW : RcL)
                                                    },
                                          Players = GetNationPlayersWinLossRecords(nationCode, g.Key ?? 0, language, source, useCache)
                                      }).Take(RecentYears).ToList();
                result.RecentResults = Mapper.Map<List<BtdRecentResultViewModel>>(lRecentResults);

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

        public BtdMyTeamViewModel GetBtdMyTeam(string nationCode,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{nationCode}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as BtdMyTeamViewModel;
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

                var result = Mapper.Map<BtdMyTeamViewModel>(_cmsService.GetCmsNationProfileWeb(nationCode, language, source, useCache));

                if (result == null)
                {
                    Logger.Warn($"Nation not found: nationCode={nationCode}");
                    return null;
                }

                result.NextTie = _eventService.GetTie(result.NextTieId, language, source, useCache);
                result.LastTie = _eventService.GetTie(result.LastTieId, language, source, useCache);

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