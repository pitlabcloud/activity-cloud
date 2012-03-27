using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NooSphere.Core.Primitives;
using NooSphere.Core.FileManagement;
using System.IO;

namespace NooSphere.Core.ActivityModel
{
    public class Resource:IEntity
    {
        public Resource(FileInfo fileInfo)
        {
            this.Identity = new Identity();
            FileDetails fd = new FileDetails();
            fd.CreationTime = fileInfo.CreationTimeUtc.ToString();
            fd.LastWriteTime = fileInfo.LastWriteTimeUtc.ToString();
            fd.Size = fileInfo.Length;
            fd.FileName = fileInfo.Name;
            this.FileDetails = fd;
            this.Type = FileType.Cloud;
        }

        public Resource()
        {
            this.Identity = new Identity();
            this.FileDetails = new FileDetails();
            this.Type = FileType.Cloud;
        }

        public Identity Identity { get; set; }
        public FileType Type { get; set; }
        public Service Service { get; set; }
        public FileDetails FileDetails { get; set; }
    }

    public enum FileType
    {
        Local,
        Cloud
    }
}
