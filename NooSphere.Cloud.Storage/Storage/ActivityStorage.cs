using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NooSphere.Cloud.Data.Storage
{
    public class ActivityStorage : BaseStorage
    {
        private const string bucketName = "noosphere.activitycloud.activities";

        public ActivityStorage(string accessKey, string accessSecret)
            : base(accessKey, accessSecret) { }

        public List<JObject> Get()
        {
            return base.Get(bucketName);
        }

        public new JObject Get(Guid id)
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
    }
}
