using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using NooSphere.Core.ActivityModel;
using NooSphere.Helpers;

namespace NooSphere.Cloud.ActivityManager.Storage
{
    public class ActivityStorage
    {
        private const string amazonAccesKeyId = "AmazonAccessKeyId";
        private const string amazonSecretAccessKey = "AmazonSecretAccessKey";
        private const string bucketName = "dk.itu.pitlab.activitycloud.activities";

        private const string IdKey = "Id";
        private const string SizeKey = "Size";
        private const string CreationTimeKey = "CreationTime";
        private const string LastWriteTimeKey = "LastWriteTime";

        public AmazonS3Client SetupClient()
        {
            return new AmazonS3Client(ConfigurationManager.AppSettings[amazonAccesKeyId], ConfigurationManager.AppSettings[amazonSecretAccessKey]);
        }

        /// <summary>
        /// Linq query to return all stored activities
        /// </summary>
        /// <returns>a list of activities</returns>
        public List<Activity> GetActivities()
        {
            List<Activity> activities = new List<Activity>();
            using (var client = SetupClient())
            {
                foreach (var s3o in client.ListObjects(new ListObjectsRequest().WithBucketName(bucketName)).S3Objects)
                    activities.Add(GetActivity(s3o.Key));
            }
            return activities;
        }

        /// <summary>
        /// Add an activity in the cloud
        /// </summary>
        public void AddActivity(Activity newItem)
        {
            string serializedActivity = ObjectToJsonHelper.Convert(newItem);
            byte[] byteStream = Encoding.UTF8.GetBytes(serializedActivity);


            NameValueCollection metadata = new NameValueCollection();
            metadata.Add(ActivityStorage.IdKey, newItem.Identity.ID.ToString());
            metadata.Add(ActivityStorage.SizeKey, byteStream.Length.ToString());
            metadata.Add(ActivityStorage.CreationTimeKey, DateTime.UtcNow.ToString());
            metadata.Add(ActivityStorage.LastWriteTimeKey, DateTime.UtcNow.ToString());

            MemoryStream stream = new MemoryStream();
            stream.Write(byteStream, 0, (int)byteStream.Length);

            PutObjectRequest req = new PutObjectRequest();
            req.WithInputStream(stream);

            using (var client = SetupClient())
            {
                client.PutObject(req.WithBucketName(bucketName).WithKey(newItem.Identity.ID.ToString()).WithMetaData(metadata));
            }
        }

        /// <summary>
        /// Get an activity from the cloud
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Activity GetActivity(string id)
        {
            byte[] byteStream;
            using (var client = SetupClient())
            {
                GetObjectResponse response = client.GetObject(new GetObjectRequest().WithBucketName(bucketName).WithKey(id));
                using (BinaryReader reader = new BinaryReader(response.ResponseStream))
                {
                    byteStream = reader.ReadBytes((int)response.ContentLength);
                }
            }

            string json = Encoding.UTF8.GetString(byteStream);
            return JsonConvert.DeserializeObject<Activity>(json);
        }

        /// <summary>
        /// Removes an object from the tableservice
        /// </summary>
        /// <param name="item"></param>
        public void RemoveActivity(string id)
        {
            using (var client = SetupClient())
            {
                client.DeleteObject(new DeleteObjectRequest().WithBucketName(bucketName).WithKey(id));
            }
        }
    }
}