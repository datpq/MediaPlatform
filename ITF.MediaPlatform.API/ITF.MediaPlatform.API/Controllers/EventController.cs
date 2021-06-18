using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using Elmah;
using ITF.DataServices.Authentication.ActionFilters;
using ITF.DataServices.SDK;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models.ViewModels;
using NLog;

namespace ITF.MediaPlatform.API.Controllers
{
    public class EventController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
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

                var result = _eventService.GetRoundRobinEvents(year, section, subSection, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("Tournaments")]
        public IHttpActionResult GetTournaments(int year, string section, string subSection,
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

                var result = _eventService.GetTournaments(year, section, subSection, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("RoundRobinNominations")]
        public IHttpActionResult GetRoundRobinNominations(int year, string section, string subSection,
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

                var result = _eventService.GetRoundRobinNominations(year, section, subSection, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("DrawSheet")]
        public IHttpActionResult GetDrawSheet(int year, string section, string subSection,
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

                var result = _eventService.GetDrawSheet(year, section, subSection, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new [] { result } as ICollection<DrawSheetViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("ResultsByYearLite")]
        public IHttpActionResult GetResultsByYearLite(int year, string section = null, string subSection = null,
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

                var result = _eventService.GetDrawSheet(year, section, subSection, lang, dataSource, useCache);
                result = result.CloneJson();// Clone to not affect the result in Cache

                if (result == null) return NotFound();
                result.Events.ToList().ForEach(x =>
                {
                    x.Rounds.ToList().ForEach(y =>
                    {
                        y.Ties.Where(z => !z.IsSide1Hosting).ToList().ForEach(tie =>
                        {
                            var tmp = tie.Side1NationCode;
                            tie.Side1NationCode = tie.Side2NationCode;
                            tie.Side2NationCode = tmp;

                            tmp = tie.Side1NationName;
                            tie.Side1NationName = tie.Side2NationName;
                            tie.Side2NationName = tmp;

                            tmp = tie.Side1NationNameES;
                            tie.Side1NationNameES = tie.Side2NationNameES;
                            tie.Side2NationNameES = tmp;

                            var tmpI = tie.Side1H2HWin;
                            tie.Side1H2HWin = tie.Side2H2HWin;
                            tie.Side2H2HWin = tmpI;

                            var tmpIn = tie.Side1Seeding;
                            tie.Side1Seeding = tie.Side2Seeding;
                            tie.Side2Seeding = tmpIn;

                            tmpIn = tie.Side1Score;
                            tie.Side1Score = tie.Side2Score;
                            tie.Side2Score = tmpIn;
                        });
                    });
                });
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                return Ok(new[] { result } as ICollection<DrawSheetViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("ResultsByYear")]
        public IHttpActionResult GetResultsByYear(int year,
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

                var result = _eventService.GetResultsByYear(year, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<ResultsByYearViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("EventYears")]
        public IHttpActionResult GetEventYears(string section, string subSection,
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

                var result = _eventService.GetEventYears(section, subSection, dataSource, useCache);

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
        [ActionName("NodeRelatedTieDetails")]
        public IHttpActionResult GetNodeRelatedTieDetails(int nodeId, string language = Constants.DefaultLanguage,
            string source = Constants.DefaultSource, bool useCache = true)
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

                var result = _eventService.GetNodeRelatedTieDetails(nodeId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if ((result == null) || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("TieDetailsWeb")]
        public IHttpActionResult GetTieDetailsWeb(string publicTieId,
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

                var result = _eventService.GetTieDetailsWeb(publicTieId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<TieDetailsWebViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("TieDetails")]
        public IHttpActionResult GetTieDetails(int tieId,
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

                var result = _eventService.GetTieDetails(tieId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<TieDetailsViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("TieNominations")]
        public IHttpActionResult GetTieNominations(string publicTieId,
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

                var result = _eventService.GetTieNominations(publicTieId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("TieResultsWeb")]
        public IHttpActionResult GetTieResultsWeb(string publicTieId,
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

                var result = _eventService.GetTieResultsWeb(publicTieId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("TieResults")]
        public IHttpActionResult GetTieResults(int tieId,
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

                var result = _eventService.GetTieResults(tieId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("SearchTies")]
        public IHttpActionResult SearchTies(string searchText1, string searchText2, string language = Constants.DefaultLanguage,
            string source = Constants.DefaultSource, bool useCache = true)
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

                var result = _eventService.SearchTies(searchText1, searchText2, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if ((result == null) || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }
    }
}
