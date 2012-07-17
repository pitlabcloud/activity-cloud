using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NooSphere.Cloud.ActivityManager.App_Start;
using SignalR;
using SignalR.Redis;

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
            MessageBusConfig.RegisterMessageBus();
            MongoConfig.RegisterMongoMap();
        }
    }
}