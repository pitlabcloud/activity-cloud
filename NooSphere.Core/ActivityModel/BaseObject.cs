using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace NooSphere.Core.ActivityModel
{
    [BsonKnownTypes(typeof(Activity), typeof(Device), typeof(User))]
    public class BaseObject
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool Equals(BaseObject obj)
        {
            return this.Id.ToString().Equals(obj.Id.ToString());
        }
    }
}
