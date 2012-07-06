using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NooSphere.Core.ActivityModel;
using System;

namespace NooSphere.Cloud.Data.Registry
{
    public class DeviceRegistry : BaseRegistry
    {
        public DeviceRegistry(string connectionString) : base(connectionString) { }

        public Guid GetUserId(Guid connectionId)
        {
            List<Device> devices = Collection.FindAs<Device>(Query.EQ("ConnectionId", connectionId)).SetFields("UserId").ToList();
            if (devices.Count == 1) return devices[0].UserId;
            else return Guid.Empty;
        }

        public bool ConnectUser(Guid connectionId, Guid userId)
        {
            return Collection.Update(Query.EQ("ConnectionId", connectionId), Update.Set("UserId", userId), SafeMode.True).Ok;
        }

        public bool DisconnectUser(Guid userId)
        {
            return Collection.Update(Query.EQ("UserId", userId), Update.Unset("UserId"), SafeMode.True).Ok;
        }

        public bool IsUserConnected(Guid connectionId)
        {
            return Collection.FindAs<Device>(Query.And(Query.EQ("ConnectionId", connectionId), Query.Exists("UserId", true))).Count() > 0;
        }

        public bool RemoveOnConnectionId(Guid connectionId)
        {
            return Collection.Remove(Query.EQ("ConnectionId", connectionId), SafeMode.True).Ok;
        }

        #region MongoDbStorage method pointers
        public List<Device> Get()
        {
            return base.Get(Collection).Cast<Device>().ToList();
        }

        public Device Get(Guid id)
        {
            return (Device)base.Get(Collection, id);
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
        private string CollectionName = "devices";
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