using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using NooSphere.Core.FileManagement;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class FilesController : ApiController
    {
        private static CloudFileManager cfm = new CloudFileManager();

        public void Post(FileBatch batch)
        {
            cfm.UploadToBlob(batch);
        }

        public FileBatch Get(FileBatch batch)
        {
            return cfm.DownloadFromBlob(batch);
        }
    }
}
