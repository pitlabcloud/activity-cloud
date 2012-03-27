using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using NooSphere.Cloud.Authentication;
using NooSphere.Core.ActivityModel;
using System.Web;

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class UsersController : ApiController
    {
        private static ParticipantTableManager ptm = new ParticipantTableManager();

        // GET /api/users
        public IEnumerable<Participant> Get()
        {
            return ptm.GetParticipants();
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
                ptm.AddParticipant(p);
            }
        }
    }
}