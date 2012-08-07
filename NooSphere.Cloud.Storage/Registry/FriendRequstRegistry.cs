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

using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NooSphere.Core.ActivityModel;

#endregion

namespace NooSphere.Cloud.Data.Registry
{
    public class FriendRequestRegistry : BaseRegistry
    {
        #region Constructors

        public FriendRequestRegistry(string connectionString) : base(connectionString)
        {
        }

        #endregion

        #region Public Methods

        public bool Add(object obj)
        {
            return base.Add(Collection, obj);
        }

        public List<FriendRequest> Get()
        {
            return base.Get(Collection).Cast<FriendRequest>().ToList();
        }

        public List<FriendRequest> Get(Guid userId)
        {
            return Collection.FindAs<FriendRequest>(Query.EQ("FriendId", userId)).ToList();
        }

        public bool Exists(Guid userId, Guid friendId)
        {
            return Collection.Find(Query.Or(
                Query.And(Query.EQ("UserId", userId), Query.EQ("FriendId", friendId)),
                Query.And(Query.EQ("UserId", friendId), Query.EQ("FriendId", userId)))).Count() > 0;
        }

        public bool Remove(Guid userId, Guid friendId)
        {
            if (!Exists(userId, friendId)) return false;
            return
                Collection.Remove(Query.And(Query.EQ("UserId", userId), Query.EQ("FriendId", friendId)), SafeMode.True).
                    Ok;
        }

        public bool Remove(Guid friendRequestId)
        {
            return Collection.Remove(Query.EQ("_id", friendRequestId), SafeMode.True).Ok;
        }

        #endregion

        #region Collection

        private string CollectionName = "friendrequests";

        protected MongoCollection<object> Collection
        {
            get { return database.GetCollection<object>(CollectionName); }
        }

        #endregion
    }
}