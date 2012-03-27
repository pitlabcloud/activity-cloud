using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NooSphere.Cloud.Authentication;
using NooSphere.Core.ActivityModel;
using NooSphere.Core.FileManagement;
using System.Web;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class OldRestHelloController : ApiController
    {
        // GET /rest/
        public string Get()
        {
            return "hello client";
        }
    }
}