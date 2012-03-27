using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NooSphere.Core;
using System.ComponentModel;
using NooSphere.Core.ContextModel;
using NooSphere.Core.Primitives;
using NooSphere.Core.ContextModel.ComponentModel;


namespace NooSphere.Core.ActivityModel
{
    /// <summary>
    /// Activity Base Class
    /// </summary>
    public class Activity : INotifyPropertyChanged
    {
        #region Constructors
        public Activity()
        {
            InitializeProperties();
        }
        #endregion

        #region Initializers
        private void InitializeProperties()
        {
            this.Identity = new Identity();
            this.Actions = new List<Action>();
            this.Workflows = new List<Workflow>();
            this.Actors = new List<Actor>();
            this.Collaborators = new List<Collaborator>();
            this.Owner = new Participant();
            this.History = new List<History>();
            this.Meta = new Metadata();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Context is a generic object so it can be included
        /// into serialisation.
        /// </summary>
        private object _context;
        public object Context
        {
            get { return _context; }
            set
            {
                this._context = value;
                NotifyPropertyChanged("Context");

            }
        }

        private Identity _identity;
        public Identity Identity
        {
            get { return _identity; }
            set
            {
                this._identity = value;
                NotifyPropertyChanged("Identity");

            }
        }
        private List<Action> _actions;
        public List<Action> Actions
        {
            get { return _actions; }
            set
            {
                this._actions = value;
                NotifyPropertyChanged("Actions");

            }
        }
        private List<Workflow> _workflows;
        public List<Workflow> Workflows
        {
            get { return _workflows; }
            set
            {
                this._workflows = value;
                NotifyPropertyChanged("Workflows");

            }
        }
        private Participant _owner;
        public Participant Owner
        {
            get { return _owner; }
            set
            {
                this._owner= value;
                NotifyPropertyChanged("Owner");

            }
        }
        private List<Actor> _actors;
        public List<Actor> Actors
        {
            get { return _actors; }
            set
            {
                this._actors = value;
                NotifyPropertyChanged("Actors");

            }
        }

        private List<Collaborator> _collaborators;
        public List<Collaborator> Collaborators
         {
             get { return _collaborators; }
            set
            {
                this._collaborators = value;
                NotifyPropertyChanged("Collaborators");

            }
        }
        private List<History> _history;
        public List<History> History
        {
            get { return _history; }
            set
            {
                this._history = value;
                NotifyPropertyChanged("History");

            }
        }
        private Metadata _meta;
        public Metadata Meta
        {
            get { return _meta; }
            set
            {
                this._meta = value;
                NotifyPropertyChanged("Metae");

            }
        }
        #endregion

        public List<Resource> GetResources()
        {
            List<Resource> resources = new List<Resource>();

            foreach (Action a in Actions)
                resources.AddRange(a.Resources);

            return resources;
        }

        #region Overrides
        public override string ToString()
        {
            return Identity.Name;
        }
        public bool Equals(Activity act)
        {
            return this.Identity.ID == act.Identity.ID;
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

        #region Lifecycle
        public bool Suspendable { get; set; }
        public bool Resumable { get; set; }
        public bool Shareable { get; set; }
        public bool Roameable { get; set; }
        public bool Externalizeable{get;set;}
        #endregion
    }
}
