using EasyPatch.Common.Implementation;
using EasyPatch.Common.Install;
using EasyPatch.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EasyPatch.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.UseEasyPatch();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
