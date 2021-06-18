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
using ITF.DataServices.SDK.Models.ViewModels.Cms;
using NLog;

namespace ITF.MediaPlatform.API.Controllers
{
    public class CmsController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICmsService _cmsService;

        public CmsController(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        [HttpGet]
        [ActionName("CmsTicketsInfo")]
        public IHttpActionResult GetCmsTicketsInfo(
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

                var result = _cmsService.GetCmsTicketsInfo(lang, dataSource, useCache);

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
        [ActionName("CmsTieRelatedAssets")]
        public IHttpActionResult GetCmsTieRelatedAssets(string publicTieId,
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

                var result = _cmsService.GetCmsTieRelatedAssets(publicTieId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<TieRelatedAssetsViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("CmsPhotoGalleries")]
        public IHttpActionResult GetCmsPhotoGalleries(int nodeId,
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

                var result = _cmsService.GetCmsPhotoGalleries(nodeId, lang, dataSource, useCache);

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
        [ActionName("CmsPhotoGallery")]
        public IHttpActionResult GetCmsPhotoGallery(int nodeId, int top = 0,
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

                var result = _cmsService.GetCmsPhotoGallery(nodeId, top, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<CmsPhotoGalleryViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("CmsHtmlComponent")]
        public IHttpActionResult GetCmsHtmlComponent(int nodeId,
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

                var result = _cmsService.GetCmsHtmlComponent(nodeId, lang, dataSource, useCache);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                if (result == null) return NotFound();
                return Ok(new[] { result } as ICollection<HtmlComponentViewModel>);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        [HttpGet]
        [ActionName("SearchCmsContent")]
        public IHttpActionResult GetSearchCmsContent(int nodeId, string search,
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

                var result = _cmsService.GetSearchCmsContent(nodeId, search, lang, dataSource, useCache);

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
