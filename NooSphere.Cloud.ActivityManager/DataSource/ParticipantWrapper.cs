using System;
using System.Runtime.Serialization;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.DataSource
{
    [DataContract]
    public class ParticipantWrapper
    {
        [DataMember]
        public string ID;
        [DataMember]
        public Participant Participant;
    }
}