using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization;

namespace NooSphere.Core.FileManagement
{
    public class FileDetails
    {
        public string FileName { get; set; }
        public string CloudPath { get; set; }
        public long Size { get; set; }
        public string CreationTime { get; set; }
        public string LastAccessTime { get; set; }
        public string LastWriteTime { get; set; }
        public SyncAction SyncAction { get; set; }

        public FileDetails()
        {
            SyncAction = SyncAction.None;
        }
    }

    public enum SyncAction
    {
        Download,
        Upload,
        Delete,
        None
    }
}