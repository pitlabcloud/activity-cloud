using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NooSphere.Cloud.Authentication;
using NooSphere.Core.ActivityModel;
using NooSphere.Core.FileManagement;
using System.Web;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class OldRestController : ApiController
    {
        private static CloudFileManager cfm = new CloudFileManager();
        private static ActivityTableManager dtm = new ActivityTableManager();

        // GET /activitymanager/activities
        public IEnumerable<Activity> Get()
        {
            return dtm.GetActivities();
        }

        // GET /activitymanager/activities/5
        public Activity Get(string id)
        {
            return dtm.GetActivity(id);
        }

        // POST /activitymanager/activities
        public FileBatch Post(Activity act)
        {
            if (act != null)
            {
                // Add activity to DataTableManager
                dtm.AddActivity(act);
                // TODO: Sent ChangeBatch to all subscribers
                return cfm.GetChangeBatch(act);
            }
            return null;
        }

        // PUT /activitymanager/activities/5
        public FileBatch Put(string id, Activity act)
        {
            var oldActivity = dtm.GetActivity(id);
            // Add updated activity to the DataTableManager
            dtm.AddActivity(act);
            // Get changeBatch between old and new activity
            FileBatch changeBatch = cfm.GetChangeBatch(oldActivity, act);
            // Delete all unreferenced files, taht might be left after activity update
            // Add those deleted files to the changeBatch
            changeBatch.Files.AddRange(cfm.DeleteUnreferencedFiles(dtm.GetActivities()));
            return changeBatch;
        }

        // DELETE /activitymanager/activities/5
        public FileBatch Delete(string id)
        {
            // Remove activity in DataTableManager
            dtm.RemoveActivity(id);
            // Delete all unreferenced files, that might be left after activity removal
            // Add those deleted files to the changeBatch
            FileBatch changeBatch = new FileBatch();
            changeBatch.Files = cfm.DeleteUnreferencedFiles(dtm.GetActivities());
            // TODO: Sent ChangeBatch to all subscribers
            return changeBatch;
        }
    }
}