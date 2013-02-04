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
using System.Linq;
using System.Web.Http;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Core.ActivityModel;

#endregion

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class BaseController : ApiController
    {
        protected DeviceRegistry DeviceRegistry = new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"], ConfigurationManager.AppSettings["MongoDb"]);
        protected UserRegistry UserRegistry = new UserRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"], ConfigurationManager.AppSettings["MongoDb"]);

        #region Protected Getters

        protected Guid ConnectionId
        {
            get
            {
                if (Request == null || Request.Headers.Authorization == null) return Guid.Empty;
                return new Guid(Request.Headers.Authorization.ToString());
            }
        }

        protected Guid CurrentUserId
        {
            get
            {
                if (ConnectionId == Guid.Empty) return Guid.Empty;
                return DeviceRegistry.GetUserId(ConnectionId);
            }
        }

        protected User CurrentUser
        {
            get
            {
                if (CurrentUserId == Guid.Empty) return null;
                return UserRegistry.Get(CurrentUserId);
            }
        }

        #endregion

        #region Protected Methods
        protected bool IsOwner(Activity activity)
        {
            return activity.Owner.Id == CurrentUser.Id;
        }

        protected bool IsParticipant(Activity activity)
        {
            return activity.Participants.Count(p => p.Id == CurrentUser.Id) == 1;
        }

        #endregion
    }
}