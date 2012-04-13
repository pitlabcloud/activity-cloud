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
    public class ParticipantsController : ApiController
    {
        private static ParticipantStorage participantStorage = new ParticipantStorage();

        // GET /api/participants
        [RequireParticipant]
        public IEnumerable<Participant> Get()
        {
            return participantStorage.GetParticipants();
        }

        // GET /api/participants/{id}
        [RequireParticipant]
        public Participant Get(string id)
        {
            return participantStorage.GetParticipant(id);
        }

        // POST /api/participants
        [RequireParticipant]
        public void Post(Participant p)
        {
            if (p != null)
                participantStorage.AddParticipant(p);
        }

        // PUT /api/participants
        [RequireParticipant]
        public void Put(Participant p)
        {
            if (p != null)
                participantStorage.AddParticipant(p);
        }

        // DELETE /api/participants{id}
        [RequireParticipant]
        public void Delete(string id)
        {
            participantStorage.RemoveParticipant(id);
        }
    }
}