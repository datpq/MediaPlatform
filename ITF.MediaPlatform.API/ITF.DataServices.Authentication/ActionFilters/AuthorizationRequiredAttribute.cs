using System.Configuration;
using System.Diagnostics;
using ITF.DataServices.Authentication.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ITF.DataServices.Authentication.Controllers;
using NLog;
using RestSharp.Extensions;

namespace ITF.DataServices.Authentication.ActionFilters
{
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public const string Token = "Token";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var stopWatch = Stopwatch.StartNew();
            if (filterContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                filterContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"No authentication required because of AllowAnonymous attribute. Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                base.OnActionExecuting(filterContext);
                return;
            }

            //  Get API key provider
            var provider = filterContext.ControllerContext.Configuration
                .DependencyResolver.GetService(typeof(ITokenService)) as ITokenService;

            if (filterContext.Request.Headers.Contains(Token))
            {
                var tokenValue = filterContext.Request.Headers.GetValues(Token).First();

                // Validate Token
                if (provider != null && provider.ValidateToken(tokenValue))
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug($"Token is valid {tokenValue}. Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                    }
                    base.OnActionExecuting(filterContext);
                    return;
                }
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"Token is no more valid. {tokenValue}");
                }
            }

            var ip = AuthenticateController.GetClientIp(filterContext.Request);
            var allowAnonymousIpRegex = ConfigurationManager.AppSettings["Authentication.AllowAnonymousIpRegex"] ?? string.Empty;
            if (!string.IsNullOrEmpty(allowAnonymousIpRegex) && ip.Matches(allowAnonymousIpRegex))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"ClientIp={ip}. Matched AllowAnonymousIpRegex. No authentication required. Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
                }
                base.OnActionExecuting(filterContext);
                return;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"ClientIp={ip}. Authentication required. Elapsed={stopWatch.Elapsed.ToStringStandardFormat()}");
            }

            var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request" };
            filterContext.Response = responseMessage;

            base.OnActionExecuting(filterContext);
        }
    }
}