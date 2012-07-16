using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.Data.Registry
{
    public class ActivityRegistry : BaseRegistry
    {
        public ActivityRegistry(string connectionString) : base(connectionString) { }

        public List<Activity> GetOnUser(Guid userId)
        {
            return Collection.FindAs<Activity>(Query.And(Query.EQ("Owner._id", userId),Query.EQ("IsHistory", false))).ToList();
        }

        #region MongoDbStorage method pointers
        public List<Activity> Get()
        {
            return base.Get(Collection).Cast<Activity>().ToList();
        }

        public Activity Get(Guid id)
        {
            return (Activity)base.Get(Collection, id);
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
            get
            {
                return database.GetCollection<object>(CollectionName);
            }
        }
        #endregion
    }
}