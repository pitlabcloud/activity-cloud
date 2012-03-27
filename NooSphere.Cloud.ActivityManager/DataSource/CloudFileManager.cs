using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using NooSphere.Core.FileManagement;
using NooSphere.Core.ActivityModel;
using System.Configuration;

namespace NooSphere.Cloud.ActivityManager
{

    /// <summary>
    /// TODO: build a decent file manager that deals with all the 
    /// Blob management stuff
    /// </summary>
    public class CloudFileManager
    {
        private const string FileNameKey = "FileName";
        private const string CreationTimeKey = "CreationTime";
        private const string LastWriteTimeKey = "LastWriteTime";
        private const string SizeKey = "Size";

        /// <summary>
        /// Connection string
        /// </summary>
        /// <remark>
        /// Currently local for enulator test. Configure this in
        /// NooSphere.Cloud -> Roles -> NooSphere.Cloud.ActivityManager -> Settings
        /// when deploying
        /// </remark> 
        private const string connectionStringName = "ConnectionString";
        private const string blobContainerName = "files";

        public FileDetails[] GetBlobs()
        {
            List<IListBlobItem> items = BlobContainer.ListBlobs().ToList();

            List<FileDetails> files = new List<FileDetails>();

            foreach (CloudBlob blob in items.OfType<CloudBlob>())
            {
                blob.FetchAttributes();
                FileDetails fd = new FileDetails();
                fd.CloudPath = blob.Uri.AbsoluteUri;
                fd.FileName = blob.Metadata[CloudFileManager.FileNameKey];
                files.Add(fd);
            }
            foreach (CloudBlobDirectory dir in items.OfType<CloudBlobDirectory>())
                files.AddRange(GetFileDetails(dir));

            return files.ToArray();
        }

        private List<FileDetails> GetFileDetails(CloudBlobDirectory cbd)
        {
            List<FileDetails> list = new List<FileDetails>();

            foreach (CloudBlobDirectory dir in cbd.ListBlobs().OfType<CloudBlobDirectory>())
                list.AddRange(GetFileDetails(dir));

            foreach (CloudBlob blob in cbd.ListBlobs().OfType<CloudBlob>())
            {
                blob.FetchAttributes();
                FileDetails fd = new FileDetails();
                fd.FileName = blob.Metadata[CloudFileManager.FileNameKey];
                list.Add(fd);
            }

            return list;
        }

