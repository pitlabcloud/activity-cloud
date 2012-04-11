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
    public class ParticipantStorage
    {
        private const string amazonAccesKeyId = "AmazonAccessKeyId";
        private const string amazonSecretAccessKey = "AmazonSecretAccessKey";
        private const string bucketName = "dk.itu.pitlab.activitycloud.participants";

        private const string IdKey = "Id";
        private const string SizeKey = "Size";
        private const string CreationTimeKey = "CreationTime";
        private const string LastWriteTimeKey = "LastWriteTime";

        public AmazonS3Client SetupClient()
        {
            return new AmazonS3Client(ConfigurationManager.AppSettings[amazonAccesKeyId], ConfigurationManager.AppSettings[amazonSecretAccessKey]);
        }

        /// <summary>
        /// Linq query to return all stored participants
        /// </summary>
        /// <returns>a list of participants</returns>
        public List<Participant> GetParticipants()
        {
            List<Participant> participants = new List<Participant>();
            using (var client = SetupClient())
            {
                foreach (var s3o in client.ListObjects(new ListObjectsRequest().WithBucketName(bucketName)).S3Objects)
                    participants.Add(GetParticipant(s3o.Key));
            }
            return participants;
        }

        /// <summary>
        /// Add a participant in the cloud
        /// </summary>
        public void AddParticipant(Participant newItem)
        {
            string serializedParticipant = ObjectToJsonHelper.Convert(newItem);
            byte[] byteStream = Encoding.UTF8.GetBytes(serializedParticipant);


            NameValueCollection metadata = new NameValueCollection();
            metadata.Add(ParticipantStorage.IdKey, newItem.Identity.ID.ToString());
            metadata.Add(ParticipantStorage.SizeKey, byteStream.Length.ToString());
            metadata.Add(ParticipantStorage.CreationTimeKey, DateTime.UtcNow.ToString());
            metadata.Add(ParticipantStorage.LastWriteTimeKey, DateTime.UtcNow.ToString());

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
        /// Get a particpant from the cloud
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Participant GetParticipant(string id)
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
            return JsonConvert.DeserializeObject<Participant>(json);
        }

        /// <summary>
        /// Remove a participant from cloud
        /// </summary>
        /// <param name="item"></param>
        public void RemoveParticipant(string id)
        {
            using (var client = SetupClient())
            {
                client.DeleteObject(new DeleteObjectRequest().WithBucketName(bucketName).WithKey(id));
            }
        }
    }
}