﻿#region License

// Copyright (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)
// 
// Pervasive Interaction Technology Laboratory (pIT lab)
// IT University of Copenhagen
// 
// This library is free software; you can redistribute it and/or 
// modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
// as published by the Free Software Foundation. Check 
// http://www.gnu.org/licenses/gpl.html for details.

#endregion

#region

using System;
using System.Collections.Specialized;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

#endregion

namespace NooSphere.Cloud.Data.Storage
{
    public class FileStorage
    {
        private const string bucketName = "noosphere.activitycloud.files";

        private const string RelativePathKey = "RelativePath";
        private const string CreationTimeKey = "CreationTime";
        private const string LastWriteTimeKey = "LastWriteTime";
        private const string SizeKey = "Size";

        private readonly string AccessKey;
        private readonly string AccessSecret;

        #region Constructors

        public FileStorage(string accessKey, string accessSecret)
        {
            AccessKey = accessKey;
            AccessSecret = accessSecret;
        }

        #endregion

        #region Public Methods

        public Stream Download(string id)
        {
            try
            {
                using (AmazonS3Client client = SetupClient())
                    return
                        client.GetObject(new GetObjectRequest().WithBucketName(bucketName).WithKey(id)).ResponseStream;
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }

        public bool Upload(string id, string relativePath, DateTime creationTime, DateTime lastWriteTime, int size,
                           Stream stream)
        {
            var metadata = new NameValueCollection();
            metadata.Add(RelativePathKey, relativePath);
            metadata.Add(CreationTimeKey, creationTime.ToString("u"));
            metadata.Add(LastWriteTimeKey, lastWriteTime.ToString("u"));
            metadata.Add(SizeKey, size.ToString());

            var req = new PutObjectRequest();
            req.WithInputStream(stream);
            req.WithMetaData(metadata);

            using (AmazonS3Client client = SetupClient())
                client.PutObject(req.WithBucketName(bucketName).WithKey(id));

            return true;
        }

        public DateTime LastWriteTime(string id)
        {
            using (AmazonS3Client client = SetupClient())
                return
                    DateTime.Parse(
                        client.GetObject(new GetObjectRequest().WithBucketName(bucketName).WithKey(id)).Metadata[
                            LastWriteTimeKey]);
        }

        #endregion

        #region Private Methods

        private AmazonS3Client SetupClient()
        {
            var S3Config = new AmazonS3Config
                               {
                                   ServiceURL = "s3.amazonaws.com",
                                   CommunicationProtocol = Protocol.HTTP
                               };

            return new AmazonS3Client(AccessKey, AccessSecret, S3Config);
        }

        #endregion
    }
}