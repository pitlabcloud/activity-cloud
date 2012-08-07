#region License

// Copyright (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)
// 
// Pervasive Interaction Technology Laboratory (pIT lab)
// IT University of Copenhagen
// 
// This library is free software; you can redistribute it and/or 
// modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
// as published by the Free Software Foundation. Check 
// http://www.gnu.org/licenses/gpl.html for details.

#endregion

#region

using System.Collections.Generic;

#endregion

namespace NooSphere.Core.ActivityModel
{
    public class User : BaseObject
    {
        #region Properties

        public string Email { get; set; }
        public string Status { get; set; }
        public List<User> Friends { get; set; }

        #endregion

        #region Constructors

        public User()
        {
            InitializeProperties();
        }

        #endregion

        #region Initializers

        private void InitializeProperties()
        {
            Friends = new List<User>();
        }

        #endregion
    }
}