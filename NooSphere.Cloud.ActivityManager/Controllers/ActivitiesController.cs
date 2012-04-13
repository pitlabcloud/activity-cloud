using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NooSphere.Cloud.Authentication;
using NooSphere.Core.ActivityModel;
using NooSphere.Core.FileManagement;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using NooSphere.Cloud.ActivityManager.Storage;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class ActivitiesController : ApiController
    {
        private static ActivityStorage activityStorage = new ActivityStorage();
        private static FileStorage fileStorage = new FileStorage();

        // GET /api/activities
        [RequireParticipant]
        public IEnumerable<Activity> Get()
        {
            return activityStorage.GetActivities().Where(a => IsAuthorizedForActivity(a));
            //return dtm.GetActivities().Where(a => IsAuthorizedForActivity(a));
        }

        // GET /api/activities/{id}
        [RequireParticipant]
        public Activity Get(string id)
        {
            string email = HttpContext.Current.User.Identity.Name;
            Activity a = activityStorage.GetActivity(id);
            //Activity a = dtm.GetActivity(id);
            if (IsAuthorizedForActivity(a))
                return a;
            else
                return null;
        }

        // POST /api/activities
        [RequireParticipant]
        public FileBatch Post(Activity act)
        {
            if (act != null)
            {
                if (IsAuthorizedForActivity(act))
                {
                    // Add activity to DataTableManager
                    activityStorage.AddActivity(act);
                    //dtm.AddActivity(act);
                    // TODO: Sent ChangeBatch to all subscribers
                    return fileStorage.GetChangeBatch(act);
                }
                return null;
            }
            else
                return null;
        }

        // PUT /api/activities/{id}
        [RequireParticipant]
        public FileBatch Put(string id, Activity act)
        {
            if (IsAuthorizedForActivity(act))
            {
                var oldActivity = activityStorage.GetActivity(id);
                //var oldActivity = dtm.GetActivity(id);
                // Add updated activity to the DataTableManager
                activityStorage.AddActivity(act);
                //dtm.AddActivity(act);
                // Get changeBatch between old and new activity
                FileBatch changeBatch = fileStorage.GetChangeBatch(oldActivity, act);
                // Delete all unreferenced files, taht might be left after activity update
                // Add those deleted files to the changeBatch
                //changeBatch.Files.AddRange(cfm.DeleteUnreferencedFiles(dtm.GetActivities()));
                changeBatch.Files.AddRange(fileStorage.DeleteUnreferencedFiles(activityStorage.GetActivities()));
                return changeBatch;
            } return null;
        }

        // DELETE /api/activities/{id}
        [RequireParticipant]
        public FileBatch Delete(string id)
        {
            if(IsAuthorizedForActivity(activityStorage.GetActivity(id)))
            //if (IsAuthorizedForActivity(dtm.GetActivity(id)))
            {
                // Remove activity in DataTableManager
                activityStorage.RemoveActivity(id);
                //dtm.RemoveActivity(id);
                // Delete all unreferenced files, that might be left after activity removal
                // Add those deleted files to the changeBatch
                FileBatch changeBatch = new FileBatch();
                changeBatch.Files = fileStorage.DeleteUnreferencedFiles(activityStorage.GetActivities());
                //changeBatch.Files = cfm.DeleteUnreferencedFiles(dtm.GetActivities());
                // TODO: Sent ChangeBatch to all subscribers
                return changeBatch;
            } return null;
        }

        private bool IsAuthorizedForActivity(Activity a)
        {
            string email = ((Participant)HttpContext.Current.User).Email;
            if ((a.Owner != null && a.Owner.Email == email) || (a.Collaborators != null && a.Collaborators.Count(col => col.Email == email) == 1))
                return true;
            else
                return false;
        }
    }
}