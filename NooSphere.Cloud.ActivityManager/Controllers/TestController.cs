using System.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using NooSphere.Cloud.Data.Registry;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TestController : ApiController
    {
        DeviceRegistry DeviceRegistry = new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        UserRegistry UserRegistry = new UserRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);

        [HttpGet]
        public int Users()
        {
            return UserRegistry.Get().Count;
        }
        [HttpGet]
        public int Devices()
        {
            return DeviceRegistry.Get().Count;
        }
    }
}
