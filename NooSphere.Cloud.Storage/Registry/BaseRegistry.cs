using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.Data.Registry
{
    public class BaseRegistry
    {
        protected MongoDatabase database;

        public BaseRegistry(string connectionString)
        {
            database = MongoDatabase.Create(connectionString);
        }

        protected List<object> Get(MongoCollection<object> collection)
        {
            return collection.FindAllAs<object>().ToList();
        }

        protected object Get(MongoCollection<object> collection, Guid id)
        {
            return collection.FindOne(Query.EQ("_id", id));
        }

        protected bool Add(MongoCollection<object> collection, object obj)
        {
            return collection.Insert(obj, SafeMode.True).Ok;
        }

        protected bool Remove(MongoCollection<object> collection, Guid id)
        {
            return collection.Remove(Query.EQ("_id", id), SafeMode.True).Ok;
        }
    }
}
