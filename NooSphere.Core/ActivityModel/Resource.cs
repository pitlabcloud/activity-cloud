using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NooSphere.Core.ActivityModel
{
    public class Resource : BaseObject
    {
        public Guid ActivityId { get; set; }
        public Guid ActionId { get; set; }
        public string RelativePath { get; set; }
        public int Size { get; set; }
        public string CreationTime { get; set; }
        public string LastWriteTime { get; set; }
    }
}
