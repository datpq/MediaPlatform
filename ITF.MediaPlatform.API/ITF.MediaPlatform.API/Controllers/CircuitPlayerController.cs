using System;
using System.Net;
using System.Web.Http;
using ITF.DataServices.SDK.Interfaces;
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using Newtonsoft.Json;
using NLog;

namespace ITF.MediaPlatform.API.Controllers
{
    [RoutePrefix("api")]
    public class CircuitPlayerController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IOlympicService _olympicService;
        private readonly IInstagramService _instagramService;

        public CircuitPlayerController(IOlympicService olympicService, IInstagramService instagramService)
        {
            _olympicService = olympicService;
            _instagramService = instagramService;
        }

        #region Player endpoints

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/latest/players")]
        public IHttpActionResult GetLatestOlympicPlayers(string cc, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetLatestPlayers(tournamentType, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/players")]
        public IHttpActionResult GetOlympicPlayers(string cc, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetAllOlympicsPlayers(tournamentType, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/players/search")]
        public IHttpActionResult GetOlympicPlayersForSearch(string cc, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetPlayersForSearch(tournamentType, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion

        #region Player profile endpoints

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/players/{playerId}/headtohead/{opponentPlayerId}")]
        public IHttpActionResult GetOlympicHeadToHead(string cc, int playerId, int opponentPlayerId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetPlayerHeadToHead(tournamentType, playerId, opponentPlayerId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/players/{playerId}/activity")]
        public IHttpActionResult GetOlympicPlayerActivity(string cc, int playerId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetPlayerActivity(tournamentType, playerId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/players/{playerId}/profile")]
        public IHttpActionResult GetOlympicPlayerProfile(string cc, int playerId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetPlayerProfile(tournamentType, playerId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion

        #region result endpoints

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}")]
        public IHttpActionResult GetAllOlympics(string cc, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetAllOlympics(tournamentType, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/events/{eventId}")]
        public IHttpActionResult GetDrawsheetByEventId(string cc, int eventId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetDrawsheetByEventId(tournamentType, eventId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("{cc:regex(^(olympics|paralympics)$)}/{year}/events")]
        public IHttpActionResult GetOlympicEventsByYear(string cc, int year, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var tournamentType = cc == "olympics" ? "OLY" : "PLY";
                var result = _olympicService.GetOlympicEventsByYear(tournamentType, year, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion

        #region Instagram

        [HttpGet]
        [Route("instagram/{profile}")]
        [Route("~/global/api/instagram/{profile}")]
        public IHttpActionResult GetInstagramProfileData(string profile, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _instagramService.GetProfileData(profile, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion

        #region LiveBlogData

        [HttpGet, Route("liveblog/{siteLanguage}/{fileName}")]
        public IHttpActionResult GetLiveBlogData(string siteLanguage, string fileName, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetLiveBlogData(siteLanguage, fileName, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion

        #region Translations

        [HttpGet]
        [Route("translations/{languageIsoCode}")]
        [Route("~/global/api/translations/{languageIsoCode}")]
        public IHttpActionResult GetTranslationsByIso(string languageIsoCode, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetTranslationsByIso(languageIsoCode, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion

        #region HeartAward

        [HttpPost, Route("umbraco/cup/heartaward")]
        public IHttpActionResult VoteUp(CupHeartAwardDataViewModel cupHeartAwardData)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.VoteUp(
                    cupHeartAwardData.Code, cupHeartAwardData.PlayerId,
                    cupHeartAwardData.NationCode, cupHeartAwardData.Name);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion

        #region AssetsController

        [HttpGet, Route("umbraco/assets/{assetId}")]
        public IHttpActionResult GetPlayerImageAssetId(int assetId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetPlayerImageAssetId(assetId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("umbraco/galleries/{webScopeNodeId}")]
        public IHttpActionResult GetGalleriesByWebScopeId(int webScopeNodeId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetGalleriesByWebScopeId(webScopeNodeId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("umbraco/assets/{assetId}/provider")]
        public IHttpActionResult GetAssetProvider(int assetId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetAssetProvider(assetId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("umbraco/assets/{assetId}/related/media")]
        public IHttpActionResult GetRelatedMediaAssets(int assetId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetRelatedMediaAssets(assetId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("umbraco/assets/gallery/{assetId}/{limitImages}")]
        public IHttpActionResult GetGalleryByAssetId(int assetId, int limitImages, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetGalleryByAssetId(assetId, limitImages, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet, Route("umbraco/assets/html/{nodeId}")]
        public IHttpActionResult GetHtmlByNodeId(int nodeId, bool useCache = true)
        {
            try
            {
                if (Logger.IsDebugEnabled && Request != null)
                {
                    Logger.Debug($"RequestUrl: {Request.RequestUri}");
                }
                var result = _olympicService.GetHtmlByNodeId(nodeId, useCache);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        #endregion
    }
}
