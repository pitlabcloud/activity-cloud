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
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NooSphere.Cloud.Data.Registry;

#endregion

namespace NooSphere.Cloud.ActivityManager.Authentication
{
    public class RequireUserAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var deviceRegistry = new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);

            Guid connectionId;
            try
            {
                connectionId = new Guid(actionContext.Request.Headers.Authorization.ToString());
            }
            catch (NullReferenceException)
            {
                actionContext.Response = new HttpResponseMessage { 
                    Content = new StringContent("AuthorizationHeader not set."), 
                    StatusCode = HttpStatusCode.NonAuthoritativeInformation };
                return;
            }

            try
            {
                if (!deviceRegistry.IsUserConnected(connectionId))
                {
                    actionContext.Response = new HttpResponseMessage
                                                 {
                                                     Content = new StringContent("ConnectionId not known. Try to reconnect."),
                                                     StatusCode = HttpStatusCode.NonAuthoritativeInformation
                                                 };
                }
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation);
            }
        }
    }
}