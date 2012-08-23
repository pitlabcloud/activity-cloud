#region License

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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NooSphere.Cloud.ActivityManager.Authentication;
using NooSphere.Cloud.ActivityManager.Events;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

#endregion

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class FileController : BaseController
    {
        private readonly FileStorage _fileStorage = new FileStorage(
            ConfigurationManager.AppSettings["AmazonAccessKeyId"],
            ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);

        #region Exposed API Methods

        /// <summary>
        ///   Download the resource.
        /// </summary>
        /// <param name="activityId"> Guid representation of the activity Id. </param>
        /// <param name="resourceId"> Guid representation of the resource Id. </param>
        /// <returns> byte[] of the given resource </returns>
        [RequireUser]
        public Task<HttpResponseMessage> Get(Guid activityId, Guid resourceId)
        {
            try
            {
                var task = _fileStorage.DownloadFileAsync(GenerateId(activityId, resourceId));
                var result = task.ContinueWith(o => new HttpResponseMessage
                                                        {
                                                            StatusCode = HttpStatusCode.OK,
                                                            Content = new StreamContent(task.Result)
                                                        });
            
            return result;
            } catch(Exception e)
            {
                var response = new HttpResponseMessage
                                   {
                                       StatusCode = HttpStatusCode.InternalServerError,
                                       Content = new StringContent(e.Message)
                                   };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///   Upload the resource
        /// </summary>
        /// <param name="activityId"> Guid representation of the activity Id. </param>
        /// <param name="resourceId"> Guid representation of the resource Id. </param>
        [RequireUser]
        public Task<HttpResponseMessage> Post(Guid activityId, Guid resourceId)
        {
            try {
                var resource = new ActivityController().GetActivity(activityId).Resources.SingleOrDefault(r => r.Id == resourceId);
                if(resource != null) {
                    var task = Request.Content.ReadAsStreamAsync();
                    var result = task.ContinueWith(o =>
                                                       {
                                                           var uploadTask = _fileStorage.Upload(GenerateId(resource), task.Result);
                                                           return uploadTask.ContinueWith(u =>
                                                                                       {
                                                                                           if(u.Result)
                                                                                           {
                                                                                               Notifier.NotifyGroup(activityId, NotificationType.FileDownload, resource);
                                                                                               return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                                                                                           }
                                                                                           throw new HttpResponseException(new HttpResponseMessage
                                                                                                                               {
                                                                                                                                   StatusCode = HttpStatusCode.NotFound,
                                                                                                                                   Content = new StringContent("The resource was not uploaded.")
                                                                                                                               });
                                                                                       });
                    });
                    return result.Result;
                }
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("The resource was not found.")
                });
            } catch(Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(e.Message)
                });
            }
        }

        #endregion

        #region Public Methods

        [NonAction]
        public void Sync(Activity activity, SyncType type, Guid connectionId)
        {
            foreach (var resource in activity.Resources)
            {
                switch (type)
                {
                    case SyncType.Added:
                        Notifier.NotifyGroup(connectionId, NotificationType.FileUpload, resource);
                        break;
                    case SyncType.Removed:
                        Notifier.NotifyGroup(activity.Id, NotificationType.FileDelete, resource);
                        break;
                    case SyncType.Updated:
                        if (resource.LastWriteTime > _fileStorage.LastWriteTime(GenerateId(resource)))
                            Notifier.NotifyGroup(connectionId, NotificationType.FileUpload, resource);
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private static string GenerateId(Guid activityId, Guid resourceId)
        {
            return activityId + "/" + resourceId;
        }

        private static string GenerateId(Resource r)
        {
            return r.ActivityId + "/" + r.Id;
        }

        #endregion
    }

    public enum SyncType
    {
        Added,
        Removed,
        Updated
    }
}