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
            services.Replace(typeof(IDocumentationProvider), new XmlCommentDocumentationProvider(HttpContext.Current.Server.MapPath("~/Content/ApiHelp.XML")));
        }
    }
}