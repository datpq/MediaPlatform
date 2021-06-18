using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Cors;
using ITF.DataServices.SDK;
using NLog;

namespace ITF.MediaPlatform.API
{
    public static class WebApiConfig
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string[] OptionalParameters = { "language", "source", "useCache" };
        private static readonly string CtrtLanguage = string.Join("|", Enum.GetValues(typeof(Language)).OfType<Language>()).ToLower();
        private static readonly string CtrtSource = string.Join("|", Enum.GetValues(typeof(DataSource)).OfType<DataSource>()).ToLower();
        //private static readonly string CtrtSegChars = $"(?!({CtrtLanguage}|{CtrtSource})).*";
        private static readonly string CtrtSegChars =
            (string.Concat(Enum.GetValues(typeof(Language)).OfType<Language>().Select(x => $"(?!^{x}$)")) +
            string.Concat(Enum.GetValues(typeof(DataSource)).OfType<DataSource>().Select(x => $"(?!^{x}$)")) + ".*").ToLower();
        private static readonly string[] BaseRouteTemplates =
        {
            "{source}/{action}",
            "{source}/{action}/{language}",
            "{action}",
            "{action}/{source}",
            "{action}/{language}",
            "{action}/{language}/{source}",
            "{action}/{source}/{language}"
        };

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Other configuration omitted
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            var routeId = 0;

            const string controllerClassSuffix = "Controller";
            //const string ctrtSegInt = @"\d+";
            //const string ctrtSegYear = @"\d{4}";

            // Find all class Controller that has at least one public method with ActionNameAttribute
            var apiControllerType = typeof(ApiController);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x != apiControllerType && apiControllerType.IsAssignableFrom(x) && x.Name.EndsWith(controllerClassSuffix))
                .Where(x => x.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Any(y => y.GetCustomAttributes(typeof(ActionNameAttribute), true).Any())).ToList();

            types.ForEach(x =>
            {
                var defaults = new
                {
                    controller = x.Name.Substring(0, x.Name.Length - controllerClassSuffix.Length),
                    language = Constants.DefaultLanguage,
                    source = Constants.DefaultSource
                };
                var controllerRoutes = new Dictionary<string, string>();
                var routePrefix = ((RoutePrefixAttribute)x.GetCustomAttributes(typeof(RoutePrefixAttribute), true).FirstOrDefault())?.Prefix;
                routePrefix = routePrefix == null ? string.Empty : routePrefix + "/";

                // Get all public methods that has ActionNameAttribute
                x.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(y => y.GetCustomAttributes(typeof(ActionNameAttribute), true).Any()).ToList().ForEach(y =>
                    {
                        var actionName = ((ActionNameAttribute) y.GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault())?.Name;
                        var route = string.Empty;
                        y.GetParameters().Where(z => !OptionalParameters.Contains(z.Name)).ToList().ForEach(z =>
                            {
                                if (z.IsOptional) // if optional, create a new route
                                {
                                    BaseRouteTemplates.Select(o => routePrefix + o + route).ToList().ForEach(o =>
                                    {
                                        controllerRoutes[o] = controllerRoutes.ContainsKey(o)
                                            ? controllerRoutes[o] + "|" + actionName
                                            : actionName;
                                    });
                                }
                                route = route + $"/{{{z.Name}}}";
                            });
                        //at the end, create a new route
                        BaseRouteTemplates.Select(o => routePrefix + o + route).ToList().ForEach(o =>
                        {
                            controllerRoutes[o] = controllerRoutes.ContainsKey(o)
                                ? controllerRoutes[o] + "|" + actionName
                                : actionName;
                        });
                    });

                foreach (var kvp in controllerRoutes)
                {
                    Logger.Info($"{kvp.Key}, {defaults.controller}{controllerClassSuffix}, {kvp.Value}");
                    config.Routes.MapHttpRoute(
                        name: $"Api{++routeId}",
                        routeTemplate: kvp.Key,
                        defaults: defaults,
                        constraints: new
                        {
                            action = kvp.Value,
                            //year = ctrtSegYear,
                            nationcode = CtrtSegChars,
                            section = CtrtSegChars,
                            subsection = CtrtSegChars,
                            language = CtrtLanguage,
                            source = CtrtSource
                        });
                }
            });

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
