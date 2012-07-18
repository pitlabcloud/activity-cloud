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
using System.Configuration;
using System.Linq;
using System.Web.Http;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers.Api
{
    public class BaseController : ApiController
    {
        #region Protected Members
        protected DeviceRegistry DeviceRegistry = new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        protected UserRegistry UserRegistry = new UserRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        #endregion

        #region Protected Getters
        protected Guid ConnectionId {
            get
            {
                if (Request == null || Request.Headers.Authorization == null) return Guid.Empty;
                return new Guid(Request.Headers.Authorization.ToString()); 
            } 
        }

        protected Guid CurrentUserId { 
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
            if (activity.Owner.Id == CurrentUser.Id)
                return true;
            else
                return false;
        }

        protected bool IsParticipant(Activity activity)
        {
            if (activity.Participants.Count(p => p.Id == CurrentUser.Id) == 1)
                return true;
            else
                return false;
        }
        #endregion
    }
}