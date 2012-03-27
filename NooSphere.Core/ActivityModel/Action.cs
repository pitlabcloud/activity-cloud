using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NooSphere.Core.ContextModel;
using NooSphere.Core.Primitives;
using System.ComponentModel;

namespace NooSphere.Core.ActivityModel
{
    public class Action : IEntity, INotifyPropertyChanged
    {
        public Action()
        {
            InitializeProperties();
        }

        #region Initializers
        private void InitializeProperties()
        {
            this.Identity = new Identity();
            this.Resources = new List<Resource>();
        }
        #endregion

        #region Properties
        private Identity _identity;
        public Identity Identity
        {
            get { return _identity; }
            set {
                this._identity = value;
                NotifyPropertyChanged("Identity");
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set {
                this._description = value;
                NotifyPropertyChanged("Description");
            }
        }

        private List<Resource> _resources;
        public List<Resource> Resources
        {
            get { return _resources; }
            set
            {
                this._resources = value;
                NotifyPropertyChanged("Resources");
            }
        }
        #endregion
        
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
