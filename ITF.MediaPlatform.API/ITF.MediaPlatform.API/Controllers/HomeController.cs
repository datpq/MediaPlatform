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
    public class HomeController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPlayerService _playerService;

        public HomeController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        [ActionName("CommitmentAwardEligiblePlayers")]
        [AllowAnonymous]
        public IHttpActionResult GetCommitmentAwardEligiblePlayers(string language = Constants.DefaultLanguage,
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

                var result = _playerService.GetCommitmentAwardEligiblePlayers(lang, dataSource, useCache);

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
        [ActionName("CommitmentAwardOneTieToPlayPlayers")]
        [AllowAnonymous]
        public IHttpActionResult GetCommitmentAwardOneTieToPlayPlayers(string language = Constants.DefaultLanguage,
            string source = Constants.DefaultLanguage, bool useCache = true)
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

                var result = _playerService.GetCommitmentAwardOneTieToPlayPlayers(lang, dataSource, useCache);

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
        [ActionName("FeaturedPlayers")]
        [AllowAnonymous]
        public IHttpActionResult GetFeaturedPlayers(int id, string language = Constants.DefaultLanguage,
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

                var result = _playerService.GetFeaturedPlayers(id, lang, dataSource, useCache);

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
        [Route("player/{id}")]
        public IHttpActionResult GetPlayerCoreProfile(int id, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _playerService.GetPlayerCore(id, Language.En, DataSource.Itf, useCache);

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
        [ActionName("Player")]
        public IHttpActionResult GetPlayer(int id,
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

                var result = _playerService.GetPlayer(id, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<PlayerViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("PlayersFromTie")]
        public IHttpActionResult GetPlayersFromTie(string publicTieId, string language = Constants.DefaultLanguage,
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

                var result = _playerService.GetPlayersFromTie(publicTieId, lang, dataSource, useCache);

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
        [ActionName("PlayerActivity")]
        public IHttpActionResult GetPlayerActivity(int id,
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

                var result = _playerService.GetPlayerActivity(id, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<PlayerActivityViewModelOld>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("HeadToHeadPlayerToPlayer")]
        public IHttpActionResult GetHeadToHeadPlayerToPlayer(int id, int opponentId,
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

                var result = _playerService.GetHeadToHeadPlayerToPlayer(id, opponentId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<HeadToHeadPlayerToPlayerViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("HeadToHeadPlayerToNation")]
        public IHttpActionResult GetHeadToHeadPlayerToNation(int id, string opponentNationCode,
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

                var result = _playerService.GetHeadToHeadPlayerToNation(id, opponentNationCode, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<HeadToHeadPlayerToNationViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("SearchPlayers")]
        public IHttpActionResult SearchPlayers(string searchText, string language = Constants.DefaultLanguage,
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

                var result = _playerService.SearchPlayers(searchText, lang, dataSource, useCache);

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
        [ActionName("IsAlive")]
        [AllowAnonymous]
        public IHttpActionResult IsAlive(string language = Constants.DefaultLanguage, string source = Constants.DefaultSource, bool useCache = true)
        {
            try
            {
                var stopWatch = Stopwatch.StartNew();
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                return Ok("Ok");
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }
    }
}