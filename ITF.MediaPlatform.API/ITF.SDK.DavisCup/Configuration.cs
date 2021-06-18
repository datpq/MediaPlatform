using System.Configuration;

namespace ITF.SDK.DavisCup
{
    public class Configuration : IConfiguration
    {
        public const string TokenHeader = "Token";
        public string ApiUrl { get { return ConfigurationManager.AppSettings["DavisCupUrl"]; } }
    }
}
