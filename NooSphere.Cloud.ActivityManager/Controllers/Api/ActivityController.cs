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
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class ActivityController : BaseController
    {
        #region Private Members
        private ActivityRegistry ActivityRegistry = new ActivityRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        private ActivityStorage ActivityStorage = new ActivityStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        #endregion

        #region Exposed API Methods
        /// <summary>
        /// Get a complete list of activities.
        /// </summary>
        /// <returns>Json representation of the list of activities.</returns>
        [RequireUser]
        public List<JObject> Get()
        {
            List<Activity> activities = ActivityRegistry.GetOnUser(CurrentUser.Id);
            foreach (Activity activity in activities)
            {
                Notifier.Subscribe(ConnectionId, activity.Id);
                if (activity.Actions != null && activity.Actions.Count > 0)
                    foreach (Resource resource in activity.Actions.SelectMany(action => action.Resources))
                        Notifier.NotifyGroup(CurrentUserId, NotificationType.FileDownload, resource);
            }
            return ReturnObject(activities);
        }

        /// <summary>
        /// Get the activity that matches the required activity Id.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        /// <returns>Json representation of the activity.</returns>
        [RequireUser]
        public JObject Get(Guid activityId)
        {
            Activity activity = ActivityRegistry.Get(activityId);
            if (IsParticipant(activity))
            {
                Notifier.Subscribe(ConnectionId, activity.Id);
                return ReturnObject(activity);
            }
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
                if (data["Id"] == null && !data["Id"].HasValues)
                    data["Id"] = Guid.NewGuid().ToString();

                Activity activity = JsonConvert.DeserializeObject<Activity>(data.ToString());
                if (activity.Participants == null || activity.Participants.Count == 0)
                {
                    data["Owner"] = JToken.FromObject(CurrentUser);
                    data["Participants"] = JToken.FromObject(new List<User>() { CurrentUser });
                }

                Notifier.Subscribe(CurrentUserId, activity.Id);
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

                    UpdateActivity(NotificationType.ActivityUpdated, data);
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
        #endregion

        #region Public Methods
        [NonAction]
        public JObject GetExtendedActivity(Guid activityId)
        {
            return ActivityStorage.Get(activityId);
        }

        [NonAction]
        public Activity GetActivity(Guid activityId)
        {
            return ActivityRegistry.Get(activityId);
        }

        [NonAction]
        public bool AddActivity(NotificationType type, JObject data, bool asHistory = false)
        {
            Activity activity = data.ToObject<Activity>();
            if (asHistory) activity.IsHistory = asHistory;
            if (ActivityRegistry.Add(activity))
            {
                ActivityStorage.Add(data.ToObject<Activity>().Id, data);
                if (!asHistory) Notifier.Subscribe(ConnectionId, activity.Id);
                Notifier.NotifyGroup(activity.Id, type, data);
                foreach (Resource resource in data.ToObject<Activity>().Actions.SelectMany(action => action.Resources))
                    Notifier.NotifyGroup(CurrentUserId, NotificationType.FileUpload, resource);
                return true;
            }
            return false;
        }

        [NonAction]
        public bool UpdateActivity(NotificationType type, JObject data)
        {
            Activity activity = data.ToObject<Activity>();
            if (ActivityRegistry.Upsert(activity.Id, activity))
            {
                ActivityStorage.Add(data.ToObject<Activity>().Id, data);
                Notifier.NotifyGroup(activity.Id, type, data);
                return true;
            }
            return false;
        }

        [NonAction]
        public bool RemoveActivity(NotificationType type, Guid activityId)
        {
            Activity activity = ActivityRegistry.Get(activityId);
            if (ActivityRegistry.Remove(activityId))
            {
                ActivityStorage.Remove(activityId);
                foreach (Resource resource in activity.Actions.SelectMany(action => action.Resources))
                    Notifier.NotifyGroup(CurrentUserId, NotificationType.FileDelete, resource);
                Notifier.NotifyAll(NotificationType.ActivityDeleted, new { Id = activityId });
                return true;
            }
            return false;
        }
        #endregion

        #region Private Methods
        private List<JObject> ReturnObject(List<Activity> activities)
        {
            List<JObject> result = new List<JObject>();
            foreach(Activity activity in activities)
                result.Add(ReturnObject(activity));

            return result;
        }
        private JObject ReturnObject(Activity activity)
        {
            JObject result = JObject.FromObject(activity);
            JObject storage = ActivityStorage.Get(activity.Id);
            foreach (JProperty node in storage.Properties())
            {
                if (result[node.Name] == null)
                    result.Add(node.Name, node.Value);
                else
                    result[node.Name] = node.Value;
            }

            result.Remove("History");
            result.Remove("IsHistory");

            return result;
        }
        #endregion
    }
}
