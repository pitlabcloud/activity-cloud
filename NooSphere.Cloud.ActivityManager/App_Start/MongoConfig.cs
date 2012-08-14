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

using MongoDB.Bson.Serialization;
using NooSphere.Core.ActivityModel;

#endregion

namespace NooSphere.Cloud.ActivityManager.App_Start
{
    public class MongoConfig
    {
        public static void RegisterMongoMap()
        {
            BsonClassMap.RegisterClassMap<Activity>();
            BsonClassMap.RegisterClassMap<Device>();
            BsonClassMap.RegisterClassMap<FriendRequest>();
            BsonClassMap.RegisterClassMap<Resource>();
            BsonClassMap.RegisterClassMap<User>();
        }
    }
}