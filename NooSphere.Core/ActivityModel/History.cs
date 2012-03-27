using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NooSphere.Core;
using NooSphere.Core.ContextModel;
using NooSphere.Core.Primitives;

namespace NooSphere.Core.ActivityModel
{
    public class History : Dictionary<DateTime,Activity>,IEntity
    {
        public History()
        {
            this.Identity = new Identity();
        }

        public Identity Identity { get; set; }
    }
}
