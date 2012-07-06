using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.Data.Registry
{
    public class ActivityRegistry : BaseRegistry
    {
        public ActivityRegistry(string connectionString) : base(connectionString) { }

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