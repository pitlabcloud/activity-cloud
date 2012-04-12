using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using NooSphere.Core.FileManagement;
using NooSphere.Cloud.ActivityManager.Storage;
using NooSphere.Cloud.Authentication;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class FilesController : ApiController
    {
        private static FileStorage fileStorage = new FileStorage();

        [HttpPost]
        [RequireParticipant]
        public FileBatch ForwardCall(FileBatch batch)
        {
            if (batch.ByteStream.Length > 0)
                return Upload(batch);
            else
                return Download(batch);
        }

        [NonAction]
        private FileBatch Upload(FileBatch batch)
        {
            fileStorage.UploadFiles(batch);
            return null;
        }

        [NonAction]
        private FileBatch Download(FileBatch batch)
        {
            return fileStorage.DownloadFiles(batch);
        }
    }
}
