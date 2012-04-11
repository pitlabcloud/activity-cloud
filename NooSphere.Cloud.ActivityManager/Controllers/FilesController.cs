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
        public void Upload(FileBatch batch)
        {
            fileStorage.UploadFiles(batch);
        }

        [HttpPost]
        [RequireParticipant]
        public FileBatch Download(FileBatch batch)
        {
            return fileStorage.DownloadFiles(batch);
        }
    }
}