        /// <summary>
        /// Delete all files without a reference
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        public List<FileDetails> DeleteUnreferencedFiles(List<Activity> activities)
        {
            List<FileDetails> files = new List<FileDetails>();

            try
            {
                Dictionary<string, FileDetails> cloudBatch = GetCloudBatch();

                foreach (var f in cloudBatch.Values.ToList())
                {
                    if (!activities.Any(i => i.GetResources().Any(j => j.FileDetails.FileName == f.FileName)))
                    {
                        f.SyncAction = SyncAction.Delete;
                        files.Add(f);
                        DeleteBlob(f);
                    }
                }
            }
            catch { }

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
            Dictionary<string, FileDetails> cloudBatch = GetCloudBatch();

            var oldResources = oldActivity.GetResources();

            foreach (var r in newActivity.GetResources())
            {
                // File exists in cloud
                if (cloudBatch.ContainsKey(r.FileDetails.FileName))
                {
                    // Cloud file is newer than local
                    if (DateTime.Parse(cloudBatch[r.FileDetails.FileName].LastWriteTime) > DateTime.Parse(r.FileDetails.LastWriteTime))
                    {
                        FileDetails download = cloudBatch[r.FileDetails.FileName];
                        download.SyncAction = SyncAction.Download;
                        changeBatch.Files.Add(download);
                    }
                    // Cloud file is older than local
                    else if (DateTime.Parse(cloudBatch[r.FileDetails.FileName].LastWriteTime) < DateTime.Parse(r.FileDetails.LastWriteTime))
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

        private Dictionary<string, FileDetails> GetCloudBatch()
        {
            Dictionary<string, FileDetails> files = new Dictionary<string, FileDetails>();

            foreach (CloudBlockBlob blob in BlobContainer.ListBlobs().OfType<CloudBlockBlob>())
            {
                FileDetails fd = CreateFileDetails(blob);
                files.Add(fd.FileName, fd);
            }

            foreach (CloudBlobDirectory dir in BlobContainer.ListBlobs().OfType<CloudBlobDirectory>())
            {
                files = files.Concat(TraverseCloudBlobDirectory(dir)).ToDictionary(e => e.Key, e => e.Value);
            }
            return files;
        }

        private Dictionary<string, FileDetails> TraverseCloudBlobDirectory(CloudBlobDirectory cbd)
        {
            Dictionary<string, FileDetails> files = new Dictionary<string, FileDetails>();

            foreach (CloudBlockBlob blob in cbd.ListBlobs().OfType<CloudBlockBlob>())
            {
                FileDetails fd = CreateFileDetails(blob);
                files.Add(fd.FileName, fd);
            }

            foreach (CloudBlobDirectory dir in cbd.ListBlobs().OfType<CloudBlobDirectory>())
            {
                files = files.Concat(TraverseCloudBlobDirectory(dir)).ToDictionary(e => e.Key, e => e.Value);
            }

            return files;
        }

        private FileDetails CreateFileDetails(CloudBlockBlob blob)
        {
            FileDetails details = new FileDetails();
            blob.FetchAttributes();
            details.FileName = blob.Metadata[CloudFileManager.FileNameKey];
            details.CloudPath = blob.Uri.AbsoluteUri;
            details.Size = long.Parse(blob.Metadata[CloudFileManager.SizeKey]);
            details.CreationTime = blob.Metadata[CloudFileManager.CreationTimeKey];
            details.LastWriteTime = blob.Metadata[CloudFileManager.LastWriteTimeKey];
            return details;
        }

        /// <summary>
        /// Uploads a filebatch to Azure Blob Storage
        /// </summary>
        /// <param name="batch"></param>
        public void UploadToBlob(FileBatch batch)
        {
            int start = 0;
            foreach (FileDetails details in batch.Files)
            {
                CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(details.FileName);
                byte[] data = new byte[details.Size];
                Buffer.BlockCopy(batch.ByteStream, start, data, 0, (int)details.Size);
                start += (int)details.Size;
                blob.Metadata.Add(CloudFileManager.FileNameKey, details.FileName);
                blob.Metadata.Add(CloudFileManager.SizeKey, details.Size.ToString());
                blob.Metadata.Add(CloudFileManager.CreationTimeKey, details.CreationTime.ToString());
                blob.Metadata.Add(CloudFileManager.LastWriteTimeKey, details.LastWriteTime.ToString());
                blob.UploadByteArray(data);
            }
        }

        /// <summary>
        /// Delete a blob in Azure Blob Storage
        /// </summary>
        /// <param name="details"></param>
        public void DeleteBlob(FileDetails details)
        {
            BlobContainer.GetBlockBlobReference(details.FileName).DeleteIfExists();
        }

        private CloudBlobContainer BlobContainer
        {
            get
            {
                CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
                CloudBlobContainer BlobContainer = BlobClient.GetContainerReference(blobContainerName);
                BlobContainer.CreateIfNotExist();

                // Let the container have public access
                BlobContainerPermissions perms = new BlobContainerPermissions();
                perms.PublicAccess = BlobContainerPublicAccessType.Container;
                BlobContainer.SetPermissions(perms);

                return BlobContainer;
            }
        }

        /// <summary>
        /// Downloads a filebatch from Azure Blob Storage
        /// </summary>
        /// <param name="fileset"></param>
        /// <returns></returns>
        public FileBatch DownloadFromBlob(FileBatch batch)
        {
            foreach (FileDetails details in batch.Files)
            {
                CloudBlockBlob blob = BlobContainer.GetBlockBlobReference(details.FileName);
                byte[] a = batch.ByteStream;
                byte[] b = blob.DownloadByteArray();
                byte[] c = new byte[a.Length + b.Length];
                
                Buffer.BlockCopy(a, 0, c, 0, a.Length);
                Buffer.BlockCopy(b, 0, c, a.Length, b.Length);

                batch.ByteStream = c;
            }
            
            return batch;
        }

        public CloudStorageAccount StorageAccount 
        {
            get
            {
                //return CloudStorageAccount.DevelopmentStorageAccount;
                return CloudStorageAccount.Parse(ConfigurationManager.AppSettings[connectionStringName]);
            }
        }
    }
}