using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NooSphere.Core.ActivityModel
{
    public class Device : BaseObject
    {
        public Guid UserId { get; set; }
        public Guid ConnectionId { get; set; }
    }
}
