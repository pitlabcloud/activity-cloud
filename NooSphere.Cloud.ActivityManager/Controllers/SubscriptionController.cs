using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class SubscriptionController : BaseController
    {
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
    }
}
