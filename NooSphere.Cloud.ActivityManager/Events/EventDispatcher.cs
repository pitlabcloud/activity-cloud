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
using System.Configuration;
using System.Threading.Tasks;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Core.ActivityModel;
using Microsoft.AspNet.SignalR;

#endregion

namespace NooSphere.Cloud.ActivityManager.Events
{
    /// <summary>
    /// </summary>
    public class EventDispatcher : PersistentConnection
    {
        private readonly DeviceRegistry DeviceRegistry =
            new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"], ConfigurationManager.AppSettings["MongoDb"]);
        
        /// <summary>
        ///   Called when a new connection is made.
        /// </summary>
        /// <param name="request"> The request associated with this connection. </param>
        /// <param name="connectionId"> The id of the new connection. </param>
        /// <returns> </returns>
        protected override Task OnConnected(IRequest request, string connectionId)
        {
            DeviceRegistry.Add(new Device {Id = Guid.NewGuid(), ConnectionId = new Guid(connectionId)});
            return Connection.Send("null", "Connected");
        }

        /// <summary>
        ///   Called when a connection is restored.
        /// </summary>
        /// <param name="request"> The request associated with this connection. </param>
        /// <param name="connectionId"> The id of the new connection. </param>
        /// <returns> </returns>
        protected override Task OnReconnected(IRequest request, string connectionId)
        {
            return Connection.Send("null", "Reconnected");
        }

        /// <summary>
        ///   Called when data is received on the connection.
        /// </summary>
        /// <param name="request"> The request associated with the connection that sent the data. </param>
        /// <param name="connectionId"> The id of the connection that sent the data. </param>
        /// <param name="data"> The payload received from the client. </param>
        /// <returns> </returns>
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            return Connection.Broadcast(Notifier.ConstructEvent(NotificationType.Message, data));
        }

        /// <summary>
        ///   Called when a connection goes away (client is no longer connected, e.g. browser close).
        /// </summary>
        /// <param name="request"> The request associated with the connection that sent the data. </param>
        /// <param name="connectionId"> The id of the client that disconnected. </param>
        /// <returns> </returns>
        protected override Task OnDisconnected(IRequest request, string connectionId)
        {
            DeviceRegistry.RemoveOnConnectionId(new Guid(connectionId));
            return Connection.Send("null", "Disconnected");
        }
    }
}