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
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class UserStatusController : BaseController
    {
        #region Private Members
        private UserController UserController = new UserController();
        #endregion

        #region Exposed API Methods
        /// <summary>
        /// Update status of the specific user.
        /// </summary>
        /// <param name="userId">Guid representation of the user Id.</param>
        /// <param name="status">String representation of the status.</param>
        /// <returns>Returns true if status was updated, false if not.</returns>
        [RequireUser]
        public bool Post(Guid userId, string status)
        {
            if (userId == CurrentUserId)
            {
                JObject user = UserController.GetExtendedUser(userId);
                user["Status"] = status;

                if(UserController.UpdateUser(user))
                {
                    Notifier.NotifyGroup(userId, NotificationType.UserStatusChanged, new { UserId = userId, Status = status });
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
