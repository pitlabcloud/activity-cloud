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
    public class DeviceRegistry : BaseRegistry
    {
        #region Constructors

        public DeviceRegistry(string connectionString, string db) : base(connectionString, db)
        {
        }

        #endregion

        #region Public Methods

        public Guid GetUserId(Guid connectionId)
        {
            List<Device> devices =
                Collection.FindAs<Device>(Query.EQ("ConnectionId", connectionId)).SetFields("UserId").ToList();
            if (devices.Count == 1) return devices[0].UserId;
            else return Guid.Empty;
        }

        public bool ConnectUser(Guid connectionId, Guid userId)
        {
            return
                Collection.Update(Query.EQ("ConnectionId", connectionId), Update.Set("UserId", userId), WriteConcern.Acknowledged).
                    Ok;
        }

        public bool DisconnectUser(Guid userId)
        {
            return Collection.Update(Query.EQ("UserId", userId), Update.Unset("UserId"), WriteConcern.Acknowledged).Ok;
        }

        public List<Device> ConnectedDevices(Guid userId)
        {
            return Collection.FindAs<Device>(Query.EQ("UserId", userId)).ToList();
        }

        public List<Guid> ConnectionIds(Guid userId)
        {
            return Collection.FindAs<Device>(Query.EQ("UserId", userId)).Select(d => d.ConnectionId).ToList();
        }

        public bool IsUserConnected(Guid connectionId)
        {
            MongoCursor<Device> device =
                Collection.FindAs<Device>(Query.And(Query.EQ("ConnectionId", connectionId), Query.Exists("UserId")));
            if (device.Count() == 0) return false;
            return device.First().UserId != Guid.Empty;
        }

        public bool RemoveOnConnectionId(Guid connectionId)
        {
            return Collection.Remove(Query.EQ("ConnectionId", connectionId), WriteConcern.Acknowledged).Ok;
        }

        public List<Device> Get()
        {
            return base.Get(Collection).Cast<Device>().ToList();
        }

        public Device Get(Guid id)
        {
            return (Device) base.Get(Collection, id);
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
            get { return database.GetCollection<object>(CollectionName); }
        }

        #endregion
    }
}