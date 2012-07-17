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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class ParticipantController : BaseController
    {
        #region Private Members
        private ActivityController ActivityController = new ActivityController();
        private ActivityRegistry ActivityRegistry = new ActivityRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        private UserStorage UserStorage = new UserStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        private ActivityStorage ActivityStorage = new ActivityStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        #endregion

        #region Exposed API Methods
        /// <summary>
        /// Add participant to the specified activity.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        /// <param name="participantId">Guid representation of the user Id.</param>
        /// <returns>Returns true if participant was added, false if not.</returns>
        [RequireUser]
        public bool Post(Guid activityId, Guid participantId)
        {
            if (activityId != null && participantId != null)
            {
                JObject activity = ActivityStorage.Get(activityId);
                JObject participant = UserStorage.Get(participantId);
                
                List<JObject> participants = activity["Participants"].Children<JObject>().ToList();
                participants.Add(participant);
                activity["Participants"] = JToken.FromObject(participants);

                ActivityController.UpdateActivity(Events.NotificationType.None, activity);
                Notifier.NotifyGroup(activityId, NotificationType.ParticipantAdded, new { ActivityId = activityId, Participant = participant });
                Notifier.NotifyGroup(participantId, NotificationType.ActivityAdded, activity);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a participant from the specified activity.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        /// <param name="participantId">Guid representation of the user Id.</param>
        /// <returns>Returns true if participant was removed, false if not.</returns>
        [RequireUser]
        public bool Delete(Guid activityId, Guid participantId)
        {
            if (activityId != null && participantId != null)
            {
                Activity activity = ActivityRegistry.Get(activityId);
                JObject participant = UserStorage.Get(participantId);
                List<User> participants = activity.Participants.Where(u => u.Id != participantId).ToList();

                List<JObject> result = new List<JObject>();
                foreach (User p in participants)
                    result.Add(UserStorage.Get(p.Id));

                JObject completeActivity = ActivityStorage.Get(activityId);
                completeActivity["Participants"] = JToken.FromObject(result);

                ActivityController.UpdateActivity(Events.NotificationType.None, completeActivity);
                Notifier.NotifyGroup(activityId, NotificationType.ParticipantRemoved, new { ActivityId = activityId, Participant = participant });
                Notifier.NotifyGroup(participantId, NotificationType.ActivityDeleted, new { Id = activity.Id } );

                return true;
            }
            return false;
        }
        #endregion
    }
}
