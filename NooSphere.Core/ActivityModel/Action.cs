using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NooSphere.Core.ContextModel;
using System.ComponentModel;

namespace NooSphere.Core.ActivityModel
{
    public class Action : BaseObject
    {
        #region Constructors
        public Action()
            : base()
        {
            InitializeProperties();
        }
        #endregion

        #region Initializers
        private void InitializeProperties()
        {
            this.Resources = new List<Resource>();
        }
        #endregion

        #region Properties
        public List<Resource> Resources { get; set; }
        #endregion
    }
}
