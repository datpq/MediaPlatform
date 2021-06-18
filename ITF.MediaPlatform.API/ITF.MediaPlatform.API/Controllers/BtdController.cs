using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Elmah;
using ITF.DataServices.Authentication.ActionFilters;
using ITF.DataServices.SDK;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models.ViewModels;
using ITF.DataServices.SDK.Models.ViewModels.Btd;
using ITF.DataServices.SDK.Services;
using ITF.MediaPlatform.API.ViewModels;
using NLog;

namespace ITF.MediaPlatform.API.Controllers
{
    [AuthorizationRequired]
    [RoutePrefix("app")]
    public class BtdController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPlayerService _playerService;
        private readonly INationService _nationService;
        private readonly IEventService _eventService;

        public BtdController(IPlayerService playerService, INationService nationService, IEventService eventService)
        {
            _playerService = playerService;
            _nationService = nationService;
            _eventService = eventService;
        }

        [HttpGet]
        [ActionName("Player")]
        public IHttpActionResult GetBtdPlayer(int id,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _playerService.GetBtdPlayer(id, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("Nation")]
        public IHttpActionResult GetBtdNation(string nationCode,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _nationService.GetBtdNation(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("MyTeam")]
        public IHttpActionResult GetBtdMyTeam(string nationCode,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _nationService.GetBtdMyTeam(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationPlayers")]
        public IHttpActionResult GetBtdNationPlayers(string nationCode,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = new BtdNationPlayersViewModel
                {
                    AllPlayers = Mapper.Map<ICollection<PlayerViewModelCoreCore>>(
                        _nationService.GetNationPlayers(nationCode, lang, dataSource, useCache)),
                    RecentPlayers = _nationService.GetNationRecentPlayers(nationCode, NationService.RecentYears, lang, dataSource, useCache)
                };

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if ((result.AllPlayers == null) || !result.AllPlayers.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("TieDetails")]
        public IHttpActionResult GetTieDetails(string publicTieId,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _eventService.GetTieDetails(publicTieId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<TieDetailsAppViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("RoundRobinEvents")]
        public IHttpActionResult GetRoundRobinEvents(int year, string section, string subSection,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var resultEvents = _eventService.GetRoundRobinEvents(year, section, subSection, lang, dataSource, useCache);
                var resultTournaments = _eventService.GetTournaments(year, section, subSection, lang, dataSource, useCache);
                var resultBtdTournaments = Mapper.Map<ICollection<BtdTournamentViewModel>>(resultTournaments);
                resultBtdTournaments.ToList().ForEach(x => x.SurfaceCode2 = x.SurfaceDesc.ToLower());

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if ((resultEvents == null || !resultEvents.Any())
                    && (resultTournaments == null || !resultTournaments.Any())) return NotFound();
                return Ok(new BtdRoundRobinEventAppViewModel
                {
                    Events = resultEvents,
                    Tournaments = resultBtdTournaments
                });
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }
    }
}
