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
using System.Configuration;
using System.Threading.Tasks;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Core.ActivityModel;
using SignalR;

namespace NooSphere.Cloud.ActivityManager.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class EventDispatcher : PersistentConnection
    {
        DeviceRegistry DeviceRegistry = new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        /// <summary>
        /// Called when a new connection is made.
        /// </summary>
        /// <param name="request">The request associated with this connection.</param>
        /// <param name="connectionId">The id of the new connection.</param>
        /// <returns></returns>
        protected override Task OnConnectedAsync(IRequest request, string connectionId)
        {
            DeviceRegistry.Add(new Device { Id = Guid.NewGuid(), ConnectionId = new Guid(connectionId) });
            return null;
            //return Connection.Broadcast(Notifier.ConstructEvent(NotificationType.DeviceConnected, new { ConnectionId = connectionId }));
        }

        /// <summary>
        /// Called when a connection is restored.
        /// </summary>
        /// <param name="request">The request associated with this connection.</param>
        /// <param name="groups">A list of groups the client is a part of.</param>
        /// <param name="connectionId">The id of the new connection.</param>
        /// <returns></returns>
        protected override Task OnReconnectedAsync(IRequest request, IEnumerable<string> groups, string connectionId)
        {
            return null;
            //return Connection.Broadcast(Notifier.ConstructEvent(NotificationType.DeviceReconnected, new { ConnectionId = connectionId }));
        }

        /// <summary>
        /// Called when data is received on the connection.
        /// </summary>
        /// <param name="request">The request associated with the connection that sent the data.</param>
        /// <param name="connectionId">The id of the connection that sent the data.</param>
        /// <param name="data">The payload received from the client.</param>
        /// <returns></returns>
        protected override Task OnReceivedAsync(IRequest request, string connectionId, string data)
        {
            return Connection.Broadcast(Notifier.ConstructEvent(NotificationType.Message, data));
        }

        /// <summary>
        /// Called when a connection goes away (client is no longer connected, e.g. browser close).
        /// </summary>
        /// <param name="connectionId">The id of the client that disconnected.</param>
        /// <returns></returns>
        protected override Task OnDisconnectAsync(string connectionId)
        {
            DeviceRegistry.RemoveOnConnectionId(new Guid(connectionId));
            return null;
            //return Connection.Broadcast(Notifier.ConstructEvent(NotificationType.DeviceDisconnected, new { ConnectionId = connectionId }));
        }

        /// <summary>
        /// Called when an error occurs on the connection.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        /// <returns></returns>
        protected override Task OnErrorAsync(Exception error)
        {
            return Connection.Broadcast("Error ocurred " + error);
        }
    }
}