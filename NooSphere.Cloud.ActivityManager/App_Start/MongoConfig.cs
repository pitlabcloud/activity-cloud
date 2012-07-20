using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.App_Start
{
    public class MongoConfig
    {
        public static void RegisterMongoMap()
        {
            BsonClassMap.RegisterClassMap<NooSphere.Core.ActivityModel.Activity>();
            BsonClassMap.RegisterClassMap<NooSphere.Core.ActivityModel.Action>();
            BsonClassMap.RegisterClassMap<NooSphere.Core.ActivityModel.Device>();
            BsonClassMap.RegisterClassMap<NooSphere.Core.ActivityModel.FriendRequest>();
            BsonClassMap.RegisterClassMap<NooSphere.Core.ActivityModel.Resource>();
            BsonClassMap.RegisterClassMap<NooSphere.Core.ActivityModel.User>();
        }
    }
}