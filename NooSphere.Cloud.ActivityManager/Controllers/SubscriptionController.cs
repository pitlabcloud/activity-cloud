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
using System.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class SubscriptionController : BaseController
    {
        #region Exposed API Methods
        /// <summary>
        /// Subscribe to activity matching the specified activity Id.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        [RequireUser]
        public void Post(Guid activityId)
        {
            Activity activity = ActivityRegistry.Get(activityId);
            if (activity != null && IsParticipant(activity))
            {
                Notifier.Subscribe(ConnectionId, activityId.ToString());
                foreach (Resource resource in activity.Actions.SelectMany(action => action.Resources))
                    Notifier.NotifyGroup(activityId.ToString(), NotificationType.FileDownload, resource);
            }
        }

        /// <summary>
        /// Unsubscribe from activity matching the specified activity Id.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        [RequireUser]
        public void Delete(Guid activityId)
        {
            Activity activity = ActivityRegistry.Get(activityId);
            if (activity != null && IsParticipant(activity))
                Notifier.Unsubscribe(ConnectionId, activityId.ToString());
        }
        #endregion
    }
}
