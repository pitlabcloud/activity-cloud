using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Services;

namespace NooSphere.Cloud.ActivityManager.App_Start
{
    public class DocumentationConfig
    {
        public static void RegisterDocumentation(DefaultServices services)
        {
            services.Replace(typeof(IDocumentationProvider), new XmlCommentDocumentationProvider(HttpContext.Current.Server.MapPath("~/App_Data/ApiHelp.xml")));
        }
    }
}