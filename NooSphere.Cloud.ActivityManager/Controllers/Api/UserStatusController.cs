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
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;

#endregion

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class UserStatusController : BaseController
    {
        private readonly UserController UserController = new UserController();

        #region Exposed API Methods

        /// <summary>
        ///   Update status of the specific user.
        /// </summary>
        /// <param name="userId"> Guid representation of the user Id. </param>
        /// <param name="status"> String representation of the status. </param>
        /// <returns> Returns true if status was updated, false if not. </returns>
        [RequireUser]
        public bool Post(Guid userId, string status)
        {
            if (userId == CurrentUserId)
            {
                JObject user = UserController.GetExtendedUser(userId);
                user["Status"] = status;

                if (UserController.UpdateUser(user))
                {
                    Notifier.NotifyGroup(userId, NotificationType.UserStatusChanged,
                                         new {UserId = userId, Status = status});
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}