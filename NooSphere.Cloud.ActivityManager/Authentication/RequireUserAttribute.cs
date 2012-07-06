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
