using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class DeviceController : BaseController
    {
        /// <summary>
        /// Register the device and pair it with the specified user.
        /// </summary>
        /// <param name="userId">Guid representation of a userId</param>
        /// <returns>Returns true if device was registered to the user, false if user doesn't exist or device is already registered.</returns>
        public bool Post(Guid userId)
        {
            if (CurrentUser != null) return false;
            if (userId != null && UserRegistry.ExistingId(userId))
            {
                if (DeviceRegistry.ConnectUser(ConnectionId, userId))
                {
                    Notifier.Subscribe(ConnectionId, userId);
                    Notifier.NotifyAll(NotificationType.UserConnected, UserStorage.Get(userId));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Unregister the device and remove the pairing with the specified user.
        /// </summary>
        /// <param name="userId">Json representation of a userId.</param>
        /// <returns>Returns true if device was unregistered, false if device is not registered to user.</returns>
        [RequireUser]
        public bool Delete(Guid userId)
        {
            if (CurrentUser == null) return false;
            if (userId != null && CurrentUser.Id == userId && DeviceRegistry.DisconnectUser(userId))
            {
                Notifier.Unsubscribe(ConnectionId, userId);
                Notifier.NotifyAll(NotificationType.UserDisconnected, UserStorage.Get(userId));
                return true;
            }
            return false;
        }
    }
}
