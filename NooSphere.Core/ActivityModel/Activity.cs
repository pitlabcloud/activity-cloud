using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NooSphere.Core;
using System.ComponentModel;
using NooSphere.Core.ContextModel;
using NooSphere.Core.ContextModel.ComponentModel;
using Newtonsoft.Json;


namespace NooSphere.Core.ActivityModel
{
    public class Activity : BaseObject
    {
        public Activity()
            : base()
        {
            InitializeProperties();
        }

        private void InitializeProperties()
        {
            this.Actions = new List<Action>();
            this.Participants = new Dictionary<Guid, string>();
            this.History = new LinkedList<Guid>();
        }

        public List<Action> Actions { get; set; }
        public Dictionary<Guid, string> Participants { get; set; }
        [JsonIgnore]
        public LinkedList<Guid> History { get; set; }
    }
}
