using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace NooSphere.Cloud.Data.Registry
{
    public class FriendRequestRegistry : BaseRegistry
    {
        #region Constructors
        public FriendRequestRegistry(string connectionString) : base(connectionString) { }
        #endregion

        #region Public Methods
        public bool Add(object obj)
        {
            return base.Add(Collection, obj);
        }
        public bool Remove(Guid userId, Guid friendId)
        {
            return Collection.Remove(Query.And(Query.EQ("UserId", userId), Query.EQ("FriendId", friendId)), SafeMode.True).Ok;
        }
        #endregion

        #region Collection
        private string CollectionName = "friendrequests";
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
