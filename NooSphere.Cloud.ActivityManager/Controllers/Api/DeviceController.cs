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
using System.Web.Http;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Core.ActivityModel;

#endregion

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class DeviceController : BaseController
    {
        private ActivityController _activityController;
        private FriendController _friendController;

        #region Exposed API Methods

        /// <summary>
        ///   Register the device and pair it with the specified user.
        /// </summary>
        /// <param name="userId"> Guid representation of a userId </param>
        /// <returns> Returns true if device was registered to the user, false if user doesn't exist or device is already registered. </returns>
        public bool Post(Guid userId)
        {
            if (CurrentUser != null) return false;
            if (ConnectionId == Guid.Empty) return false;
            if (UserRegistry.ExistingId(userId))
            {
                if (DeviceRegistry.ConnectUser(ConnectionId, userId))
                {
                    _activityController = new ActivityController();
                    _friendController = new FriendController();

                    // Subscribe to user
                    Notifier.Subscribe(ConnectionId, userId);

                    // Subscribe to friends
                    foreach (var friend in UserRegistry.Get(userId).Friends)
                        Notifier.Subscribe(ConnectionId, friend.Id);

                    // Subscribe to activities and push to client
                    foreach (var activity in _activityController.GetExtendedActivities(userId))
                    {
                        Notifier.Subscribe(ConnectionId, Guid.Parse(activity["Id"].ToString()));
                        Notifier.NotifyGroup(ConnectionId, NotificationType.ActivityAdded, activity);
                    }

                    // Push pending friend requests
                    foreach (var fr in _friendController.GetFriendRequests(userId))
                        Notifier.NotifyGroup(ConnectionId, NotificationType.FriendRequest,
                                             new UserController().GetExtendedUser(fr.UserId));

                    // Push connected device to user
                    if (DeviceRegistry.ConnectedDevices(CurrentUserId).Count == 1)
                        Notifier.NotifyGroup(CurrentUserId, NotificationType.UserConnected, userId);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///   Unregister the device and remove the pairing with the specified user.
        /// </summary>
        /// <param name="userId"> Guid representation of a userId. </param>
        /// <returns> Returns true if device was unregistered, false if device is not registered to user. </returns>
        [RequireUser]
        public bool Delete(Guid userId)
        {
            if (CurrentUser == null) return false;
            if (CurrentUser.Id == userId && DeviceRegistry.DisconnectUser(userId))
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
            foreach (var device in DeviceRegistry.Get())
            {
                DeviceRegistry.Remove(device.Id);
            }
        }

        #endregion
    }
}