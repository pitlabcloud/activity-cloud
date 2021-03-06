﻿#region License

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

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace NooSphere.Cloud.Data.Storage
{
    public class ActivityStorage : BaseStorage
    {
        private const string bucketName = "noosphere.activitycloud.activities";

        #region Constructors

        public ActivityStorage(string accessKey, string accessSecret)
            : base(accessKey, accessSecret)
        {
        }

        #endregion

        #region Public Methods

        public List<JObject> Get()
        {
            return base.Get(bucketName);
        }

        public JObject Get(Guid id)
        {
            return base.Get(bucketName, id);
        }

        public void Add(Guid id, object newItem)
        {
            base.Add(bucketName, id, newItem);
        }

        public void Remove(Guid id)
        {
            base.Remove(bucketName, id);
        }

        #endregion
    }
}