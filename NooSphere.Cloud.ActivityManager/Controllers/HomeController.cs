using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}
