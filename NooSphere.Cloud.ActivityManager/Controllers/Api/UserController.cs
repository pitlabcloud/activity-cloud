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
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UserController : BaseController
    {
        #region Private Members
        private UserStorage UserStorage = new UserStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        #endregion

        #region Exposed API Methods
        /// <summary>
        /// Get a complete list of users.
        /// </summary>
        /// <returns>Json representation of the list of users.</returns>
        [RequireUser]
        public List<JObject> Get()
        {
            List<User> users = UserRegistry.Get();
            return ReturnObject(users);
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
                return ReturnObject(user);
            return null;
        }

        /// <summary>
        /// Get the user that matches the required email.
        /// </summary>
        /// <param name="email">Email of the specific user.</param>
        /// <returns>Json representation of the user.</returns>
        public JObject Get(string email)
        {
            User user = UserRegistry.GetUserOnEmail(email);
            if (user != null)
                return ReturnObject(user);
            return null;
        }

        /// <summary>
        /// Create user in Activity Cloud
        /// </summary>
        /// <param name="data">Json representation of the user.</param>
        /// <returns>Returns true if user is added, false if user already exists.</returns>
        public bool Post(JObject data)
        {
            if (data != null && IsFormatOk(data))
            {
                if (!UserRegistry.ExistingEmail(data["Email"].ToString()))
                {
                    if (data["Id"] == null && !data["Id"].HasValues)
                        data["Id"] = Guid.NewGuid().ToString();

                    return AddUser(data);
                }
            }
            return false;
        }

        /// <summary>
        /// Update user in Activity Cloud.
        /// </summary>
        /// <param name="userId">Guid representation of the user Id.</param>
        /// <param name="data">Json representation of the user.</param>
        /// <returns>Returns true if user is updated, false if not.</returns>
        [RequireUser]
        public bool Put(Guid userId, JObject data)
        {
            if (data != null && IsFormatOk(data))
                return AddUser(data);

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
                return RemoveUser(userId);
            }
            return false;
        }
        #endregion

        #region Public Methods
        [NonAction]
        public List<JObject> GetExtendedUsers()
        {
            return UserStorage.Get();
        }

        [NonAction]
        public JObject GetExtendedUser(Guid userId)
        {
            return UserStorage.Get(userId);
        }

        [NonAction]
        public void Clear()
        {
            foreach (User user in UserRegistry.Get())
            {
                RemoveUser(user.Id);
            }
        }

        [NonAction]
        public User GetUser(Guid userId)
        {
            return UserRegistry.Get(userId);
        }

        [NonAction]
        public bool AddUser(JObject data)
        {
            User user = data.ToObject<User>();
            if (UserRegistry.Add(user))
            {
                UserStorage.Add(user.Id, data);
                return true;
            }
            return false;
        }

        [NonAction]
        public bool UpdateUser(JObject data)
        {
            User user = data.ToObject<User>();
            if (UserRegistry.Upsert(user.Id, user))
            {
                UserStorage.Add(user.Id, data);
                return true;
            }
            return false;
        }

        [NonAction]
        public bool RemoveUser(Guid userId)
        {
            if (UserRegistry.Remove(userId))
            {
                if (CurrentUser != null && CurrentUser.Id == userId)
                    DeviceRegistry.DisconnectUser(userId);

                UserStorage.Remove(userId);
                return true;
            }
            return false;
        }

        [NonAction]
        public List<JObject> ReturnObject(List<User> users)
        {
            List<JObject> result = new List<JObject>();
            foreach (User user in users)
                result.Add(ReturnObject(user));

            return result;
        }

        [NonAction]
        public JObject ReturnObject(User user)
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

            if (user.Id != CurrentUserId)
                result.Remove("Friends");

            return result;
        }

        private bool IsFormatOk(JObject obj)
        {
            if (obj["Email"] == null || obj["Name"] == null) return false;
            return true;
        }
        #endregion
    }
}