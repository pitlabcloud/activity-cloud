#region License

// Copyright (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)
// 
// Pervasive Interaction Technology Laboratory (pIT lab)
// IT University of Copenhagen
// 
// This library is free software; you can redistribute it and/or 
// modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
// as published by the Free Software Foundation. Check 
// http://www.gnu.org/licenses/gpl.html for details.

#endregion

#region

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Redis;
using System;
using System.Configuration;

#endregion

namespace NooSphere.Cloud.ActivityManager.App_Start
{
    public class MessageBusConfig
    {
        public static void RegisterMessageBus(IDependencyResolver resolver)
        {
            string server = ConfigurationManager.AppSettings["redis.server"];
            string port = ConfigurationManager.AppSettings["redis.port"];
            string password = ConfigurationManager.AppSettings["redis.password"];

            resolver.UseRedis(server, Int32.Parse(port), password, new[] { "ActivityCloud"});
        }
    }
}