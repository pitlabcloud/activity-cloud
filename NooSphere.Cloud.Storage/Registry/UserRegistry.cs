using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.Data.Registry
{
    public class UserRegistry : BaseRegistry
    {
        public UserRegistry(string connectionString) : base(connectionString) { }

        public bool ExistingId(Guid userId)
        {
            return Collection.Find(Query.EQ("_id", userId)).Count() > 0;
        }
        public bool ExistingEmail(string email)
        {
            return Collection.Find(Query.EQ("Email", email)).Count() > 0;
        }

        public User GetUserOnEmail(string email)
        {
            return Collection.FindOneAs<User>(Query.EQ("Email", email));
        }

        #region MongoDbStorage method pointers
        public List<User> Get()
        {
            return base.Get(Collection).Cast<User>().ToList();
        }

        public User Get(Guid id)
        {
            return (User)base.Get(Collection, id);
        }

        public bool Add(User obj)
        {
            return base.Add(Collection, obj);
        }

        public bool Remove(Guid id)
        {
            return base.Remove(Collection, id);
        }
        #endregion

        #region Collection
        private string CollectionName = "users";
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