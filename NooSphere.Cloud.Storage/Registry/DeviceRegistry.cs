/// <licence>
/// 
/// (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)
/// 
/// Pervasive Interaction Technology Laboratory (pIT lab)
/// IT University of Copenhagen
///
/// This library is free software; you can redistribute it and/or 
/// modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
/// as published by the Free Software Foundation. Check 
/// http://www.gnu.org/licenses/gpl.html for details.
/// 
/// </licence>

using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.Data.Registry
{
    public class DeviceRegistry : BaseRegistry
    {
        #region Constructors
        public DeviceRegistry(string connectionString) : base(connectionString) { }
        #endregion

        #region Public Methods
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

        public int ConnectedDevices(Guid userId)
        {
            return (int)Collection.FindAs<Device>(Query.EQ("UserId", userId)).Count();
        }

        public bool IsUserConnected(Guid connectionId)
        {
            var device = Collection.FindAs<Device>(Query.And(Query.EQ("ConnectionId", connectionId), Query.Exists("UserId", true)));
            if (device.Count() == 0) return false;
            return device.First().UserId != Guid.Empty;
        }

        public bool RemoveOnConnectionId(Guid connectionId)
        {
            return Collection.Remove(Query.EQ("ConnectionId", connectionId), SafeMode.True).Ok;
        }

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