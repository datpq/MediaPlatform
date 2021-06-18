using System;
using System.Web.Http;
using System.Net.Http;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Web;
using ITF.DataServices.Authentication.ActionFilters;
using ITF.DataServices.Authentication.Services;
using ITF.DataServices.Authentication.Filters;
using NLog;

namespace ITF.DataServices.Authentication.Controllers
{
    public class AuthenticateController : ApiController
    {
        #region Private variable.

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITokenService _tokenService;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public AuthenticateController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        #endregion

        [HttpGet]
        [Route("GetIp")]
        [AllowAnonymous]
        public IHttpActionResult GetIp()
        {
            try
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Dumping Request Headers: \n" + Request.Headers);
                }
                var result = GetClientIp(Request);

                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                Logger.Error(e, "GetIp error");
                return Content(HttpStatusCode.BadRequest, "Unexpected error");
            }
        }

        public static string GetClientIp(HttpRequestMessage request)
        {
            if (request.Headers.Contains("X-Forwarded-For"))
            {
                var forwardedIps = request.Headers.GetValues("X-Forwarded-For").FirstOrDefault();
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"X-Forwarded-For: {forwardedIps}");
                }
                return forwardedIps?.Split(',').FirstOrDefault();
            }
            if (request.Headers.Contains("Incap-Client-IP"))
            {
                return request.Headers.GetValues("Incap-Client-IP").FirstOrDefault();
            }
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            return null;
        }

        /// <summary>00
        /// Authenticates user and returns token with expiry.
        /// </summary>
        /// <returns></returns>
        [ApiAuthenticationFilter]
        [Route("login")]
        [Route("authenticate")]
        [HttpPost]
        public HttpResponseMessage Login()
        {
            HttpResponseMessage result = null;
            var token = string.Empty;
            var stopWatch = Stopwatch.StartNew();
            if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                {
                    var userId = basicAuthenticationIdentity.UserId;
                    result = GetAuthToken(userId, out token);
                }
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"RequestUrl: {Request?.RequestUri}, Token={token}, ProcessingTime={stopWatch.Elapsed.ToStringStandardFormat()}");
            }
            return result;
        }

        [AuthorizationRequired]
        [HttpGet, Route("ping")]
        public IHttpActionResult Ping()
        {
            if (Logger.IsDebugEnabled && Request != null)
            {
                string token = null;
                if (Request.Headers.Contains(AuthorizationRequiredAttribute.Token))
                {
                    token = Request.Headers.GetValues(AuthorizationRequiredAttribute.Token).First();
                }
                Logger.Debug($"RequestUrl: {Request.RequestUri}, Token={token}");
            }
            return Ok();
        }

        /// <summary>
        /// Returns auth token for the validated user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private HttpResponseMessage GetAuthToken(int userId, out string token)
        {
            var tokenEntity = _tokenService.GenerateToken(userId);
            token = tokenEntity.AuthToken;
            var response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
            response.Headers.Add("Token", tokenEntity.AuthToken);
            response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["AuthTokenExpiry"]);
            response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
            return response;
        }
    }
}