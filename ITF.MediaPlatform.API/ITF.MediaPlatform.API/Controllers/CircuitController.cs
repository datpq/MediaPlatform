using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using Elmah;
using ITF.DataServices.SDK;
using ITF.DataServices.SDK.Interfaces;
using NLog;

namespace ITF.MediaPlatform.API.Controllers
{
    public class CircuitController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IOlympicService _olympicService;

        public CircuitController(IOlympicService olympicService)
        {
            _olympicService = olympicService;
        }

        [HttpGet]
        [ActionName("OlympicTennis")]
        public IHttpActionResult GetOlympicTennis(
            string language = Constants.DefaultLanguage, string source = "itf", bool useCache = true)
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

                var result = _olympicService.GetOlympicTennis(lang, dataSource, useCache);

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
        [ActionName("GrandSlam")]
        public IHttpActionResult GetGrandSlam(string nationCode,
            string language = Constants.DefaultLanguage, string source = "itf", bool useCache = true)
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

                var result = _olympicService.GetGrandSlam(nationCode, lang, dataSource, useCache);

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
    }
}
