using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.Data.Registry
{
    public class ActionRegistry : BaseRegistry
    {
        public ActionRegistry(string connectionString) : base(connectionString) { }

        #region MongoDbStorage method pointers
        public List<NooSphere.Core.ActivityModel.Action> Get()
        {
            return base.Get(Collection).Cast<NooSphere.Core.ActivityModel.Action>().ToList();
        }

        public NooSphere.Core.ActivityModel.Action Get(Guid id)
        {
            return (NooSphere.Core.ActivityModel.Action)base.Get(Collection, id);
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
        private string CollectionName = "actions";
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