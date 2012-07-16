using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Web;
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
    public class ActivityController : BaseController
    {
        /// <summary>
        /// Get a complete list of activities.
        /// </summary>
        /// <returns>Json representation of the list of activities.</returns>
        [RequireUser]
        public List<JObject> Get()
        {
            List<JObject> result = new List<JObject>();
            List<Activity> activities = ActivityRegistry.GetOnUser(CurrentUser.Id);
            foreach (Activity activity in activities)
            {
                result.Add(ActivityStorage.Get(activity.Id));
                if (activity.Actions != null && activity.Actions.Count > 0)
                    foreach (Resource resource in activity.Actions.SelectMany(action => action.Resources))
                        Notifier.NotifyGroup(CurrentUserId, NotificationType.FileDownload, resource);
            }
            return result;
        }

        /// <summary>
        /// Get the activity that matches the required activity Id.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        /// <returns>Json representation of the activity.</returns>
        [RequireUser]
        public object Get(Guid activityId)
        {
            Activity activity = ActivityRegistry.Get(activityId);
            if(IsParticipant(activity))
                return activity;
            else
                return null;
        }

        /// <summary>
        /// Create an activity in Activity Cloud.
        /// </summary>
        /// <param name="data">Json representation of the activity.</param>
        [RequireUser]
        public void Post(JObject data)
        {
            if (data != null)
            {
                if (data["Id"] == null && data["Id"].HasValues) data["Id"] = Guid.NewGuid().ToString();

                var activity = JsonConvert.DeserializeObject<Activity>(data.ToString());
                if (activity.Participants == null || activity.Participants.Count == 0)
                {
                    data["Owner"] = JToken.FromObject(CurrentUser);
                    data["Participants"] = JToken.FromObject(new List<User>() { CurrentUser });
                }

                AddActivity(NotificationType.ActivityAdded, data);
            }
        }

        /// <summary>
        /// Update activity in Activity Cloud.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        /// <param name="data">Json representation of the activity.</param>
        [RequireUser]
        public void Put(Guid activityId, JObject data)
        {
            if (data != null)
            {
                if (IsOwner(data.ToObject<Activity>()) || IsParticipant(data.ToObject<Activity>()))
                {
                    // Return if no changes has been made
                    JObject oldActivity = ActivityStorage.Get(activityId);
                    JObject compare = JObject.Parse(oldActivity.DeepClone().ToString());
                    compare.Remove("History");
                    if (compare.ToString().Equals(data.ToString())) return;
                    
                    oldActivity["Id"] = Guid.NewGuid().ToString();
                    AddActivity(NotificationType.None, oldActivity, true);

                    if (data["History"] == null)
                        data["History"] = JToken.FromObject(new List<Guid>() { new Guid(oldActivity["Id"].ToString()) });
                    else
                        data["History"].AddAfterSelf(oldActivity);

                    AddActivity(NotificationType.ActivityUpdated, data);
                }
            }
        }

        /// <summary>
        /// Delete activity from Activity Cloud.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        [RequireUser]
        public void Delete(Guid activityId)
        {
            if (activityId != null)
            {
                if (IsOwner(ActivityRegistry.Get(activityId)))
                    RemoveActivity(NotificationType.ActivityDeleted, activityId);
            }
        }

        private bool AddActivity(NotificationType type, JObject data, bool asHistory = false)
        {
            Activity activity = data.ToObject<Activity>();
            if (asHistory) activity.IsHistory = asHistory;
            if (ActivityRegistry.Upsert(activity.Id, activity))
            {
                ActivityStorage.Add(data.ToObject<Activity>().Id, data);
                Notifier.NotifyAll(type, data);
                foreach (Resource resource in data.ToObject<Activity>().Actions.SelectMany(action => action.Resources))
                    Notifier.NotifyGroup(CurrentUserId, NotificationType.FileUpload, resource);
                return true;
            }
            return false;
        }

        private bool RemoveActivity(NotificationType type, Guid activityId)
        {
            if (ActivityRegistry.Remove(activityId))
            {
                ActivityStorage.Remove(activityId);
                Notifier.NotifyAll(NotificationType.ActivityDeleted, new { Id = activityId });
                return true;
            }
            return false;
        }
    }
}
