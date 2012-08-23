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
        public HttpResponseMessage Get(Guid activityId, Guid resourceId)
        {
            var response = new HttpResponseMessage();
            try {
                var stream = _fileStorage.Download(GenerateId(activityId, resourceId));
                if (stream != null)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StreamContent(stream);
                }
            } catch(Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(e.Message);
            }
            return response;
        }

        /// <summary>
        ///   Upload the resource
        /// </summary>
        /// <param name="activityId"> Guid representation of the activity Id. </param>
        /// <param name="resourceId"> Guid representation of the resource Id. </param>
        [RequireUser]
        public Task<HttpResponseMessage> Post(Guid activityId, Guid resourceId)
        {
            var resource = new ActivityController().GetActivity(activityId).Resources.SingleOrDefault(r => r.Id == resourceId);
            if(resource != null) {
                var task = Request.Content.ReadAsStreamAsync();
                var result = task.ContinueWith(o =>
                {
                    if (_fileStorage.Upload(GenerateId(resource), task.Result))
                        Notifier.NotifyGroup(activityId, NotificationType.FileDownload, resource);
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                });

                return result;
            }
            return null;
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