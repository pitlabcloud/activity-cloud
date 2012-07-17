﻿/// <licence>
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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NooSphere.Cloud.Data.Storage
{
    public class BaseStorage
    {
        #region Private Members
        private const string IdKey = "Id";
        private const string SizeKey = "Size";
        private const string CreationTimeKey = "CreationTime";
        private const string LastWriteTimeKey = "LastWriteTime";

        private string AccessKey;
        private string AccessSecret;
        #endregion

        #region Constructors
        public BaseStorage(string accessKey, string accessSecret)
        {
            AccessKey = accessKey;
            AccessSecret = accessSecret;
        }
        #endregion

        #region Protected Methods
        protected List<JObject> Get(string bucketName)
        {
            List<JObject> users = new List<JObject>();
            using (var client = SetupClient())
            {
                foreach (var s3o in client.ListObjects(new ListObjectsRequest().WithBucketName(bucketName)).S3Objects)
                    users.Add(Get(bucketName, new Guid(s3o.Key)));
            }
            return users;
        }

        protected JObject Get(string bucketName, Guid id)
        {
            byte[] byteStream;
            using (var client = SetupClient())
            {
                GetObjectResponse response = client.GetObject(new GetObjectRequest().WithBucketName(bucketName).WithKey(id.ToString()));
                using (BinaryReader reader = new BinaryReader(response.ResponseStream))
                {
                    byteStream = reader.ReadBytes((int)response.ContentLength);
                }
            }

            string json = Encoding.UTF8.GetString(byteStream);
            return JsonConvert.DeserializeObject<JObject>(json);
        }

        protected void Add(string bucketName, Guid id, object newItem)
        {
            string serializedUser = JsonConvert.SerializeObject(newItem);
            byte[] byteStream = Encoding.UTF8.GetBytes(serializedUser);

            NameValueCollection metadata = new NameValueCollection();
            metadata.Add(BaseStorage.IdKey, id.ToString());
            metadata.Add(BaseStorage.SizeKey, byteStream.Length.ToString());
            metadata.Add(BaseStorage.CreationTimeKey, DateTime.UtcNow.ToString());
            metadata.Add(BaseStorage.LastWriteTimeKey, DateTime.UtcNow.ToString());

            MemoryStream stream = new MemoryStream();
            stream.Write(byteStream, 0, (int)byteStream.Length);

            PutObjectRequest req = new PutObjectRequest();
            req.WithInputStream(stream);

            using (var client = SetupClient())
            {
                client.PutObject(req.WithBucketName(bucketName).WithKey(id.ToString()).WithMetaData(metadata));
            }
        }

        protected void Remove(string bucketName, Guid id)
        {
            using (var client = SetupClient())
            {
                client.DeleteObject(new DeleteObjectRequest().WithBucketName(bucketName).WithKey(id.ToString()));
            }
        }
        #endregion

        #region Private Methods
        private AmazonS3Client SetupClient()
        {
            AmazonS3Config S3Config = new AmazonS3Config
            {
                ServiceURL = "s3.amazonaws.com",
                CommunicationProtocol = Amazon.S3.Model.Protocol.HTTP
            };

            return new AmazonS3Client(AccessKey, AccessSecret, S3Config);
        }
        #endregion
    }
}
