using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using ITF.DataServices.Authentication.ActionFilters;
using ITF.DataServices.SDK;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models.ViewModels;
using NLog;

namespace ITF.MediaPlatform.API.Controllers
{
    public class NationController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly INationService _nationService;

        public NationController(INationService nationService)
        {
            _nationService = nationService;
        }

        [HttpGet]
        [ActionName("NodeRelatedNations")]
        public IHttpActionResult GetNodeRelatedNation(int nodeId, string language = Constants.DefaultLanguage, 
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

                var result = _nationService.GetNodeRelatedNations(nodeId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if ((result == null) || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationProfileWeb")]
        public IHttpActionResult GetNationProfileWeb(string nationCode = null,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                if (nationCode == null) return Content(HttpStatusCode.BadRequest, "NationCode is required");
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _nationService.GetNationProfileWeb(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok( new [] { result } as ICollection<NationProfileWebViewModelOld>);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("Nation")]
        public IHttpActionResult GetNation(string nationCode = null,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                if (nationCode == null) return Content(HttpStatusCode.BadRequest, "NationCode is required");
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _nationService.GetNation(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new [] { result } as ICollection<NationViewModelOld>);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("HeadToHeadNationToNation")]
        public IHttpActionResult GetHeadToHeadNationToNation(string nationCode, string opponentNationCode,
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

                var result = _nationService.GetHeadToHeadNationToNation(nationCode, opponentNationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<HeadToHeadNationToNationViewModel>);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("Nations")]
        public IHttpActionResult GetNations(string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
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

                var result = _nationService.GetNations(lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || result.Count == 0) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("Champions")]
        public IHttpActionResult GetChampions(string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
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

                var result = _nationService.GetChampions(lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || result.Count == 0) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationWinLossRecords")]
        public IHttpActionResult GetNationWinLossRecords(string nationCode = null,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                if (nationCode == null) return Content(HttpStatusCode.BadRequest, "NationCode is required");
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _nationService.GetNationWinLossRecords(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || result.Count == 0) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationPlayersWinLossRecords")]
        public IHttpActionResult GetNationPlayersWinLossRecords(string nationCode, int year,
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

                var result = _nationService.GetNationPlayersWinLossRecords(nationCode, year, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || result.Count == 0) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationStatsRecords")]
        public IHttpActionResult GetNationStatsRecords(string nationCode,
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

                var result = _nationService.GetNationStatsRecords(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<NationStatsRecordsViewModel>);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationPlayersCareerRecords")]
        public IHttpActionResult GetNationPlayersCareerRecords(string nationCode,
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

                var result = _nationService.GetNationPlayersCareerRecords(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || result.Count == 0) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("RankingsNation")]
        public IHttpActionResult GetRankingsNation(string nationCode = null,
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

                var result = _nationService.GetNationRankings(0, lang, dataSource, useCache).AsQueryable().SetComparer(StringComparison.CurrentCultureIgnoreCase);

                var nationRanking = result.SingleOrDefault(x => x.NationCode == nationCode);
                if (nationRanking == null) return NotFound();

                var lstResult = result.Where(x => x.Rank >= nationRanking.Rank - 2 && x.Rank <= nationRanking.Rank + 2);
                var shortResult = Mapper.Map<ICollection<NationRankingCoreViewModel>>(lstResult);
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                return Ok(shortResult);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("Rankings")]
        public IHttpActionResult GetRankings(int top = 0,
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

                var result = _nationService.GetNationRankings(top, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || result.Count == 0) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationAllTimeRecords")]
        public IHttpActionResult GetNationAllTimeRecords(string nationCode = null,
            string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                if (nationCode == null) return Content(HttpStatusCode.BadRequest, "NationCode is required");
                var dataSource = source.ParseDataSource();
                var lang = language.ParseLanguage();

                var result = _nationService.GetNationAllTimeRecords(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<NationAllTimeRecordsViewModel>);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationsGroup")]
        public IHttpActionResult GetNationsGroup(int year,
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

                var result = _nationService.GetNationsGroup(year, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null || result.Count == 0) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("NationPlayers")]
        public IHttpActionResult GetNationPlayers(string nationCode,
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

                var result = _nationService.GetNationPlayers(nationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if ((result == null) || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("SearchNations")]
        public IHttpActionResult SearchNations(string searchText, string language = Constants.DefaultLanguage,
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

                var result = _nationService.SearchNations(searchText, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if ((result == null) || !result.Any()) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }
    }
}
