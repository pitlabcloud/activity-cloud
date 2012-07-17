/// <licence>
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
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NooSphere.Cloud.Data.Registry;

namespace NooSphere.Cloud.ActivityManager.Authentication
{
    public class RequireUserAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            DeviceRegistry DeviceRegistry = new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);

            Guid connectionId = Guid.Empty;
            try
            {
                connectionId = new Guid(actionContext.Request.Headers.Authorization.ToString());
            }
            catch (NullReferenceException)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation);
                return;
            }

            try
            {
                if (!DeviceRegistry.IsUserConnected(connectionId))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation);
                    return;
                }
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation);
                return;
            }
        }
    }
}
