using System.Configuration;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Specialized;
using System;

namespace NooSphere.Cloud.Data.Storage
{
    public class FileStorage
    {
        private const string bucketName = "dk.itu.pitlab.activitycloud.files";

        private const string RelativePathKey = "RelativePath";
        private const string CreationTimeKey = "CreationTime";
        private const string LastWriteTimeKey = "LastWriteTime";
        private const string SizeKey = "Size";

        private string AccessKey;
        private string AccessSecret;

        public FileStorage(string accessKey, string accessSecret)
        {
            AccessKey = accessKey;
            AccessSecret = accessSecret;
        }

        public byte[] Download(string id)
        {
            using (var client = SetupClient())
            {
                GetObjectResponse response = client.GetObject(new GetObjectRequest().WithBucketName(bucketName).WithKey(id));
                using (MemoryStream ms = new MemoryStream())
                {
                    response.ResponseStream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public bool Upload(string id, string relativePath, DateTime creationTime, DateTime lastWriteTime, int size, byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);

            NameValueCollection metadata = new NameValueCollection();
            metadata.Add(RelativePathKey, relativePath);
            metadata.Add(CreationTimeKey, creationTime.ToString("u"));
            metadata.Add(LastWriteTimeKey, lastWriteTime.ToString("u"));
            metadata.Add(SizeKey, size.ToString());

            var req = new PutObjectRequest();
            req.WithInputStream(stream);
            req.WithMetaData(metadata);

            using (var client = SetupClient())
            {
                client.PutObject(req.WithBucketName(bucketName).WithKey(id));
            }

            return true;
        }

        private AmazonS3Client SetupClient()
        {
            return new AmazonS3Client(AccessKey, AccessSecret);
        }
    }
}