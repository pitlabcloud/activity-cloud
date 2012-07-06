using System;
using System.Configuration;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.Data.Storage;

namespace NooSphere.Cloud.ActivityManager.Controllers
{

    public class FileController : BaseController
    {
        [RequireUser]
        public byte[] Get(Guid activityId, Guid actionId, Guid resourceId)
        {
            return FileStorage.Download(resourceId);
        }

        [RequireUser]
        public bool Post(Guid activityId, Guid actionId, Guid resourceId, byte[] data)
        {
            return FileStorage.Upload(resourceId, "filename", DateTime.Now, DateTime.Now, 0, data);
        }

        [RequireUser]
        public void Post(Guid resourceId, int size, string creationTime, string lastWriteTime, string relativePath, byte[] data)
        {
            FileStorage.Upload(resourceId, relativePath, DateTime.Parse(creationTime), DateTime.Parse(lastWriteTime), size, data);
        }
    }
}
