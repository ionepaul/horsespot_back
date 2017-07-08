using HorseSpot.Api.Utils;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HorseSpot.Api
{
    public static class WebApiConfig
    {
        /// <summary>
        /// WebAPi Configuration
        /// </summary>
        /// <param name="config">Http Configuration</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            config.Filters.Add(new HorseSpotExceptionFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
