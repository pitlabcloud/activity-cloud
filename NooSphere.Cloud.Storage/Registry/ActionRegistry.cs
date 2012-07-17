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
using System.Linq;
using MongoDB.Driver;

namespace NooSphere.Cloud.Data.Registry
{
    public class ActionRegistry : BaseRegistry
    {
        #region Constructors
        public ActionRegistry(string connectionString) : base(connectionString) { }
        #endregion

        #region Public Methods
        public List<NooSphere.Core.ActivityModel.Action> Get()
        {
            return base.Get(Collection).Cast<NooSphere.Core.ActivityModel.Action>().ToList();
        }

        public NooSphere.Core.ActivityModel.Action Get(Guid id)
        {
            return (NooSphere.Core.ActivityModel.Action)base.Get(Collection, id);
        }

        public bool Add(object obj)
        {
            return base.Add(Collection, obj);
        }

        public bool Remove(Guid id)
        {
            return base.Remove(Collection, id);
        }
        #endregion

        #region Collection
        private string CollectionName = "actions";
        protected MongoCollection<object> Collection
        {
            get
            {
                return database.GetCollection<object>(CollectionName);
            }
        }
        #endregion
    }
}