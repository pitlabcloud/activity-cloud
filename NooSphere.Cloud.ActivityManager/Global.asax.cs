using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;
using NooSphere.Cloud.ActivityManager.App_Start;
using SignalR;
using SignalR.Redis;
using MVC.ApiExplorer;
using System.Configuration;

namespace NooSphere.Cloud.ActivityManager
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DocumentationConfig.RegisterDocumentation(GlobalConfiguration.Configuration.Services);

            string server = ConfigurationManager.AppSettings["redis.server"];
            string port = ConfigurationManager.AppSettings["redis.port"];
            string password = ConfigurationManager.AppSettings["redis.password"];

            GlobalHost.DependencyResolver.UseRedis(server, Int32.Parse(port), password, "SignalR.Redis.Sample");
        }
    }
}