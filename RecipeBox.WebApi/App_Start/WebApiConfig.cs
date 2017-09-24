using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RecipeBox.WebApi
{
    public static class WebApiConfig
    {
        private static ILog Log = LogManager.GetLogger(typeof(WebApiConfig));

        public static void Register(HttpConfiguration config)
        {
            // Attribute routing.
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                            name: "DefaultApi",
                            routeTemplate: "api/{controller}/{id}",
                            defaults: new { id = RouteParameter.Optional });

            // TODO: JSON formatter setting to handle recursive references.
            //config.Formatters
            //        .JsonFormatter
            //        .SerializerSettings
            //        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
