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
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Core.ActivityModel;
using Action = NooSphere.Core.ActivityModel.Action;

#endregion

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ActionController : BaseController
    {
        private readonly ActivityRegistry ActivityRegistry =
            new ActivityRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);

        #region Exposed API Methods

        [RequireUser]
        [AcceptVerbs("POST")]
        public void Post(Guid activityId, JObject action)
        {
            if (action != null)
            {
                if (action["Id"] == null && action["Id"].HasValues) action["Id"] = Guid.NewGuid().ToString();
                var obj = JsonConvert.DeserializeObject<Action>(action.ToString());

                Activity activity = ActivityRegistry.Get(activityId);

                if (IsParticipant(activity))
                {
                    (activity).Actions.Add(obj);

                    // Notify subscribers
                    //Notifier.NotifyAll(NotificationType.ActionAdded, action);
                }
            }
        }

        [RequireUser]
        [AcceptVerbs("PUT")]
        public void Put(Guid activityId, JObject action)
        {
            if (action != null)
            {
                var obj = JsonConvert.DeserializeObject<Action>(action.ToString());
                // Get activity
                Activity activity = ActivityRegistry.Get(activityId);

                if (IsParticipant(activity))
                {
                    Activity oldActivity = activity;
                    // Create new id and add ti activity storage
                    oldActivity.Id = Guid.NewGuid();
                    ActivityRegistry.Add(oldActivity);
                    // Add old activity to the new activity's history
                    activity.History.Insert(0, oldActivity.Id);
                    // Add action to activity
                    activity.Actions.Add(obj);
                    // Add updated activity to the activity storage
                    ActivityRegistry.Add(activity);
                    // Notify subscribers
                    //Notifier.NotifyAll(NotificationType.ActionUpdated, action);
                }
            }
        }

        [RequireUser]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid activityId, Guid actionId)
        {
            Activity activity = ActivityRegistry.Get(activityId);
            if (IsOwner(activity))
            {
                // Find correct action
                Action action = (activity).Actions.SingleOrDefault(act => act.Id == actionId);
                // Remove action in activity
                (activity).Actions.Remove(action);
                // Notify subscribers
                //Notifier.NotifyAll(NotificationType.ActionDeleted, action);
            }
        }

        #endregion
    }
}