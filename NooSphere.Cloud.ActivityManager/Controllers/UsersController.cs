using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using NooSphere.Cloud.Authentication;
using NooSphere.Core.ActivityModel;
using System.Web;
using NooSphere.Cloud.ActivityManager.Storage;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class UsersController : ApiController
    {
        private static ParticipantStorage participantStorage = new ParticipantStorage();

        // GET /api/users
        public IEnumerable<Participant> Get()
        {
            return participantStorage.GetParticipants();
        }

        // GET /api/users/{email}
        public Participant Get(string email)
        {
            return new Participant();
            //return ptm.GetParticipant(email);
        }

        // POST /api/users
        [RequireParticipant]
        public void Post(Participant p)
        {
            if (p != null)
            {
                // Add participant to Participant Table Manager
                participantStorage.AddParticipant(p);
            }
        }
    }
}