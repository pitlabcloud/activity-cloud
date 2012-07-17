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

using System.Web.Http;
using System.Web.Mvc;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            return View(apiExplorer);
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Model()
        {
            return View();
        }

        public ActionResult Connect()
        {
            return View();
        }
    }
}
