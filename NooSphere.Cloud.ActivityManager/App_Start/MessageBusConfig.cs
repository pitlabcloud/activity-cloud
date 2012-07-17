using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using SignalR;
using SignalR.Redis;

namespace NooSphere.Cloud.ActivityManager.App_Start
{
    public class MessageBusConfig
    {
        public static void RegisterMessageBus()
        {
            string server = ConfigurationManager.AppSettings["redis.server"];
            string port = ConfigurationManager.AppSettings["redis.port"];
            string password = ConfigurationManager.AppSettings["redis.password"];

            GlobalHost.DependencyResolver.UseRedis(server, Int32.Parse(port), password, "ActivityCloud");
        }
    }
}