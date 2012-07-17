/// <licence>
/// 
/// (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)
/// 
/// Pervasive Interaction Technology Laboratory (pIT lab)
/// IT University of Copenhagen
///
/// This library is free software; you can redistribute it and/or 
/// modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
/// as published by the Free Software Foundation. Check 
/// http://www.gnu.org/licenses/gpl.html for details.
/// 
/// </licence>

using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;


namespace NooSphere.Core.ActivityModel
{
    public class Activity : BaseObject
    {
        #region Constructors
        public Activity()
            : base()
        {
            InitializeProperties();
        }
        #endregion

        #region Initializers
        private void InitializeProperties()
        {
            this.Actions = new List<Action>();
            this.Participants = new List<User>();
            this.History = new List<Guid>();
            this.IsHistory = false;
        }
        #endregion

        #region Properties
        public List<Action> Actions { get; set; }
        public List<User> Participants { get; set; }
        public User Owner { get; set; }
        public bool IsHistory { get; set; }
        public List<Guid> History { get; set; }
        #endregion
    }
}
