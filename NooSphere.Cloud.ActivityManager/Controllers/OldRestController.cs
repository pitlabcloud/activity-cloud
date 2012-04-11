using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NooSphere.Cloud.Authentication;
using NooSphere.Core.ActivityModel;
using NooSphere.Core.FileManagement;
using System.Web;
using NooSphere.Cloud.ActivityManager.Storage;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class OldRestController : ApiController
    {
        private static FileStorage fileStorage = new FileStorage();
        private static ActivityStorage activityStorage = new ActivityStorage();

        // GET /activitymanager/activities
        public IEnumerable<Activity> Get()
        {
            return activityStorage.GetActivities();
        }

        // GET /activitymanager/activities/5
        public Activity Get(string id)
        {
            return activityStorage.GetActivity(id);
        }

        // POST /activitymanager/activities
        public FileBatch Post(Activity act)
        {
            if (act != null)
            {
                // Add activity to DataTableManager
                activityStorage.AddActivity(act);
                // TODO: Sent ChangeBatch to all subscribers
                return fileStorage.GetChangeBatch(act);
            }
            return null;
        }

        // PUT /activitymanager/activities/5
        public FileBatch Put(string id, Activity act)
        {
            var oldActivity = activityStorage.GetActivity(id);
            // Add updated activity to the DataTableManager
            activityStorage.AddActivity(act);
            // Get changeBatch between old and new activity
            FileBatch changeBatch = fileStorage.GetChangeBatch(oldActivity, act);
            // Delete all unreferenced files, taht might be left after activity update
            // Add those deleted files to the changeBatch
            changeBatch.Files.AddRange(fileStorage.DeleteUnreferencedFiles(activityStorage.GetActivities()));
            return changeBatch;
        }

        // DELETE /activitymanager/activities/5
        public FileBatch Delete(string id)
        {
            // Remove activity in DataTableManager
            activityStorage.RemoveActivity(id);
            // Delete all unreferenced files, that might be left after activity removal
            // Add those deleted files to the changeBatch
            FileBatch changeBatch = new FileBatch();
            changeBatch.Files = fileStorage.DeleteUnreferencedFiles(activityStorage.GetActivities());
            // TODO: Sent ChangeBatch to all subscribers
            return changeBatch;
        }
    }
}