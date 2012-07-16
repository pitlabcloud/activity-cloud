using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NooSphere.Cloud.Data.Registry;
using NooSphere.Cloud.Data.Storage;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class BaseController : ApiController
    {
        protected ActionRegistry ActionRegistry = new ActionRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        protected ActivityRegistry ActivityRegistry = new ActivityRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        protected DeviceRegistry DeviceRegistry = new DeviceRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        protected UserRegistry UserRegistry = new UserRegistry(ConfigurationManager.AppSettings["MONGOLAB_URI"]);

        protected ActionStorage ActionStorage = new ActionStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        protected ActivityStorage ActivityStorage = new ActivityStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        protected UserStorage UserStorage = new UserStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        protected FileStorage FileStorage = new FileStorage(ConfigurationManager.AppSettings["AmazonAccessKeyId"], ConfigurationManager.AppSettings["AmazonSecretAccessKey"]);
        
        protected Guid ConnectionId {
            get
            {
                if (Request.Headers.Authorization == null) return Guid.Empty;
                return new Guid(Request.Headers.Authorization.ToString()); 
            } 
        }

        protected Guid CurrentUserId { 
            get 
            {
                if (ConnectionId == Guid.Empty) return Guid.Empty;
                return DeviceRegistry.GetUserId(ConnectionId); 
            } 
        }

        protected User CurrentUser 
        { 
            get
            {
                if (CurrentUserId == Guid.Empty) return null;
                return UserRegistry.Get(CurrentUserId); 
            } 
        }

        #region Authorization
        protected bool IsOwner(Activity activity)
        {
            if (activity.Owner.Id == CurrentUser.Id)
                return true;
            else
                return false;
        }

        protected bool IsParticipant(Activity activity)
        {
            if (activity.Participants.Count(p => p.Id == CurrentUser.Id) == 1)
                return true;
            else
                return false;
        }
        #endregion
    }
}