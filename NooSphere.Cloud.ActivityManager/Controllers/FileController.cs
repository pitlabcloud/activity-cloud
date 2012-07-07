using System;
using System.Configuration;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.Data.Storage;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class FileController : BaseController
    {
        /// <summary>
        /// Download the resource.
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        /// <param name="actionId">Guid representation of the action Id.</param>
        /// <param name="resourceId">Guid representation of the resource Id.</param>
        /// <returns>byte[] of the given resource</returns>
        [RequireUser]
        public byte[] Get(Guid activityId, Guid actionId, Guid resourceId)
        {
            return FileStorage.Download(Id(activityId, actionId, resourceId));
        }

        /// <summary>
        /// Upload the resource
        /// </summary>
        /// <param name="activityId">Guid representation of the activity Id.</param>
        /// <param name="actionId">Guid representation of the action Id.</param>
        /// <param name="resourceId">Guid representation of the resource Id.</param>
        /// <param name="size">Size of the resource.</param>
        /// <param name="creationTime">Creation time of the resource.</param>
        /// <param name="lastWriteTime">Last write time of the resource.</param>
        /// <param name="relativePath">Relative path including the filename.</param>
        /// <param name="data">The resource as a byte[].</param>
        [RequireUser]
        public void Post(Guid activityId, Guid actionId, Guid resourceId, int size, string creationTime, string lastWriteTime, string relativePath, byte[] data)
        {

            FileStorage.Upload(Id(activityId, actionId, resourceId), relativePath, DateTime.Parse(creationTime), DateTime.Parse(lastWriteTime), size, data);
        }

        private string Id(Guid activityId, Guid actionId, Guid resourceId)
        {
            return "Activities/" + activityId + "/Actions/" + actionId + "/Resources/" + resourceId;
        }
    }
}
