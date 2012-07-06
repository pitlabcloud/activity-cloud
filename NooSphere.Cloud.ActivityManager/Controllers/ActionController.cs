using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers
{

    public class ActionController : BaseController
    {

        [RequireUser]
        [AcceptVerbs("POST")]
        public void Post(Guid activityId, JObject action)
        {
            if (action != null)
            {
                if (action["Id"] == null) action["Id"] = Guid.NewGuid().ToString();
                NooSphere.Core.ActivityModel.Action obj = JsonConvert.DeserializeObject<NooSphere.Core.ActivityModel.Action>(action.ToString());
                
                var activity = ActivityRegistry.Get(activityId);

                if (IsParticipant((Activity)activity))
                {
                    ((Activity)activity).Actions.Add(obj);

                    // Notify subscribers
                    Notifier.NotifyAll(NotificationType.ActionAdded, action);
                }
            }
        }

        [RequireUser]
        [AcceptVerbs("PUT")]
        public void Put(Guid activityId, JObject action)
        {
            if (action != null)
            {
                NooSphere.Core.ActivityModel.Action obj = JsonConvert.DeserializeObject<NooSphere.Core.ActivityModel.Action>(action.ToString());
                // Get activity
                var activity = ActivityRegistry.Get(activityId);

                if (IsParticipant((Activity)activity))
                {
                    var oldActivity = activity;
                    // Create new id and add ti activity storage
                    ((Activity)oldActivity).Id = Guid.NewGuid();
                    ActivityRegistry.Add(oldActivity);
                    // Add old activity to the new activity's history
                    ((Activity)activity).History.AddFirst(((Activity)oldActivity).Id);
                    // Add action to activity
                    ((Activity)activity).Actions.Add(obj);
                    // Add updated activity to the activity storage
                    ActivityRegistry.Add(activity);
                    // Notify subscribers
                    Notifier.NotifyAll(NotificationType.ActionUpdated, action);
                }
            }
        }

        [RequireUser]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid activityId, Guid actionId)
        {
            if (actionId != null)
            {
                var activity = ActivityRegistry.Get(activityId);
                if (IsOwner((Activity)activity))
                {
                    // Find correct action
                    NooSphere.Core.ActivityModel.Action action = ((Activity)activity).Actions.SingleOrDefault(act => act.Id == actionId);
                    // Remove action in activity
                    ((Activity)activity).Actions.Remove(action);
                    // Notify subscribers
                    Notifier.NotifyAll(NotificationType.ActionDeleted, action);
                }
            }
        }
    }
}
