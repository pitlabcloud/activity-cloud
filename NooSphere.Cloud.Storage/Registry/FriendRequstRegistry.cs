using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NooSphere.Core.ActivityModel;

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
        public List<FriendRequest> Get()
        {
            return base.Get(Collection).Cast<FriendRequest>().ToList();
        }
        public List<FriendRequest> Get(Guid userId)
        {
            return Collection.FindAs<FriendRequest>(Query.EQ("FriendId", userId)).ToList();
        }
        public bool Exists(Guid userId, Guid friendId)
        {
            return Collection.Find(Query.Or(
                Query.And(Query.EQ("UserId", userId), Query.EQ("FriendId", friendId)),
                Query.And(Query.EQ("UserId", friendId), Query.EQ("FriendId", userId)))).Count() > 0;
        }
        public bool Remove(Guid userId, Guid friendId)
        {
            return Collection.Remove(Query.And(Query.EQ("UserId", userId), Query.EQ("FriendId", friendId)), SafeMode.True).Ok;
        }
        public bool Remove(Guid friendRequestId)
        {
            return Collection.Remove(Query.EQ("_id", friendRequestId), SafeMode.True).Ok;
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
