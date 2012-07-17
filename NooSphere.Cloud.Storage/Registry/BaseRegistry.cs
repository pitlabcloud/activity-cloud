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
using MongoDB.Driver.Builders;

namespace NooSphere.Cloud.Data.Registry
{
    public class BaseRegistry
    {
        #region Protected Members
        protected MongoDatabase database;
        #endregion

        #region Constructors
        public BaseRegistry(string connectionString)
        {
            database = MongoDatabase.Create(connectionString);
        }
        #endregion

        #region Protected Methods
        protected List<object> Get(MongoCollection<object> collection)
        {
            return collection.FindAllAs<object>().ToList();
        }

        protected object Get(MongoCollection<object> collection, Guid id)
        {
            return collection.FindOne(Query.EQ("_id", id));
        }

        protected bool Add(MongoCollection<object> collection, object obj)
        {
            return collection.Insert(obj, SafeMode.True).Ok;
        }

        protected bool Upsert(MongoCollection<object> collection, object obj, Guid id)
        {
            if (collection.FindOneById(id) != null)
                return collection.Update(Query.EQ("_id", id), Update.Replace(obj), SafeMode.True).Ok;
            else
                return Add(collection, obj);
        }

        protected bool Remove(MongoCollection<object> collection, Guid id)
        {
            return collection.Remove(Query.EQ("_id", id), SafeMode.True).Ok;
        }
        #endregion
    }
}
