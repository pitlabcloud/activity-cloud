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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

#endregion

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class ActivityController : BaseController
    {
        private readonly ActivityRegistry _activityRegistry =
            new ActivityRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);

        private readonly ActivityStorage _activityStorage =
            new ActivityStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"],
                                ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);

        private readonly FileController _fileController = new FileController();

        #region Exposed API Methods

        /// <summary>
        ///   Get a complete list of activities.
        /// </summary>
        /// <returns> Json representation of the list of activities. </returns>
        [RequireUser]
        public HttpResponseMessage Get()
        {
            var activities = GetExtendedActivities(CurrentUserId);
            var response = new HttpResponseMessage
                               {
                                   Content =
                                       new ObjectContent(activities.GetType(), activities, new JsonMediaTypeFormatter())
                               };
            return response;
        }

        /// <summary>
        ///   Get the activity that matches the required activity Id.
        /// </summary>
        /// <param name="activityId"> Guid representation of the activity Id. </param>
        /// <returns> Json representation of the activity. </returns>
        [RequireUser]
        public HttpResponseMessage Get(Guid activityId)
        {
            var activity = _activityRegistry.Get(activityId);
            var response = new HttpResponseMessage();
            if (!IsParticipant(activity))
            {
                response.StatusCode = HttpStatusCode.Forbidden;
            }
            else
            {
                Notifier.Subscribe(ConnectionId, activity.Id);
                response.StatusCode = HttpStatusCode.OK;
                var result = ReturnObject(activity);
                response.Content = new ObjectContent(result.GetType(), result, new JsonMediaTypeFormatter());
            }
            return response;
        }

        /// <summary>
        ///   Create an activity in Activity Cloud.
        /// </summary>
        /// <param name="data"> Json representation of the activity. </param>
        [RequireUser]
        public void Post(JObject data)
        {
            if (data != null)
            {
                if (data["Id"] == null || !data["Id"].HasValues)
                    data["Id"] = Guid.NewGuid().ToString();

                var activity = JsonConvert.DeserializeObject<Activity>(data.ToString());
                if (activity.Participants == null || activity.Participants.Count == 0)
                {
                    data["Owner"] = JToken.FromObject(CurrentUser);
                    data["Participants"] = JToken.FromObject(new List<User> {CurrentUser});
                }

                Notifier.Subscribe(CurrentUserId, activity.Id);
                AddActivity(NotificationType.ActivityAdded, data);
            }
        }

        /// <summary>
        ///   Update activity in Activity Cloud.
        /// </summary>
        /// <param name="activityId"> Guid representation of the activity Id. </param>
        /// <param name="data"> Json representation of the activity. </param>
        [RequireUser]
        public void Put(Guid activityId, JObject data)
        {
            if (data != null)
            {
                if (IsOwner(data.ToObject<Activity>()) || IsParticipant(data.ToObject<Activity>()))
                {
                    // Return if no changes has been made
                    var oldActivity = _activityStorage.Get(activityId);
                    var compare = JObject.Parse(oldActivity.DeepClone().ToString());
                    compare.Remove("History");
                    if (compare.ToString().Equals(data.ToString())) return;

                    oldActivity["Id"] = Guid.NewGuid().ToString();
                    AddActivity(NotificationType.None, oldActivity, true);

                    if (data["History"] == null)
                        data["History"] = JToken.FromObject(new List<Guid> {new Guid(oldActivity["Id"].ToString())});
                    else
                        data["History"].AddAfterSelf(oldActivity);

                    UpdateActivity(NotificationType.ActivityUpdated, data);
                }
            }
        }

        /// <summary>
        ///   Delete activity from Activity Cloud.
        /// </summary>
        /// <param name="activityId"> Guid representation of the activity Id. </param>
        [RequireUser]
        public void Delete(Guid activityId)
        {
            var activity = _activityRegistry.Get(activityId);
            if (activity != null && IsOwner(activity))
                RemoveActivity(NotificationType.ActivityDeleted, activityId);
        }

        #endregion

        #region Public Methods

        [NonAction]
        public List<JObject> GetExtendedActivities(Guid userId)
        {
            var activities = _activityRegistry.GetOnUser(userId);
            foreach (var activity in activities)
            {
                Notifier.Subscribe(ConnectionId, activity.Id);
                if (activity.Actions != null && activity.Actions.Count > 0)
                    foreach (var resource in activity.Actions.SelectMany(action => action.Resources))
                        Notifier.NotifyGroup(CurrentUserId, NotificationType.FileDownload, resource);
            }
            return ReturnObject(activities);
        }

        [NonAction]
        public JObject GetExtendedActivity(Guid activityId)
        {
            return _activityStorage.Get(activityId);
        }

        [NonAction]
        public Activity GetActivity(Guid activityId)
        {
            return _activityRegistry.Get(activityId);
        }

        [NonAction]
        public void Clear()
        {
            foreach (var activity in _activityRegistry.Get())
            {
                RemoveActivity(NotificationType.ActivityDeleted, activity.Id);
            }
        }

        [NonAction]
        public bool AddActivity(NotificationType type, JObject data, bool asHistory = false)
        {
            var activity = data.ToObject<Activity>();
            if (asHistory) activity.IsHistory = true;
            if (_activityRegistry.Add(activity))
            {
                _activityStorage.Add(data.ToObject<Activity>().Id, data);
                if (!asHistory) Notifier.Subscribe(ConnectionId, activity.Id);
                Notifier.NotifyGroup(activity.Id, type, data);
                if (!asHistory) _fileController.Sync(activity, SyncType.Added);
                return true;
            }
            return false;
        }

        [NonAction]
        public bool UpdateActivity(NotificationType type, JObject data)
        {
            var activity = data.ToObject<Activity>();
            if (_activityRegistry.Upsert(activity.Id, activity))
            {
                _activityStorage.Add(data.ToObject<Activity>().Id, data);
                Notifier.NotifyGroup(activity.Id, type, data);
                _fileController.Sync(activity, SyncType.Updated);
                return true;
            }
            return false;
        }

        [NonAction]
        public bool RemoveActivity(NotificationType type, Guid activityId)
        {
            var activity = _activityRegistry.Get(activityId);
            if (_activityRegistry.Remove(activityId))
            {
                _activityStorage.Remove(activityId);
                if (CurrentUserId != Guid.Empty)
                {
                    foreach (var resource in activity.Actions.SelectMany(action => action.Resources))
                        Notifier.NotifyGroup(CurrentUserId, NotificationType.FileDelete, resource);
                    Notifier.NotifyAll(NotificationType.ActivityDeleted, new {Id = activityId});
                }
                _fileController.Sync(activity, SyncType.Removed);
                return true;
            }
            return false;
        }

        #endregion

        #region Private Methods

        private List<JObject> ReturnObject(IEnumerable<Activity> activities)
        {
            return activities.Select(ReturnObject).ToList();
        }

        private JObject ReturnObject(Activity activity)
        {
            var result = JObject.FromObject(activity);
            var storage = _activityStorage.Get(activity.Id);
            foreach (var node in storage.Properties())
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