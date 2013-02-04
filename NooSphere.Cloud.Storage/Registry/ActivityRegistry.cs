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
    public class ActivityRegistry : BaseRegistry
    {
        #region Constructors

        public ActivityRegistry(string connectionString, string db) : base(connectionString, db)
        {
        }

        #endregion

        #region Public Methods

        public List<Activity> GetOnUser(Guid userId)
        {
            return Collection.FindAs<Activity>(Query.And(Query.EQ("Owner._id", userId), Query.EQ("IsHistory", false))).ToList();
        }

        public List<Activity> Get()
        {
            return base.Get(Collection).Cast<Activity>().ToList();
        }

        public Activity Get(Guid id)
        {
            return (Activity) base.Get(Collection, id);
        }

        public bool Add(object obj)
        {
            return base.Add(Collection, obj);
        }

        public bool Upsert(Guid id, object obj)
        {
            return base.Upsert(Collection, obj, id);
        }

        public bool Remove(Guid id)
        {
            return base.Remove(Collection, id);
        }

        #endregion

        #region Collection

        private string CollectionName = "activities";

        protected MongoCollection<object> Collection
        {
            get { return database.GetCollection<object>(CollectionName); }
        }

        #endregion
    }
}