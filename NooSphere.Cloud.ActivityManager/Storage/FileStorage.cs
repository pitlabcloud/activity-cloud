using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.S3;
using System.Configuration;
using Amazon.S3.Model;
using NooSphere.Core.FileManagement;
using NooSphere.Core.ActivityModel;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace NooSphere.Cloud.ActivityManager.Storage
{
    public class FileStorage
    {
        private const string amazonAccesKeyId = "AmazonAccessKeyId";
        private const string amazonSecretAccessKey = "AmazonSecretAccessKey";
        private const string bucketName = "dk.itu.pitlab.activitycloud.files";

        private const string FileNameKey = "FileName";
        private const string CreationTimeKey = "CreationTime";
        private const string LastWriteTimeKey = "LastWriteTime";
        private const string SizeKey = "Size";

        /// <summary>
        /// Download files from Amazon S3
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public FileBatch DownloadFiles(FileBatch batch)
        {
            int start = 0;
            foreach (FileDetails details in batch.Files)
            {
                using (var client = SetupClient())
                {
                    GetObjectResponse response = client.GetObject(new GetObjectRequest().WithBucketName(bucketName).WithKey(details.FileName));
                    response.ResponseStream.Write(batch.ByteStream, start, (int)details.Size);
                }
                start += (int)details.Size;
            }
            return batch;
        }

        /// <summary>
        /// Upload files to Amazon S3
        /// </summary>
        /// <param name="batch"></param>
        public void UploadFiles(FileBatch batch)
        {
            int start = 0;
            foreach (FileDetails details in batch.Files)
            {
                NameValueCollection metadata = new NameValueCollection();
                metadata.Add(FileStorage.FileNameKey, details.FileName);
                metadata.Add(FileStorage.SizeKey, details.Size.ToString());
                metadata.Add(FileStorage.CreationTimeKey, details.CreationTime.ToString());
                metadata.Add(FileStorage.LastWriteTimeKey, details.LastWriteTime.ToString());

                MemoryStream stream = new MemoryStream();
                stream.Write(batch.ByteStream, start, (int)details.Size);
                start += (int)details.Size;

                var req = new PutObjectRequest();
                req.WithInputStream(stream);

                using (var client = SetupClient())
                {
                    client.PutObject(req.WithBucketName(bucketName).WithKey(details.FileName).WithMetaData(metadata));
                }
            }
        }

        /// <summary>
        /// Delete all files without a reference
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        public List<FileDetails> DeleteUnreferencedFiles(List<Activity> activities)
        {
            List<FileDetails> files = new List<FileDetails>();

            using(var client = SetupClient())
            {
                ListObjectsRequest req = new ListObjectsRequest();
                req.BucketName = bucketName;

                foreach (var s3o in client.ListObjects(req).S3Objects)
                {
                    if (!activities.Any(a => a.GetResources().Any(r => r.FileDetails.FileName == s3o.Key)))
                    {
                        FileDetails f = new FileDetails();
                        f.FileName = s3o.Key;
                        f.SyncAction = SyncAction.Delete;
                        files.Add(f);
                        DeleteFile(f);
                    }
                }
            }

            return files;
        }

        public FileBatch GetChangeBatch(Activity activity)
        {
            FileBatch changeBatch = new FileBatch();

            foreach (var r in activity.GetResources())
            {
                FileDetails upload = r.FileDetails;
                upload.SyncAction = SyncAction.Upload;
                changeBatch.Files.Add(upload);
            }

            return changeBatch;
        }


        public FileBatch GetChangeBatch(Activity oldActivity, Activity newActivity)
        {
            FileBatch changeBatch = new FileBatch();
            List<S3Object> filesInCloud;

            using (var client = SetupClient())
            {
                filesInCloud = client.ListObjects(new ListObjectsRequest().WithBucketName(bucketName)).S3Objects;
            }

            var oldResources = oldActivity.GetResources();

            foreach (var r in newActivity.GetResources())
            {
                // File exists in cloud
                if(filesInCloud.Exists(f => f.Key == r.FileDetails.FileName))
                {
                    FileDetails file = GetFileDetails(r.FileDetails.FileName);

                    // Cloud file is newer than local
                    if (DateTime.Parse(file.LastWriteTime) > DateTime.Parse(r.FileDetails.LastWriteTime))
                    {
                        FileDetails download = file;
                        download.SyncAction = SyncAction.Download;
                        changeBatch.Files.Add(download);
                    }
                    // Cloud file is older than local
                    else if (DateTime.Parse(file.LastWriteTime) < DateTime.Parse(r.FileDetails.LastWriteTime))
                    {
                        FileDetails upload = r.FileDetails;
                        r.FileDetails.SyncAction = SyncAction.Upload;
                        changeBatch.Files.Add(upload);
                    }
                }
                // File doesn't exist in cloud
                else
                {
                    FileDetails upload = r.FileDetails;
                    r.FileDetails.SyncAction = SyncAction.Upload;
                    changeBatch.Files.Add(upload);
                }
            }

            return changeBatch;
        }

        /// <summary>
        /// Delete file in Amazon S3
        /// </summary>
        /// <param name="details"></param>
        private void DeleteFile(FileDetails details)
        {
            using (var client = SetupClient())
            {
                client.DeleteObject(new DeleteObjectRequest().WithBucketName(bucketName).WithKey(details.FileName));
            }
        }

        private AmazonS3Client SetupClient()
        {
            return new AmazonS3Client(ConfigurationManager.AppSettings[amazonAccesKeyId], ConfigurationManager.AppSettings[amazonSecretAccessKey]);
        }

        private FileDetails GetFileDetails(string key)
        {
            FileDetails details = new FileDetails();

            using (var client = SetupClient())
            {
                GetObjectResponse response = client.GetObject(new GetObjectRequest().WithBucketName(bucketName).WithKey(key));
                details.FileName = response.Metadata[FileStorage.FileNameKey];
                details.Size = int.Parse(response.Metadata[FileStorage.SizeKey]);
                details.CreationTime = response.Metadata[FileStorage.CreationTimeKey];
                details.LastWriteTime = response.Metadata[FileStorage.LastWriteTimeKey];
            }

            return details;
        }
    }
}