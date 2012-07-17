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
using MongoDB.Bson.Serialization.Attributes;

namespace NooSphere.Core.ActivityModel
{
    public class BaseObject
    {
        #region Properties
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        #endregion

        public bool Equals(BaseObject obj)
        {
            return this.Id.ToString().Equals(obj.Id.ToString());
        }
    }
}
