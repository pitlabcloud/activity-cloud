﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers
{

    public class UserController : BaseController
    {
        /// <summary>
        /// Get a complete list of users.
        /// </summary>
        /// <returns>Json representation of the list of users.</returns>
        [RequireUser]
        public List<JObject> Get()
        {
            List<JObject> result = UserStorage.Get();
            if(result.Count > 0) return UserStorage.Get();
            return null;
        }

        /// <summary>
        /// Get the user that matches the required user Id.
        /// </summary>
        /// <param name="userId">Guid representation of the user Id.</param>
        /// <returns>Json representation of the user.</returns>
        [RequireUser]
        public JObject Get(Guid userId)
        {
            User user = UserRegistry.Get(userId);
            if (user != null)
                return UserObject(user);
            return null;
        }

        /// <summary>
        /// Get the user that matches the required email.
        /// </summary>
        /// <param name="email">Email of the specific user.</param>
        /// <returns>Json representation of the user.</returns>
        public object Get(string email)
        {
            User user = UserRegistry.GetUserOnEmail(email);
            if (user != null)
                return UserObject(user);
            return null;
        }

        /// <summary>
        /// Create user in Activity Cloud
        /// </summary>
        /// <param name="user">Json representation of the user.</param>
        /// <returns>Returns true if user is added, false if user already exists.</returns>
        public bool Post(JObject user)
        {
            if (user != null && IsFormatOk(user))
            {
                if (!UserRegistry.ExistingEmail(user["Email"].ToString()))
                {
                    if (user["Id"] == null && !user["Id"].HasValues) user["Id"] = Guid.NewGuid().ToString();

                    User obj = JsonConvert.DeserializeObject<User>(user.ToString());
                    if (UserRegistry.Add(obj))
                    {
                        UserStorage.Add(obj.Id, user);
                        Notifier.NotifyAll(NotificationType.UserAdded, user);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Update user in Activity Cloud.
        /// </summary>
        /// <param name="userId">Guid representation of the user Id.</param>
        /// <param name="user">Json representation of the user.</param>
        /// <returns>Returns true if user is updated, false if not.</returns>
        [RequireUser]
        public bool Put(Guid userId, JObject user)
        {
            if (user != null && IsFormatOk(user))
            {
                if (UserRegistry.ExistingId(userId))
                {
                    UserStorage.Add(userId, user);
                    Notifier.NotifyAll(NotificationType.UserUpdated, user);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Delete user in Activity Cloud.
        /// </summary>
        /// <param name="userId">Guid representation of the user Id.</param>
        /// <returns>Returns true if user is deleted, false if not.</returns>
        [RequireUser]
        public bool Delete(Guid userId)
        {
            if (userId != null)
            {
                if (UserRegistry.Remove(userId))
                {
                    if (CurrentUser.Id == userId)
                        DeviceRegistry.DisconnectUser(userId);

                    var user = UserStorage.Get(userId);
                    UserStorage.Remove(userId);
                    Notifier.NotifyAll(NotificationType.UserDeleted, user);
                    return true;
                }
            }
            return false;
        }

        private JObject UserObject(User user)
        {
            JObject result = JObject.FromObject(user);
            JObject storage = UserStorage.Get(user.Id);
            foreach (JProperty node in storage.Properties())
            {
                if (result[node.Name] == null)
                    result.Add(node.Name, node.Value);
                else
                    result[node.Name] = node.Value;
            }
            return result;
        }

        private bool IsFormatOk(JObject obj)
        {
            if (obj["Email"] == null || obj["Name"] == null) return false;
            return true;
        }
    }
}