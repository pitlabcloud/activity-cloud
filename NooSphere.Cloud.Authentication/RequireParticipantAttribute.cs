using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Net.Http.Headers;
using System.Web;
using System.IO;
using NooSphere.Core.ActivityModel;
using System.Security.Principal;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Configuration;

namespace NooSphere.Cloud.Authentication
{
    public class RequireParticipantAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string auth = null;
            try
            {
                auth = actionContext.Request.Headers.Authorization.ToString();
            }
            catch (NullReferenceException)
            {
                Unauthorized(actionContext, HttpStatusCode.NonAuthoritativeInformation);
                return;
            }

            // check that it starts with 'Participant = '
            if (!auth.StartsWith("Participant = "))
            {
                Unauthorized(actionContext, HttpStatusCode.Unauthorized);
                return;
            }

            // trim off the leading and trailing double-quotes and get pure json
            string json = HttpUtility.UrlDecode(auth.Substring("Participant = ".Length + 1, auth.Length - "Participant = ".Length - 2));
            Participant p;
            try
            {
                p = JsonConvert.DeserializeObject<Participant>(json);
                if (string.IsNullOrEmpty(p.Email)) throw new Exception("Email is not set.");
            }
            catch (Exception)
            {
                Unauthorized(actionContext, HttpStatusCode.Unauthorized);
                return;
            }
            /*
             * We need to find a solution with regards to ACS and appharbor
            // create a token validator
            TokenValidator validator = new TokenValidator(
                ConfigurationManager.AppSettings["ACSHostName"],
                ConfigurationManager.AppSettings["ServiceNamespace"],
                ConfigurationManager.AppSettings["TrustedAudience"],
                ConfigurationManager.AppSettings["TrustedTokenPolicyKey"]);

            // validate the token
            if (!validator.Validate(p.AccessToken))
            {
                Unauthorized(actionContext, HttpStatusCode.Unauthorized);
                return;
            }
            */
            // set current user
            HttpContext.Current.User = p;
        }

        private void Unauthorized(HttpActionContext actionContext, HttpStatusCode statusCode)
        {
            actionContext.Response = new HttpResponseMessage(statusCode);
            //throw new HttpResponseException(statusCode);
        }
    }
}
