using System;
using System.Runtime.Serialization;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.DataSource
{
    [DataContract]
    public class ActivityWrapper
    {
        [DataMember]
        public string ID;
        [DataMember]
        public Activity Activity;
    }
}