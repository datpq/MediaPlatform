using ITF.MediaPlatform.API.App_Start;
using System.Web;
using System.Web.Http;

namespace ITF.MediaPlatform.API
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            MyAppStart.RegisterMappings();
        }
    }
}
