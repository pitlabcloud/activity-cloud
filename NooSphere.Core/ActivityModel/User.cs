using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NooSphere.Core.ContextModel;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Xml;
using System.Security.Principal;
using MongoDB.Bson.Serialization.Attributes;

namespace NooSphere.Core.ActivityModel
{
    [BsonKnownTypes(typeof(User))]
    public class User : BaseObject
    {
        public string Email { get; set; }
    }
}
