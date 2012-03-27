using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace NooSphere.Core.FileManagement
{
    public class FileBatch
    {
        public byte[] ByteStream { get; set; }
        public List<FileDetails> Files { get; set; }
        public long TotalSize
        {
            get
            {
                long total = 0;
                foreach (FileDetails fd in Files)
                {
                    total += fd.Size;
                }
                return total;
            }
        }

        public FileBatch()
        {
            ByteStream = new Byte[0];
            Files = new List<FileDetails>();
        }
    }
}