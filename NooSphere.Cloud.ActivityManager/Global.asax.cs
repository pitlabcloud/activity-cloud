using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;

namespace NooSphere.Cloud.ActivityManager
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "Api",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "FilesApi",
                routeTemplate: "api/files/{id}",
                defaults: new { controller = "Files", id = RouteParameter.Optional }
            );

            /**
             * To maintain old API
             */
            routes.MapHttpRoute(
                name: "OldRest",
                routeTemplate: "activitymanager/activities/{id}",
                defaults: new { controller = "OldRest", id = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "OldRestHello",
                routeTemplate: "activitymanager/{id}",
                defaults: new { controller = "OldRestHello", id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            
            BundleTable.Bundles.RegisterTemplateBundles();
        }
    }
}