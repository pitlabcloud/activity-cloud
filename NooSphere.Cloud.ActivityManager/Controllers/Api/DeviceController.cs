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
using System.Configuration;
using System.Web.Http;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class DeviceController : BaseController
    {
        #region Exposed API Methods
        /// <summary>
        /// Register the device and pair it with the specified user.
        /// </summary>
        /// <param name="userId">Guid representation of a userId</param>
        /// <returns>Returns true if device was registered to the user, false if user doesn't exist or device is already registered.</returns>
        public bool Post(Guid userId)
        {
            if (CurrentUser != null) return false;
            if (ConnectionId == Guid.Empty) return false;
            if (userId != null && UserRegistry.ExistingId(userId))
            {
                if (DeviceRegistry.ConnectUser(ConnectionId, userId))
                {
                    Notifier.Subscribe(ConnectionId, userId);
                    foreach (User friend in CurrentUser.Friends)
                        Notifier.Subscribe(ConnectionId, friend.Id);

                    if (DeviceRegistry.ConnectedDevices(CurrentUserId).Count == 1)
                        Notifier.NotifyGroup(CurrentUserId, NotificationType.UserConnected, userId);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Unregister the device and remove the pairing with the specified user.
        /// </summary>
        /// <param name="userId">Guid representation of a userId.</param>
        /// <returns>Returns true if device was unregistered, false if device is not registered to user.</returns>
        [RequireUser]
        public bool Delete(Guid userId)
        {
            if (CurrentUser == null) return false;
            if (userId != null && CurrentUser.Id == userId && DeviceRegistry.DisconnectUser(userId))
            {
                Notifier.Unsubscribe(ConnectionId, userId);
                if (DeviceRegistry.ConnectedDevices(CurrentUserId).Count == 0)
                    Notifier.NotifyGroup(CurrentUserId, NotificationType.UserDisconnected, userId);
                return true;
            }
            return false;
        }
        #endregion

        #region Public Methods
        [NonAction]
        public void Clear()
        {
            foreach (Device device in DeviceRegistry.Get())
            {
                DeviceRegistry.Remove(device.Id);
            }
        }
        #endregion
    }
}
