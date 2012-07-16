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
            IsHistory = false;
        }

        private void InitializeProperties()
        {
            this.Actions = new List<Action>();
            this.Participants = new List<User>();
            this.History = new List<Guid>();
        }

        public List<Action> Actions { get; set; }
        public List<User> Participants { get; set; }
        public User Owner { get; set; }
        public bool IsHistory { get; set; }
        public List<Guid> History { get; set; }
    }
}
