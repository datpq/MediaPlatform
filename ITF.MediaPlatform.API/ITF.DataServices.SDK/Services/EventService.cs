using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.SDK.Data;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models;
using ITF.DataServices.SDK.Models.Cms;
using ITF.DataServices.SDK.Models.Cup;
using ITF.DataServices.SDK.Models.ViewModels;
using Ninject;
using NLog;

namespace ITF.DataServices.SDK.Services
{
    public class EventService : CupService, IEventService
    {
        private readonly IPlayerService _playerService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EventService(
            [Named("DavisCupRepo")] ICupDataRepository davisCupRepo,
            [Named("FedCupRepo")] ICupDataRepository fedCupRepo,
            [Named("CmsRepo")] ISameStructureDataRepository cmsRepo,
            IPlayerService playerService,
            ICacheConfigurationService cacheConfigurationService)
            : base(davisCupRepo, fedCupRepo, cmsRepo, cacheConfigurationService)
        {
            _playerService = playerService;
        }

        public ICollection<EventYearViewModel> GetEventYears(string section, string subSection,
            DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{source}.{section}.{subSection}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<EventYearViewModel>;
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
                var lResult = (from e in repo.GetMany<Event>(x => x.DivisionCode == section && x.DrawsheetStructureCode == "KO" &&
                            ((EdcWg.Contains(section) && x.EventClassificationCode == subSection && (section != "WG" || x.Year > 1980))
                            || (!EdcWg.Contains(section) && x.EventClassificationCode == "M" && x.SubZoneCode == null && x.Year >= 1994 &&
                                ((subSection == "EPA" && (x.ZoneCode == subSection || x.ZoneCode == "EA"))
                                || (subSection == "AM" && (x.ZoneCode == subSection || x.ZoneCode == "AMN"))
                                || (subSection == "AO" && x.ZoneCode == subSection)))))
                               orderby e.Year descending
                               select new { e.Year }).Distinct().ToList();

                var result = Mapper.Map<ICollection<EventYearViewModel>>(lResult);

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

        public DrawSheetViewModel GetDrawSheet(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{language}.{source}.{year}.{section}.{subSection}";
                var cacheTimeout = GetCacheTimeout(cachePrefix, cacheKey);
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as DrawSheetViewModel;
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

                var lResult = (from e in repo.GetMany<Event>(x => x.Year == year
                               && ((source == DataSource.Fc && (section == null || (x.DivisionCode == "WG1" && section == "WG") || x.DivisionCode == section) && (subSection == null || x.EventClassificationCode == subSection))
                               || (source == DataSource.Dc && ((EdcWg.Contains(x.DivisionCode) && (subSection == null || x.EventClassificationCode == subSection))
                               || (EdcG.Contains(x.DivisionCode) && (section == null || x.DivisionCode == section) && (subSection == null || x.ZoneCode == subSection))))))
                               from er in repo.GetMany<EventRound>(x => x.EventID == e.EventID)
                               from tie in repo.GetMany<Tie>(x => x.EventID == e.EventID)
                                   //join er in repo.GetAll<EventRound>() on e.EventID equals er.EventID into erl from er in erl.DefaultIfEmpty() //LEFT JOIN
                                   //join er in repo.GetAll<EventRound>() on e.EventID equals er.EventID
                                   //join tie in repo.GetAll<Tie>() on e.EventID equals tie.EventID
                               join nt1 in repo.GetAll<NationTranslated>(true) on tie.Side1NationCode equals nt1.NationCode into nt1L from nt1 in nt1L.DefaultIfEmpty() // LEFT JOIN
                               join nt2 in repo.GetAll<NationTranslated>(true) on tie.Side2NationCode equals nt2.NationCode into nt2L from nt2 in nt2L.DefaultIfEmpty() // LEFT JOIN
                               let divisionCodeSort = EdcOrders.IndexOf(e.DivisionCode) < 0 ? EdcOrders.Count : EdcOrders.IndexOf(e.DivisionCode)
                               let eventClassificationCodeSort = EccOrders.OrderOf(e.EventClassificationCode)
                               where (e.DrawsheetStructureCode == DcR || (tie.DrawsheetRoundNumber.HasValue && er.RoundNumber == tie.DrawsheetRoundNumber.Value))
                               orderby divisionCodeSort, e.ZoneCode, eventClassificationCodeSort, er.RoundNumber, e.SubGroupCode
                               group new {
                                   Events = new
                                   {
                                       e.EventID,
                                       e.Name,
                                       EventDesc = e.EventClassificationDesc,
                                       EventCode = e.EventClassificationCode,
                                       e.DivisionCode,
                                       ZoneCode = e.ZoneCode.GetZoneCode(e.DivisionCode),
                                       DrawsheetType = e.DrawsheetStructureCode
                                   },
                                   Rounds = new
                                   {
                                       er.RoundNumber,
                                       er.RoundCode,
                                       //RoundDesc = er.RoundCode.GetRoundDesc(),
                                       //RoundStartDate = tie.StartDate?.ToString(StartDateFormat),
                                       //RoundEndDate = tie.EndDate?.ToString(EndDateFormat)
                                   },
                                   Ties = new {
                                       tie.Name,
                                       tie.Side1NationCode,
                                       //tie.Side1NationName,
                                       Side1NationName = nt1?.GetNationByLanguage(language),
                                       Side1NationNameES = nt1?.GetNationByLanguage(Language.Es),
                                       tie.Side1Score,
                                       tie.Side2NationCode,
                                       //tie.Side2NationName,
                                       Side2NationName = nt2?.GetNationByLanguage(language),
                                       Side2NationNameES = nt2?.GetNationByLanguage(Language.Es),
                                       tie.Side2Score,
                                       tie.Side1Seeding,
                                       tie.Side2Seeding,
                                       Side1H2HWin = GetWinLosssCount(source, tie.Side1NationCode, tie.Side2NationCode)[0],
                                       Side2H2HWin = GetWinLosssCount(source, tie.Side1NationCode, tie.Side2NationCode)[1],
                                       IsSide1Hosting = tie.Side1ChoiceOfGroundFlag.ParseBool(),
                                       IsSide2Hosting = tie.Side2ChoiceOfGroundFlag.ParseBool(),
                                       IsCogByLot = tie.ChoiceOfGroundDecidedByLotFlag.ParseBool(),
                                       tie.Venue,
                                       StartDateOrg = tie.StartDate,
                                       EndDateOrg = tie.EndDate,
                                       StartDate = tie.StartDate?.ToString(StartDateFormat),
                                       EndDate = tie.EndDate?.ToString(EndDateFormat),
                                       tie.PublicTieId,
                                       PlayStatus = tie.PlayStatusCode,
                                       IsRubberInPlay = repo.GetMany<Match>(x => x.TieID == tie.TieID && x.PlayStatusCode == PsIp).Any(),
                                       IsBye = tie.Name.ToLower().Contains("bye")
                                   }
                               } by e.Year into g
                               select new
                               {
                                   Year = g.Key,
                                   Events = from gl in g.ToList()
                                            group gl by gl.Events into ge
                                            select new
                                            {
                                                ge.Key.Name,
                                                ge.Key.EventDesc,
                                                ge.Key.EventCode,
                                                ge.Key.DivisionCode,
                                                ge.Key.ZoneCode,
                                                ge.Key.DrawsheetType,
                                                Rounds = from gel in ge.ToList()
                                                         group gel by gel.Rounds into gr
                                                         select new
                                                         {
                                                             gr.Key.RoundNumber,
                                                             //gr.Key.RoundDesc,
                                                             RoundDesc = GetRoundDesc(source, ge.Key.DivisionCode, gr.Key.RoundNumber, gr.Key.RoundCode, ge.Key.EventCode, gr.Count(), gr.ToList().Count),
                                                             //gr.Key.RoundStartDate,
                                                             //gr.Key.RoundEndDate,
                                                             RoundStartDate = gr.ToList().FirstOrDefault()?.Ties.StartDateOrg?.ToString(StartDateFormat),
                                                             RoundEndDate = gr.ToList().FirstOrDefault()?.Ties.EndDateOrg?.ToString(EndDateFormat),
                                                             Ties = from grl in gr.ToList() select grl.Ties
                                                         }
                                            }
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"DrawSheet not found: year={year}, section={section}, subSection={subSection}");
                    return null;
                }

                var result = Mapper.Map<DrawSheetViewModel>(lResult);

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

        public ResultsByYearViewModel GetResultsByYear(int year,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheKey = $"{cachePrefix}.{language}.{source}.{year}";
                var cacheTimeout = GetCacheTimeout(cachePrefix, cacheKey);
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ResultsByYearViewModel;
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

                var events = repo.GetMany<Event>(x => x.Year == year).ToList();
                var eventIds = events.Select(x => x.EventID);
                var ties = repo.GetMany<Tie>(x => eventIds.Contains(x.EventID)).ToList();
                var tieIds = ties.Select(x => x.TieID).Distinct().ToList();

                var lResult = (from e in events
                               join er in repo.GetMany<EventRound>(x => eventIds.Contains(x.EventID)) on e.EventID equals er.EventID
                               join tie in ties on e.EventID equals tie.EventID
                               //join m in repo.GetMany<Match>(x => tieIds.Contains(x.TieID.Value)) on tie.TieID equals m.TieID into mL from m in mL.DefaultIfEmpty() // LEFT JOIN
                               //join ns in repo.GetMany<NationSquad>(x => tieIds.Contains(x.TieID)) on tie.TieID equals ns.TieID into nsL from ns in nsL.DefaultIfEmpty() // LEFT JOIN
                               let nt = repo.Get<NationTranslated>(x => x.NationCode == (tie.HostNationCode ?? string.Empty), true)
                               let nt1 = repo.Get<NationTranslated>(x => x.NationCode == (tie.Side1NationCode ?? string.Empty), true)
                               let nt2 = repo.Get<NationTranslated>(x => x.NationCode == (tie.Side2NationCode ?? string.Empty), true)
                               let divisionCodeSort = EdcOrders.IndexOf(e.DivisionCode) < 0 ? EdcOrders.Count : EdcOrders.IndexOf(e.DivisionCode)
                               let eventClassificationCodeSort = EccOrders.OrderOf(e.EventClassificationCode)
                               where (e.DrawsheetStructureCode == DcR || (tie.DrawsheetRoundNumber.HasValue && er.RoundNumber == tie.DrawsheetRoundNumber.Value))
                               orderby divisionCodeSort, e.ZoneCode, eventClassificationCodeSort, er.RoundNumber, e.SubGroupCode, tie.DrawsheetRoundNumber, tie.DrawsheetPositionTie//, m?.DrawsheetPositionMatch
                               group new
                               {
                                   Events = new
                                   {
                                       e.EventID,
                                       e.Name,
                                       EventDesc = e.EventClassificationDesc,
                                       EventCode = e.EventClassificationCode,
                                       e.DivisionCode,
                                       ZoneCode = e.ZoneCode.GetZoneCode(e.DivisionCode),
                                       DrawsheetType = e.DrawsheetStructureCode
                                   },
                                   Rounds = new
                                   {
                                       er.RoundNumber,
                                       er.RoundCode,
                                       //RoundDesc = er.RoundCode.GetRoundDesc(),
                                       //RoundStartDate = tie.StartDate?.ToString(StartDateFormat),
                                       //RoundEndDate = tie.EndDate?.ToString(EndDateFormat)
                                   },
                                   Ties = new
                                   {
                                       tie.Name,
                                       tie.Side1NationCode,
                                       //tie.Side1NationName,
                                       Side1NationName = nt1?.GetNationByLanguage(language),
                                       tie.Side1Score,
                                       tie.Side2NationCode,
                                       //tie.Side2NationName,
                                       Side2NationName = nt2?.GetNationByLanguage(language),
                                       tie.HostNationCode,
                                       HostNationName = nt?.GetNationByLanguage(language),
                                       tie.Side2Score,
                                       //tie.Side1Seeding,
                                       //tie.Side2Seeding,
                                       //IsSide1Hosting = tie.Side1ChoiceOfGroundFlag.ParseBool(),
                                       //IsSide2Hosting = tie.Side2ChoiceOfGroundFlag.ParseBool(),
                                       //IsCogByLot = tie.ChoiceOfGroundDecidedByLotFlag.ParseBool(),
                                       Surface = tie.SurfaceDesc,
                                       IndoorOutdoor = tie.IndoorOutdoorFlag.GetIndoorOutdoor(),
                                       tie.WinningSide,
                                       Score = tie.Score = tie.WinningSide == 1 ? $"{tie.Side1Score}-{tie.Side2Score}" : tie.WinningSide == 2 ? $"{tie.Side2Score}-{tie.Side1Score}" : string.Empty,
                                       tie.Venue,
                                       TieId = tie.TieID,
                                       Result = tie.ResultLong,
                                       StartDateOrg = tie.StartDate,
                                       EndDateOrg = tie.EndDate,
                                       StartDate = tie.StartDate?.ToString(StartDateFormat),
                                       EndDate = tie.EndDate?.ToString(EndDateFormat),
                                       tie.PublicTieId,
                                       //PlayStatus = tie.PlayStatusCode,
                                       //IsRubberInPlay = repo.GetMany<Match>(x => x.TieID == tie.TieID && x.PlayStatusCode == PsIp).Any(),
                                       //IsBye = tie.Name.ToLower().Contains("bye")
                                   },
                                   //Matches = m == null ? null : new
                                   //{
                                   //    m.Side1Player1GivenName,
                                   //    m.Side1Player1FamilyName,
                                   //    m.Side1Player2GivenName,
                                   //    m.Side1Player2FamilyName,
                                   //    m.Side2Player1GivenName,
                                   //    m.Side2Player1FamilyName,
                                   //    m.Side2Player2GivenName,
                                   //    m.Side2Player2FamilyName,
                                   //    m.WinningSide,
                                   //    m.Score
                                   //},
                                   //TeamsNationCode = ns?.NationCode,
                                   //TeamsCaptain = ns?.CaptainGivenName + " " + ns?.CaptainFamilyName,
                                   //TeamPlayers = ns == null ? null : new[]
                                   //{
                                   //    new { PlayerGivenName = ns.Player1GivenName, PlayerFamilyName = ns.Player1FamilyName, PlayerId = ns.Player1ID},
                                   //    new { PlayerGivenName = ns.Player2GivenName, PlayerFamilyName = ns.Player2FamilyName, PlayerId = ns.Player2ID},
                                   //    new { PlayerGivenName = ns.Player3GivenName, PlayerFamilyName = ns.Player3FamilyName, PlayerId = ns.Player3ID},
                                   //    new { PlayerGivenName = ns.Player4GivenName, PlayerFamilyName = ns.Player4FamilyName, PlayerId = ns.Player4ID},
                                   //    new { PlayerGivenName = ns.Player5GivenName, PlayerFamilyName = ns.Player5FamilyName, PlayerId = ns.Player5ID}
                                   //}
                               } by e.Year into g
                               select new
                               {
                                   Year = g.Key,
                                   Events = from gl in g.ToList()
                                            group gl by gl.Events into ge
                                            select new
                                            {
                                                ge.Key.Name,
                                                ge.Key.EventDesc,
                                                ge.Key.EventCode,
                                                ge.Key.DivisionCode,
                                                ge.Key.ZoneCode,
                                                ge.Key.DrawsheetType,
                                                Rounds = from gel in ge.ToList()
                                                         group gel by gel.Rounds into gr
                                                         select new
                                                         {
                                                             gr.Key.RoundNumber,
                                                             //gr.Key.RoundDesc,
                                                             RoundDesc = GetRoundDesc(source, ge.Key.DivisionCode, gr.Key.RoundNumber, gr.Key.RoundCode, ge.Key.EventCode, gr.Count(), gr.ToList().GroupBy(x => x.Ties).ToList().Count),
                                                             //gr.Key.RoundStartDate,
                                                             //gr.Key.RoundEndDate,
                                                             RoundStartDate = gr.ToList().FirstOrDefault()?.Ties.StartDateOrg?.ToString(StartDateFormat),
                                                             RoundEndDate = gr.ToList().FirstOrDefault()?.Ties.EndDateOrg?.ToString(EndDateFormat),
                                                             Ties = from grl in gr.ToList()
                                                                    group grl by grl.Ties into gt
                                                                    select new
                                                                    {
                                                                        gt.Key.Name,
                                                                        gt.Key.Side1NationCode,
                                                                        gt.Key.Side1NationName,
                                                                        gt.Key.Side2NationCode,
                                                                        gt.Key.Side2NationName,
                                                                        gt.Key.Side1Score,
                                                                        gt.Key.Side2Score,
                                                                        gt.Key.Venue,
                                                                        gt.Key.PublicTieId,
                                                                        gt.Key.HostNationCode,
                                                                        gt.Key.HostNationName,
                                                                        gt.Key.Surface,
                                                                        gt.Key.IndoorOutdoor,
                                                                        gt.Key.WinningSide,
                                                                        gt.Key.Score,
                                                                        gt.Key.Result,
                                                                        gt.Key.TieId,
                                                                        //Matches = from gtl in gt.ToList() where gtl.Matches != null group gtl by gtl.Matches into gm select gm.Key,
                                                                        //Teams = from gtl in gt.ToList() where gtl.TeamsNationCode != null
                                                                        //        group gtl by new { gtl.TeamsNationCode, gtl.TeamsCaptain } into gte
                                                                        //        select new
                                                                        //        {
                                                                        //            NationCode = gte.Key.TeamsNationCode,
                                                                        //            Captain = gte.Key.TeamsCaptain,
                                                                        //            Players = from gpl in gte.ToList().Select(x => x.TeamPlayers).FirstOrDefault()?.Where(x => x.PlayerId.HasValue) select new { gpl.PlayerGivenName, gpl.PlayerFamilyName, gpl.PlayerId }
                                                                        //        }
                                                                    }
                                                         }
                                            }
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"DrawSheet not found: year={year}");
                    return null;
                }

                var result = Mapper.Map<ResultsByYearViewModel>(lResult);

                var matches = repo.GetMany<Match>(x => tieIds.Contains(x.TieID.Value)).ToList();
                var nationSquads = repo.GetMany<NationSquad>(x => tieIds.Contains(x.TieID)).ToList();
                result.Events.SelectMany(x => x.Rounds).SelectMany(x => x.Ties).ToList().ForEach(x =>
                {
                    var mResult = (from m in matches where m.TieID == x.TieId
                                   orderby m.DrawsheetPositionMatch
                                   select new
                                   {
                                       m.Side1Player1GivenName,
                                       m.Side1Player1FamilyName,
                                       m.Side1Player2GivenName,
                                       m.Side1Player2FamilyName,
                                       m.Side2Player1GivenName,
                                       m.Side2Player1FamilyName,
                                       m.Side2Player2GivenName,
                                       m.Side2Player2FamilyName,
                                       m.WinningSide,
                                       m.Score
                                   }).ToList();
                    x.Matches = Mapper.Map<ICollection<ResultMatchViewModel>>(mResult);
                    var pResult = (from p in nationSquads where p.TieID == x.TieId
                                   select new
                                   {
                                       p.NationCode,
                                       Captain = p.CaptainGivenName + " " + p.CaptainFamilyName,
                                       Players = new List<ResultsPlayerViewModel>
                                       {
                                           new ResultsPlayerViewModel { PlayerGivenName = p.Player1GivenName, PlayerFamilyName = p.Player1FamilyName, PlayerId = p.Player1ID},
                                           new ResultsPlayerViewModel { PlayerGivenName = p.Player2GivenName, PlayerFamilyName = p.Player2FamilyName, PlayerId = p.Player2ID},
                                           new ResultsPlayerViewModel { PlayerGivenName = p.Player3GivenName, PlayerFamilyName = p.Player3FamilyName, PlayerId = p.Player3ID},
                                           new ResultsPlayerViewModel { PlayerGivenName = p.Player4GivenName, PlayerFamilyName = p.Player4FamilyName, PlayerId = p.Player4ID},
                                           new ResultsPlayerViewModel { PlayerGivenName = p.Player5GivenName, PlayerFamilyName = p.Player5FamilyName, PlayerId = p.Player5ID}
                                       }
                                   }).ToList();
                    pResult.ForEach(y =>
                    {
                        y.Players.RemoveAll(z => !z.PlayerId.HasValue);
                    });
                    x.Teams = Mapper.Map<ICollection<ResultTeamViewModel>>(pResult);
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

        public ICollection<RoundRobinEventViewModel> GetRoundRobinEvents(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{year}.{section}.{subSection}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<RoundRobinEventViewModel>;
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
                var lResult = (from e in repo.GetMany<Event>(x => x.DivisionCode == section && x.ZoneCode == subSection && x.Year == year)
                               join edo in repo.GetAll<EventPlayOffsDisplayOrder>(true) on e.EventClassificationCode equals edo.EventClassificationCode into edoL from edo in edoL.DefaultIfEmpty() // LEFT JOIN
                               //join en in repo.GetAll<EventNation>() on e.EventID equals en.EventID
                               from en in repo.GetMany<EventNation>(x => x.EventID == e.EventID)
                               join nt in repo.GetAll<NationTranslated>(true) on en.NationCode equals nt.NationCode into ntL from nt in ntL.DefaultIfEmpty() // LEFT JOIN
                               //join tie in repo.GetAll<Tie>() on e.EventID equals tie.EventID
                               from tie in repo.GetMany<Tie>(x => x.EventID == e.EventID)
                               join nt1 in repo.GetAll<NationTranslated>(true) on tie.Side1NationCode equals nt1.NationCode into nt1L from nt1 in nt1L.DefaultIfEmpty() // LEFT JOIN
                               join nt2 in repo.GetAll<NationTranslated>(true) on tie.Side2NationCode equals nt2.NationCode into nt2L from nt2 in nt2L.DefaultIfEmpty() // LEFT JOIN
                               where (e.DrawsheetStructureCode == DcR && (tie.Side1NationCode == en.NationCode || tie.Side2NationCode == en.NationCode)) ||
                                     (e.DrawsheetStructureCode == DcK && (tie.Side1NationCode == en.NationCode || (tie.Side1NationCode == null && tie.Side2NationCode == en.NationCode)))
                               orderby EccOrdersRoundRobin.OrderOf(e.EventClassificationCode), e.DrawsheetStructureCode, e.SubGroupCode, edo?.DisplayOrder, e.EventID,
                                    tie.DrawsheetRoundNumber, tie.DrawsheetPositionTie
                               group new
                               {
                                   ResultsRR = e.DrawsheetStructureCode == DcR ?
                                       new
                                       {
                                           EventId = e.EventID,
                                           en.NationCode,
                                           NationName = nt?.GetNationByLanguage(language),
                                           DrawPosition = en.NationCode == tie.Side1NationCode ? tie.DrawsheetPositionSide1 : tie.DrawsheetPositionSide2,
                                           Seeding = en.NationCode == tie.Side1NationCode ? tie.Side1Seeding : tie.Side2Seeding
                                       } : null,
                                   ResultsPO = e.DrawsheetStructureCode == DcK,
                                   Ties =
                                       new
                                       {
                                           tie.PublicTieId,
                                           tie.Side1NationCode,
                                           Side1NationName = nt1?.GetNationByLanguage(language),
                                           tie.Side1Score,
                                           tie.Side2NationCode,
                                           Side2NationName = nt2?.GetNationByLanguage(language),
                                           tie.Side2Score,
                                           PlayStatus = tie.PlayStatusCode,
                                           IsInProgress = repo.GetMany<Match>(x => x.TieID == tie.TieID && x.PlayStatusCode == PsIp).Any(),
                                           Date = tie.StartDate == tie.EndDate ? tie.StartDate?.ToString(StartDateFormat) : tie.StartDate?.ToString(StartDateFormat) + " - " + tie.EndDate?.ToString(StartDateFormat)
                                       },
                                   en.NationCode,
                                   tie.WinningSide
                               } by new
                               {
                                   EventId = e.EventID,
                                   e.Venue,
                                   Surface = e.SurfaceDesc,
                                   e.SubGroupCode,
                                   e.DrawsheetStructureCode,
                                   e.EventClassificationCode,
                                   e.EventClassificationDesc
                               } into eg
                               select new
                               {
                                   eg.Key.EventId,
                                   eg.Key.Venue,
                                   eg.Key.Surface,
                                   eg.Key.SubGroupCode,
                                   eg.Key.DrawsheetStructureCode,
                                   eg.Key.EventClassificationDesc,
                                   ResultsRR = from rr in eg.Where(x => x.ResultsRR != null).ToList()
                                               group rr by rr.ResultsRR into rrG
                                               let tieWon = rrG.Count(x => x.Ties.PlayStatus == PsPc && ((x.WinningSide == 1 && x.Ties.Side1NationCode == x.NationCode)
                                                   || (x.WinningSide == 2 && x.Ties.Side2NationCode == x.NationCode)))
                                               let tieLost = rrG.Count(x => x.Ties.PlayStatus == PsPc && ((x.WinningSide == 1 && x.Ties.Side1NationCode != x.NationCode)
                                                   || (x.WinningSide == 2 && x.Ties.Side2NationCode != x.NationCode)))
                                               let tiePlayed = rrG.Count(x => x.Ties.PlayStatus == PsPc)
                                               let rubbersWon = rrG.Where(x => x.Ties.PlayStatus == PsPc).Sum(
                                                   x => x.Ties.Side1NationCode == x.NationCode ? x.Ties.Side1Score : x.Ties.Side2Score)
                                               let rubbersLost = rrG.Where(x => x.Ties.PlayStatus == PsPc).Sum(
                                                   x => x.Ties.Side1NationCode == x.NationCode ? x.Ties.Side2Score : x.Ties.Side1Score)
                                               orderby tieWon descending, rubbersWon descending ,tiePlayed descending, rrG.Key.Seeding, rrG.Key.DrawPosition
                                               select new
                                               {
                                                   rrG.Key.EventId,
                                                   rrG.Key.NationCode,
                                                   rrG.Key.NationName,
                                                   rrG.Key.DrawPosition,
                                                   rrG.Key.Seeding,
                                                   TiePlayed = tiePlayed,
                                                   TieWon = tieWon,
                                                   TieLost = tieLost,
                                                   Rubbers = $"{rubbersWon} - {rubbersLost}",
                                                   Ties = from rrL in rrG.ToList() select rrL.Ties
                                               },
                                   ResultsPO = from po in eg.Where(x => x.ResultsPO).ToList() select po.Ties
                               }).ToList();

                var result = Mapper.Map<ICollection<RoundRobinEventViewModel>>(lResult);
                result.ForEach(delegate(int idx, RoundRobinEventViewModel x) { x.EventOrder = idx; });

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

        public ICollection<TournamentViewModel> GetTournaments(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{year}.{section}.{subSection}";
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

                var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;
                var lResult = (from td in repo.GetMany<TournamentDetail>(x => x.DivisionCode == section && x.ZoneCode == subSection && x.Year == year)
                               select new
                               {
                                   TournamentId = td.PublicTournamentId,
                                   td.Name,
                                   td.StartDate,
                                   td.EndDate,
                                   td.Year,
                                   td.DivisionCode,
                                   td.ZoneCode,
                                   td.SubZoneCode,
                                   td.Location,
                                   Venue = td.SiteName,
                                   td.HostNationCode,
                                   HostNationName = td.HostNationCode == null ? null : repo.Get<NationTranslated>(x => x.NationCode == td.HostNationCode, true)?.GetNationByLanguage(language),
                                   //td.HostNationName,
                                   td.SurfaceCode,
                                   td.SurfaceDesc
                               }).ToList();

                var result = Mapper.Map<ICollection<TournamentViewModel>>(lResult);

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

        public ICollection<RoundRobinNominationViewModel> GetRoundRobinNominations(int year, string section, string subSection,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{year}.{section}.{subSection}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<RoundRobinNominationViewModel>;
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
                var tms = repo.GetMany<TeamNomination>(x => x.DivisionCode == section && x.ZoneCode == subSection && x.CalendarYear == year).ToList();
                var teamNominationIds = tms.Select(x => x.TeamNominationId);

                var lResult = (from tm in tms
                               join tmp in repo.GetMany<TeamNominationPlayer>(x => teamNominationIds.Contains(x.TeamNominationId)) on tm.TeamNominationId equals tmp.TeamNominationId
                               into tmpL from tmp in tmpL.DefaultIfEmpty() // LEFT JOIN
                               let nt = repo.Get<NationTranslated>(x => x.NationCode == tm.NationCode, true)
                               orderby tm.Nation, tmp?.TeamNominationPosition
                               group new
                               {
                                   Players = tmp == null ? null : new
                                   {
                                       PlayerId = tmp.DataExchangePlayerId,
                                       tmp.FamilyName,
                                       tmp.GivenName,
                                       NationCode = tmp.NationalityCode,
                                       tmp.BirthDate
                                   }
                               } by new
                               {
                                   tm.TeamNominationId,
                                   Nation = nt?.GetNationByLanguage(language),
                                   tm.NationCode,
                                   tm.Seeding,
                                   CaptainPlayerId = tm.CaptainDataExchangePlayerId,
                                   tm.CaptainFamilyName,
                                   tm.CaptainGivenName,
                                   CaptainNationCode = tm.CaptainNationalityCode
                               } into g
                               select new
                               {
                                   g.Key.TeamNominationId,
                                   g.Key.Nation,
                                   g.Key.NationCode,
                                   g.Key.Seeding,
                                   g.Key.CaptainPlayerId,
                                   g.Key.CaptainFamilyName,
                                   g.Key.CaptainGivenName,
                                   g.Key.CaptainNationCode,
                                   Players = from gl in g.ToList() where gl.Players != null select gl.Players
                               }).ToList();

                var result = Mapper.Map<ICollection<RoundRobinNominationViewModel>>(lResult);

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

        public ICollection<TieDetailsWebViewModel> GetNodeRelatedTieDetails(int nodeId, Language language = Language.En,
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
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<TieDetailsWebViewModel>;
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

                var result = (from rt in CmsRepo.GetMany<ITFRelationships>(x => x.assetId == nodeId && x.relatedAssetTypeId == 33 && x.notes != null)
                              orderby rt.sortOrder
                              select GetTieDetailsWeb(rt.notes, language, source, useCache)).ToList();

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

        public TieDetailsWebViewModel GetTieDetailsWeb(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{publicTieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as TieDetailsWebViewModel;
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

                var lResult = (from tie in repo.GetMany<Tie>(x => x.PublicTieId == publicTieId)
                               let wl = GetWinLosssCount(source, tie.Side1NationCode, tie.Side2NationCode)
                               let side1NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1NationCode : tie.Side2NationCode
                               let side2NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2NationCode : tie.Side1NationCode
                               let nt1 = repo.Get<NationTranslated>(x => x.NationCode == side1NationCode, true)
                               let nt2 = repo.Get<NationTranslated>(x => x.NationCode == side2NationCode, true)
                               select new
                               {
                                   TieId = tie.PublicTieId,
                                   tie.EventName,
                                   RoundDesc = tie.DrawsheetRoundDesc,
                                   tie.Venue,
                                   StartDate = tie.StartDate?.ToString(StartDateFormat),
                                   EndDate = tie.EndDate?.ToString(EndDateFormat),
                                   tie.HostNationCode,
                                   tie.HostNationName,
                                   tie.SurfaceCode,
                                   Surface = tie.SurfaceDesc,
                                   tie.SurfaceTypeDesc,
                                   tie.BallDesc,
                                   IndoorOutdoor = tie.IndoorOutdoorFlag.GetIndoorOutdoor(),
                                   tie.TimeOfPlayDay1,
                                   tie.TimeOfPlayDay2,
                                   tie.TimeOfPlayDay3,
                                   Side1NationCode = side1NationCode,
                                   Side1NationName = nt1.GetNationByLanguage(language)?.Replace("Republic", "Rep."),
                                   Side2NationCode = side2NationCode,
                                   Side2NationName = nt2.GetNationByLanguage(language)?.Replace("Republic", "Rep."),
                                   Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? $"{tie.Side1Score}:{tie.Side2Score}" : $"{tie.Side2Score}:{tie.Side1Score}",
                                   Side1Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1Score : tie.Side2Score,
                                   Side2Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2Score : tie.Side1Score,
                                   Side1NationH2HWinCount = (tie.HomeSide == 1 || tie.HomeSide == null) ? wl[0] : wl[1],
                                   Side2NationH2HWinCount = (tie.HomeSide == 1 || tie.HomeSide == null) ? wl[1] : wl[0],
                                   IsSquadNominated = repo.GetMany<NationSquad>(x => x.TieID == tie.TieID).Any(),
                                   IsDrawsMade = repo.GetMany<Match>(x => x.TieID == tie.TieID).Any(),
                                   tie.PlayStatusCode,
                                   tie.ResultStatusCode
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"Tie not found: publicTieId={publicTieId}");
                    return null;
                }

                var result = Mapper.Map<TieDetailsWebViewModel>(lResult);

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

        public TieDetailsViewModel GetTieDetails(int tieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{tieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as TieDetailsViewModel;
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

                var lResult = (from tie in repo.GetMany<Tie>(x => x.TieID == tieId)
                               let wl = GetWinLosssCount(source, tie.Side1NationCode, tie.Side2NationCode)
                               select new
                               {
                                   TieId = tie.TieID,
                                   tie.PublicTieId,
                                   tie.EventName,
                                   RoundDesc = tie.DrawsheetRoundDesc,
                                   tie.Venue,
                                   StartDate = tie.StartDate?.ToString(StartDateFormat),
                                   EndDate = tie.EndDate?.ToString(EndDateFormat),
                                   tie.HostNationCode,
                                   tie.HostNationName,
                                   tie.SurfaceCode,
                                   Surface = tie.SurfaceDesc,
                                   IndoorOutdoor = tie.IndoorOutdoorFlag.GetIndoorOutdoor(),
                                   tie.TimeOfPlayDay1,
                                   tie.TimeOfPlayDay2,
                                   tie.TimeOfPlayDay3,
                                   Side1NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1NationCode : tie.Side2NationCode,
                                   Side1NationName = ((tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1NationName : tie.Side2NationName).Replace("Republic", "Rep."),
                                   Side2NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2NationCode : tie.Side1NationCode,
                                   Side2NationName = ((tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2NationName : tie.Side1NationName).Replace("Republic", "Rep."),
                                   Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? $"{tie.Side1Score}:{tie.Side2Score}" : $"{tie.Side2Score}:{tie.Side1Score}",
                                   Side1Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1Score : tie.Side2Score,
                                   Side2Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2Score : tie.Side1Score,
                                   Side1NationH2HWinCount = (tie.HomeSide == 1 || tie.HomeSide == null) ? wl[0] : wl[1],
                                   Side2NationH2HWinCount = (tie.HomeSide == 1 || tie.HomeSide == null) ? wl[1] : wl[0],
                                   IsSquadNominated = repo.GetMany<NationSquad>(x => x.TieID == tie.TieID).Any(),
                                   IsDrawsMade = repo.GetMany<Match>(x => x.TieID == tie.TieID).Any(),
                                   tie.PlayStatusCode,
                                   tie.ResultStatusCode
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"Tie not found: tieId={tieId}");
                    return null;
                }

                var result = Mapper.Map<TieDetailsViewModel>(lResult);

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

        public ICollection<TieNominationViewModel> GetTieNominations(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{publicTieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<TieNominationViewModel>;
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

                var lResult = (from tie in repo.GetMany<Tie>(x => x.PublicTieId == publicTieId)
                               //join ns in repo.GetAll<NationSquad>() on tie.TieID equals ns.TieID
                               from ns in repo.GetMany<NationSquad>(x => x.TieID == tie.TieID)
                               //join pb1 in repo.GetAllSpecific<PlayerBiographyCup>() on ns.Player1ID equals pb1.PlayerID into nb1L from pb1 in nb1L.DefaultIfEmpty() // LEFT JOIN
                               //join pb2 in repo.GetAllSpecific<PlayerBiographyCup>() on ns.Player2ID equals pb2.PlayerID into nb2L from pb2 in nb2L.DefaultIfEmpty() // LEFT JOIN
                               //join pb3 in repo.GetAllSpecific<PlayerBiographyCup>() on ns.Player3ID equals pb3.PlayerID into nb3L from pb3 in nb3L.DefaultIfEmpty() // LEFT JOIN
                               //join pb4 in repo.GetAllSpecific<PlayerBiographyCup>() on ns.Player4ID equals pb4.PlayerID into nb4L from pb4 in nb4L.DefaultIfEmpty() // LEFT JOIN
                               //join pb5 in repo.GetAllSpecific<PlayerBiographyCup>() on ns.Player5ID equals pb5.PlayerID into nb5L from pb5 in nb5L.DefaultIfEmpty() // LEFT JOIN
                               from pb1 in repo.GetManySpecific<PlayerBiographyCup>(x => x.PlayerID == ns.Player1ID).DefaultIfEmpty()
                               from pb2 in repo.GetManySpecific<PlayerBiographyCup>(x => x.PlayerID == ns.Player2ID).DefaultIfEmpty()
                               from pb3 in repo.GetManySpecific<PlayerBiographyCup>(x => x.PlayerID == ns.Player3ID).DefaultIfEmpty()
                               from pb4 in repo.GetManySpecific<PlayerBiographyCup>(x => x.PlayerID == ns.Player4ID).DefaultIfEmpty()
                               from pb5 in repo.GetManySpecific<PlayerBiographyCup>(x => x.PlayerID == ns.Player5ID).DefaultIfEmpty()
                               let wl = GetWinLosssCount(source, tie.Side1NationCode, tie.Side2NationCode)
                               select new
                               {
                                   ns.NationCode,
                                   ns.NationName,
                                   CapGN = ns.CaptainGivenName,
                                   CapFN = ns.CaptainFamilyName,
                                   P1Id = ns.Player1DataExchangePlayerId,
                                   P1GN = ns.Player1GivenName,
                                   P1GNI = string.IsNullOrEmpty(ns.Player1GivenName) ? null : ns.Player1GivenName.Substring(0, 1),
                                   P1FN = ns.Player1FamilyName,
                                   P1HSId = ns.Player1ID.HasValue ? _playerService.GetHeadshotImgId(ns.Player1ID.Value) : null,
                                   P1DOB = pb1?.BirthDate?.ToString(EndDateFormat),
                                   P1RkS = pb1?.RankCurrentSinglesRollover,
                                   P1RkD = pb1?.RankCurrentDoublesRollover,
                                   P2Id = ns.Player2DataExchangePlayerId,
                                   P2GN = ns.Player2GivenName,
                                   P2GNI = string.IsNullOrEmpty(ns.Player2GivenName) ? null : ns.Player2GivenName.Substring(0, 1),
                                   P2FN = ns.Player2FamilyName,
                                   P2HSId = ns.Player2ID.HasValue ? _playerService.GetHeadshotImgId(ns.Player2ID.Value) : null,
                                   P2DOB = pb2?.BirthDate?.ToString(EndDateFormat),
                                   P2RkS = pb2?.RankCurrentSinglesRollover,
                                   P2RkD = pb2?.RankCurrentDoublesRollover,
                                   P3Id = ns.Player3DataExchangePlayerId,
                                   P3GN = ns.Player3GivenName,
                                   P3GNI = string.IsNullOrEmpty(ns.Player3GivenName) ? null : ns.Player3GivenName.Substring(0, 1),
                                   P3FN = ns.Player3FamilyName,
                                   P3HSId = ns.Player3ID.HasValue ? _playerService.GetHeadshotImgId(ns.Player3ID.Value) : null,
                                   P3DOB = pb3?.BirthDate?.ToString(EndDateFormat),
                                   P3RkS = pb3?.RankCurrentSinglesRollover,
                                   P3RkD = pb3?.RankCurrentDoublesRollover,
                                   P4Id = ns.Player4DataExchangePlayerId,
                                   P4GN = ns.Player4GivenName,
                                   P4GNI = string.IsNullOrEmpty(ns.Player4GivenName) ? null : ns.Player4GivenName.Substring(0, 1),
                                   P4FN = ns.Player4FamilyName,
                                   P4HSId = ns.Player4ID.HasValue ? _playerService.GetHeadshotImgId(ns.Player4ID.Value) : null,
                                   P4DOB = pb4?.BirthDate?.ToString(EndDateFormat),
                                   P4RkS = pb4?.RankCurrentSinglesRollover,
                                   P4RkD = pb4?.RankCurrentDoublesRollover,
                                   P5Id = ns.Player5DataExchangePlayerId,
                                   P5GN = ns.Player5GivenName,
                                   P5GNI = string.IsNullOrEmpty(ns.Player5GivenName) ? null : ns.Player5GivenName.Substring(0, 1),
                                   P5FN = ns.Player5FamilyName,
                                   P5HSId = ns.Player5ID.HasValue ? _playerService.GetHeadshotImgId(ns.Player5ID.Value) : null,
                                   P5DOB = pb5?.BirthDate?.ToString(EndDateFormat),
                                   P5RkS = pb5?.RankCurrentSinglesRollover,
                                   P5RkD = pb5?.RankCurrentDoublesRollover
                               }).ToList();

                var result = Mapper.Map<ICollection<TieNominationViewModel>>(lResult);

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

        public ICollection<TieResultsWebViewModel> GetTieResultsWeb(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{publicTieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<TieResultsWebViewModel>;
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

                var lResult = (from tie in repo.GetMany<Tie>(x => x.PublicTieId == publicTieId)
                                   //join m in repo.GetAll<Match>() on tie.TieID equals m.TieID
                               from m in repo.GetMany<Match>(x => x.TieID == tie.TieID)
                               orderby m.DrawsheetPositionMatch
                               select new
                               {
                                   TieId = tie.TieID,
                                   S1NationCode = m.Side1TieNationCode,
                                   S1P1Id = m.Side1Player1DataExchangePlayerId,
                                   S1P1GN = m.Side1Player1GivenName,
                                   S1P1GNI = string.IsNullOrEmpty(m.Side1Player1GivenName) ? null : m.Side1Player1GivenName.Substring(0, 1),
                                   S1P1FN = m.Side1Player1FamilyName,
                                   S1P2Id = m.Side1Player2DataExchangePlayerId,
                                   S1P2GN = m.Side1Player2GivenName,
                                   S1P2GNI = string.IsNullOrEmpty(m.Side1Player2GivenName) ? null : m.Side1Player2GivenName.Substring(0, 1),
                                   S1P2FN = m.Side1Player2FamilyName,
                                   S2NationCode = m.Side2TieNationCode,
                                   S2P1Id = m.Side2Player1DataExchangePlayerId,
                                   S2P1GN = m.Side2Player1GivenName,
                                   S2P1GNI = string.IsNullOrEmpty(m.Side2Player1GivenName) ? null : m.Side2Player1GivenName.Substring(0, 1),
                                   S2P1FN = m.Side2Player1FamilyName,
                                   S2P2Id = m.Side2Player2DataExchangePlayerId,
                                   S2P2GN = m.Side2Player2GivenName,
                                   S2P2GNI = string.IsNullOrEmpty(m.Side2Player2GivenName) ? null : m.Side2Player2GivenName.Substring(0, 1),
                                   S2P2FN = m.Side2Player2FamilyName,
                                   m.WinningSide,
                                   S1Set1Sco = m.ScoreSet1Side1,
                                   S2Set1Sco = m.ScoreSet1Side2,
                                   S1Set1TBSco = PlayerService.GetTieBreakScore(m.ScoreSet1LosingTieBreak, m.ScoreSet1Side1, m.ScoreSet1Side2),
                                   S2Set1TBSco = PlayerService.GetTieBreakScore(m.ScoreSet1LosingTieBreak, m.ScoreSet1Side2, m.ScoreSet1Side1),
                                   S1Set2Sco = m.ScoreSet2Side1,
                                   S2Set2Sco = m.ScoreSet2Side2,
                                   S1Set2TBSco = PlayerService.GetTieBreakScore(m.ScoreSet2LosingTieBreak, m.ScoreSet2Side1, m.ScoreSet2Side2),
                                   S2Set2TBSco = PlayerService.GetTieBreakScore(m.ScoreSet2LosingTieBreak, m.ScoreSet2Side2, m.ScoreSet2Side1),
                                   S1Set3Sco = m.ScoreSet3Side1,
                                   S2Set3Sco = m.ScoreSet3Side2,
                                   S1Set3TBSco = PlayerService.GetTieBreakScore(m.ScoreSet3LosingTieBreak, m.ScoreSet3Side1, m.ScoreSet3Side2),
                                   S2Set3TBSco = PlayerService.GetTieBreakScore(m.ScoreSet3LosingTieBreak, m.ScoreSet3Side2, m.ScoreSet3Side1),
                                   S1Set4Sco = m.ScoreSet4Side1,
                                   S2Set4Sco = m.ScoreSet4Side2,
                                   S1Set4TBSco = PlayerService.GetTieBreakScore(m.ScoreSet4LosingTieBreak, m.ScoreSet4Side1, m.ScoreSet4Side2),
                                   S2Set4TBSco = PlayerService.GetTieBreakScore(m.ScoreSet4LosingTieBreak, m.ScoreSet4Side2, m.ScoreSet4Side1),
                                   S1Set5Sco = m.ScoreSet5Side1,
                                   S2Set5Sco = m.ScoreSet5Side2,
                                   S1Set5TBSco = PlayerService.GetTieBreakScore(m.ScoreSet5LosingTieBreak, m.ScoreSet5Side1, m.ScoreSet5Side2),
                                   S2Set5TBSco = PlayerService.GetTieBreakScore(m.ScoreSet5LosingTieBreak, m.ScoreSet5Side2, m.ScoreSet5Side1),
                                   Score = m.Score?.Trim(),
                                   ScoreReversed = m.ScoreReversed?.Trim(),
                                   m.PlayStatusCode,
                                   m.PlayStatusDesc,
                                   m.ResultStatusCode
                               }).ToList();

                var result = Mapper.Map<ICollection<TieResultsWebViewModel>>(lResult);
                result.ForEach(delegate (int idx, TieResultsWebViewModel x) { x.RubberNumber = idx+1; });

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

        public ICollection<TieResultsViewModel> GetTieResults(int tieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{tieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<TieResultsViewModel>;
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

                var lResult = (from tie in repo.GetMany<Tie>(x => x.TieID == tieId)
                               from m in repo.GetMany<Match>(x => x.TieID == tie.TieID)
                               orderby m.DrawsheetPositionMatch
                               select new
                               {
                                   TieId = tie.TieID,
                                   S1NationCode = m.Side1TieNationCode,
                                   S1P1Id = m.Side1Player1ID,
                                   S1P1GN = m.Side1Player1GivenName,
                                   S1P1GNI = string.IsNullOrEmpty(m.Side1Player1GivenName) ? null : m.Side1Player1GivenName.Substring(0, 1),
                                   S1P1FN = m.Side1Player1FamilyName,
                                   S1P2Id = m.Side1Player2ID,
                                   S1P2GN = m.Side1Player2GivenName,
                                   S1P2GNI = string.IsNullOrEmpty(m.Side1Player2GivenName) ? null : m.Side1Player2GivenName.Substring(0, 1),
                                   S1P2FN = m.Side1Player2FamilyName,
                                   S2NationCode = m.Side2TieNationCode,
                                   S2P1Id = m.Side2Player1ID,
                                   S2P1GN = m.Side2Player1GivenName,
                                   S2P1GNI = string.IsNullOrEmpty(m.Side2Player1GivenName) ? null : m.Side2Player1GivenName.Substring(0, 1),
                                   S2P1FN = m.Side2Player1FamilyName,
                                   S2P2Id = m.Side2Player2ID,
                                   S2P2GN = m.Side2Player2GivenName,
                                   S2P2GNI = string.IsNullOrEmpty(m.Side2Player2GivenName) ? null : m.Side2Player2GivenName.Substring(0, 1),
                                   S2P2FN = m.Side2Player2FamilyName,
                                   m.WinningSide,
                                   ScoSet1S1 = m.ScoreSet1Side1,
                                   ScoSet1S2 = m.ScoreSet1Side2,
                                   ScoSet1TB = PlayerService.GetTieBreakScore(m.ScoreSet1LosingTieBreak, m.ScoreSet1Side1, m.ScoreSet1Side2),
                                   ScoSet2S1 = m.ScoreSet2Side1,
                                   ScoSet2S2 = m.ScoreSet2Side2,
                                   ScoSet2TB = PlayerService.GetTieBreakScore(m.ScoreSet2LosingTieBreak, m.ScoreSet2Side1, m.ScoreSet2Side2),
                                   ScoSet3S1 = m.ScoreSet3Side1,
                                   ScoSet3S2 = m.ScoreSet3Side2,
                                   ScoSet3TB = PlayerService.GetTieBreakScore(m.ScoreSet3LosingTieBreak, m.ScoreSet3Side1, m.ScoreSet3Side2),
                                   ScoSet4S1 = m.ScoreSet4Side1,
                                   ScoSet4S2 = m.ScoreSet4Side2,
                                   ScoSet4TB = PlayerService.GetTieBreakScore(m.ScoreSet4LosingTieBreak, m.ScoreSet4Side1, m.ScoreSet4Side2),
                                   ScoSet5S1 = m.ScoreSet5Side1,
                                   ScoSet5S2 = m.ScoreSet5Side2,
                                   ScoSet5TB = PlayerService.GetTieBreakScore(m.ScoreSet5LosingTieBreak, m.ScoreSet5Side1, m.ScoreSet5Side2),
                                   Score = m.Score?.Trim(),
                                   ScoreReversed = m.ScoreReversed?.Trim(),
                                   m.PlayStatusCode,
                                   m.PlayStatusDesc,
                                   m.ResultStatusCode
                               }).ToList();

                var result = Mapper.Map<ICollection<TieResultsViewModel>>(lResult);
                //result.ForEach(delegate (int idx, TieResultsViewModel x) { x.RubberNumber = idx + 1; });

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

        public ICollection<TieDetailsViewModel> SearchTies(string searchText1, string searchText2,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{searchText1}vs{searchText2}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as ICollection<TieDetailsViewModel>;
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

                var result = (from tie in repo.GetMany<Tie>(
                        x => (x.Side1NationCode.Contains(searchText1) && x.Side2NationCode.Contains(searchText2))
                            || (x.Side1NationCode.Contains(searchText2) && x.Side2NationCode.Contains(searchText1))
                            || (x.Side1NationCode.Contains(searchText1) && x.Side2NationName.Contains(searchText2))
                            || (x.Side1NationCode.Contains(searchText2) && x.Side2NationName.Contains(searchText1))
                            || (x.Side1NationName.Contains(searchText1) && x.Side2NationCode.Contains(searchText2))
                            || (x.Side1NationName.Contains(searchText2) && x.Side2NationCode.Contains(searchText1))
                            || (x.Side1NationName.Contains(searchText1) && x.Side2NationName.Contains(searchText2))
                            || (x.Side1NationName.Contains(searchText2) && x.Side2NationName.Contains(searchText1))
                    )
                    orderby tie.StartDate descending
                    select new TieDetailsViewModel
                    {
                        TieId = tie.PublicTieId,
                        PublicTieId = tie.PublicTieId,
                        Venue = tie.Venue,
                        StartDate = tie.StartDate?.ToString(StartDateFormat),
                        EndDate = tie.EndDate?.ToString(EndDateFormat),
                        HostNationCode = tie.HostNationCode,
                        HostNationName = tie.HostNationName,
                        SurfaceCode = tie.SurfaceCode,
                        Surface = tie.SurfaceDesc,
                        IndoorOutdoor = tie.IndoorOutdoorFlag.GetIndoorOutdoor(),
                        TimeOfPlayDay1 = tie.TimeOfPlayDay1,
                        TimeOfPlayDay2 = tie.TimeOfPlayDay2,
                        TimeOfPlayDay3 = tie.TimeOfPlayDay3,
                        Side1NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1NationCode : tie.Side2NationCode,
                        Side1NationName = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1NationName : tie.Side2NationName,
                        Side2NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2NationCode : tie.Side1NationCode,
                        Side2NationName = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2NationName : tie.Side1NationName,
                        Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? $"{tie.Side1Score}:{tie.Side2Score}" : $"{tie.Side2Score}:{tie.Side1Score}",
                        PlayStatusCode = tie.PlayStatusCode
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

        public TieViewModel GetTie(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{publicTieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as TieViewModel;
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

                var lResult = (from tie in repo.GetMany<Tie>(x => x.PublicTieId == publicTieId)
                               select new
                               {
                                   TieId = tie.PublicTieId,
                                   TieResult = tie.ResultShort,
                                   TieYear = tie.Year,
                                   EventDivision = tie.EventDivisionCode,
                                   EventZone = tie.EventZoneCode,
                                   EventSubGroup = tie.EventSubGroupCode,
                                   EventClass = tie.EventClassificationCode,
                                   EventDrawStructure = tie.EventDrawsheetStructureCode,
                                   EventRound = tie.DrawsheetRoundDesc,
                                   TieDate = $"{tie.StartDate.FormatShortByLanguage(language)} - {tie.EndDate.FormatMediumByLanguage(language)}",
                                   TieVenue = tie.Venue,
                                   tie.SurfaceCode,
                                   IndoorOutdoor = tie.IndoorOutdoorFlag,
                                   Side1NationCode = (tie.EventDrawsheetStructureCode.SqlNotEquals(DcR) && tie.HostNationCode == tie.Side2NationCode) ? tie.Side2NationCode : tie.Side1NationCode,
                                   Side1NationName = (tie.EventDrawsheetStructureCode.SqlNotEquals(DcR) && tie.HostNationCode == tie.Side2NationCode) ? tie.Side2NationName : tie.Side1NationName,
                                   Side1Score = (tie.EventDrawsheetStructureCode.SqlNotEquals(DcR) && tie.HostNationCode == tie.Side2NationCode) ? tie.Side2Score : tie.Side1Score,
                                   Side2NationCode = (tie.EventDrawsheetStructureCode.SqlNotEquals(DcR) && tie.HostNationCode == tie.Side2NationCode) ? tie.Side1NationCode : tie.Side2NationCode,
                                   Side2NationName = (tie.EventDrawsheetStructureCode.SqlNotEquals(DcR) && tie.HostNationCode == tie.Side2NationCode) ? tie.Side1NationName : tie.Side2NationName,
                                   Side2Score = (tie.EventDrawsheetStructureCode.SqlNotEquals(DcR) && tie.HostNationCode == tie.Side2NationCode) ? tie.Side1Score : tie.Side2Score,
                                   tie.HostNationCode
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"Tie not found: publicTieId={publicTieId}");
                    return null;
                }

                var result = Mapper.Map<TieViewModel>(lResult);

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
        /// This function is very heavy so we need to cache the result for long time in order to avoid the performance issue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="side1NationCode"></param>
        /// <param name="side2NationCode"></param>
        /// <returns></returns>
        private int?[] GetWinLosssCount(DataSource source, string side1NationCode, string side2NationCode)
        {
            var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
            var cacheTimeout = GetCacheTimeout(cachePrefix);
            var cacheKey = $"{cachePrefix}.{source}.{side1NationCode}.{side2NationCode}";
            var cacheValue = MemoryCache.Get(cacheKey) as int?[];
            if (cacheValue != null)
            {
                return cacheValue;
            }
            var stopWatch = Stopwatch.StartNew();

            var repo = source == DataSource.Dc ? DavisCupRepo : FedCupRepo;
            var lResult = side1NationCode == null || side2NationCode == null ? new { Side1H2HWin = (int?)0, Side2H2HWin = (int?)0 } :
                (from nh1 in repo.GetMany<NationHistory>(x => x.NationCode == side1NationCode, true)
                 from nh2 in repo.GetMany<NationHistory>(x => x.NationCode == side2NationCode, true)
                 from nat in repo.GetMany<NationActivityTie>(x => x.NationCode == nh1.HistoricNationCode && x.OpponentNationCode == nh2.HistoricNationCode && PcPlayeds.Contains(x.PlayStatusCode))
                 where (side1NationCode != Srb && side2NationCode != Srb) || nat.Year >= SrbYear
                 group nat by 1 into g
                 select new
                 {
                     Side1H2HWin = g.Distinct().Sum(x => x.WinCount),
                     Side2H2HWin = g.Distinct().Sum(x => x.LossCount)
                 }).FirstOrDefault();

            var result = new[] { lResult?.Side1H2HWin, lResult?.Side2H2HWin };

            stopWatch.Stop();
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{cacheKey}, cacheTimeout={cacheTimeout}, Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
            }

            MemoryCache.Set(cacheKey, result, DateTimeOffset.Now.AddSeconds(cacheTimeout));

            return result;
        }

        private static string GetRoundDesc(DataSource source, string divisionCode, int roundNumber, string roundCode, string eventCode, int roundCount, int tieCount)
        {
            var result =
                source == DataSource.Dc
                    ? EdcG.Contains(divisionCode)
                        ? (eventCode == "M"
                            ? roundNumber.GetRoundDesc()
                            : ((roundNumber == 1) && (roundCount == 1)
                                ? "Play-offs"
                                : roundNumber.GetRoundDesc() + " Play-offs"))
                        : EdcWg.Contains(divisionCode)
                            ? (eventCode == "PO"
                                ? "Play-offs"
                                : roundNumber == 1
                                    ? roundNumber.GetRoundDesc()
                                    : tieCount == 4
                                        ? "Quarterfinals"
                                        : roundCode.GetRoundDesc())
                            : roundNumber == 1 ? roundNumber.GetRoundDesc() : roundCode.GetRoundDesc()
                    : roundNumber == 1 ? roundNumber.GetRoundDesc() : roundCode.GetRoundDesc();
            //if (result == null)
            //{
            //    throw new ArgumentException(
            //        $"Can not determine round description with source={source}, divisionCode={divisionCode}, roundNumber={roundNumber}, eventCode={eventCode}, roundCount={roundCount}");
            //}
            return result;
        }

        #region BTD Customization

        public TieDetailsAppViewModel GetTieDetails(string publicTieId,
            Language language = Language.En, DataSource source = DataSource.Dc, bool useCache = true)
        {
            try
            {
                if (source != DataSource.Dc && source != DataSource.Fc) throw new ArgumentException($"source {source} is not supported");
                var cachePrefix = $"{CacheNameSpace}.{CurrentMethodName}";
                var cacheTimeout = GetCacheTimeout(cachePrefix);
                var cacheKey = $"{cachePrefix}.{language}.{source}.{publicTieId}";
                if (useCache)
                {
                    var cacheValue = MemoryCache.Get(cacheKey) as TieDetailsAppViewModel;
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

                var lResult = (from tie in repo.GetMany<Tie>(x => x.PublicTieId == publicTieId)
                               let wl = GetWinLosssCount(source, tie.Side1NationCode, tie.Side2NationCode)
                               let side1NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1NationCode : tie.Side2NationCode
                               let side2NationCode = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2NationCode : tie.Side1NationCode
                               from nt1 in repo.GetMany<NationTranslated>(x => x.NationCode == side1NationCode, true)
                               from nt2 in repo.GetMany<NationTranslated>(x => x.NationCode == side2NationCode, true)
                               select new
                               {
                                   TieId = tie.PublicTieId,
                                   tie.EventName,
                                   RoundDesc = tie.DrawsheetRoundDesc,
                                   tie.Venue,
                                   StartDate = tie.StartDate?.ToString(StartDateFormat),
                                   EndDate = tie.EndDate?.ToString(EndDateFormat),
                                   tie.HostNationCode,
                                   tie.HostNationName,
                                   tie.SurfaceCode,
                                   Surface = tie.SurfaceDesc,
                                   tie.SurfaceTypeDesc,
                                   tie.BallDesc,
                                   IndoorOutdoor = tie.IndoorOutdoorFlag.GetIndoorOutdoor(),
                                   tie.TimeOfPlayDay1,
                                   tie.TimeOfPlayDay2,
                                   tie.TimeOfPlayDay3,
                                   Side1NationCode = side1NationCode,
                                   Side1NationName = nt1.GetNationByLanguage(language).Replace("Republic", "Rep."),
                                   Side2NationCode = side2NationCode,
                                   Side2NationName = nt2.GetNationByLanguage(language).Replace("Republic", "Rep."),
                                   Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? $"{tie.Side1Score}:{tie.Side2Score}" : $"{tie.Side2Score}:{tie.Side1Score}",
                                   Side1Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side1Score : tie.Side2Score,
                                   Side2Score = (tie.HomeSide == 1 || tie.HomeSide == null) ? tie.Side2Score : tie.Side1Score,
                                   Side1NationH2HWinCount = (tie.HomeSide == 1 || tie.HomeSide == null) ? wl[0] : wl[1],
                                   Side2NationH2HWinCount = (tie.HomeSide == 1 || tie.HomeSide == null) ? wl[1] : wl[0],
                                   IsSquadNominated = repo.GetMany<NationSquad>(x => x.TieID == tie.TieID).Any(),
                                   IsDrawsMade = repo.GetMany<Match>(x => x.TieID == tie.TieID).Any(),
                                   tie.PlayStatusCode,
                                   tie.ResultStatusCode,
                                   Report = CmsService.GetItfBaselineContents(CmsRepo, tie.TieID, AssetTypeTie, "info", language.ToString()),
                                   Results = GetTieResultsWeb(publicTieId, language, source, useCache),
                                   Nominations = GetTieNominations(publicTieId, language, source, useCache)
                               }).FirstOrDefault();

                if (lResult == null)
                {
                    Logger.Warn($"Tie not found: publicTieId={publicTieId}");
                    return null;
                }

                var result = Mapper.Map<TieDetailsAppViewModel>(lResult);

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
