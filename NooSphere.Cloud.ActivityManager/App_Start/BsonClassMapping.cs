using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.App_Start
{
    public class BsonClassMapping
    {
        public static void RegisterClassMaps()
        {
            BsonClassMap.RegisterClassMap<Activity>();
            BsonClassMap.RegisterClassMap<Device>();
            BsonClassMap.RegisterClassMap<User>();
        }
    }
}