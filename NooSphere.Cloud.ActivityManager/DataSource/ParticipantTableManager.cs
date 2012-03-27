using System.Collections.Generic;
using Lokad.Cloud.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using NooSphere.Cloud.ActivityManager.DataSource;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager
{
    public class ParticipantTableManager
    {
        /// <summary>
        /// Name of the participant table
        /// </summary>
        private const string participantTableName = "participants";

        /// <summary>
        /// Name of the partition key
        /// </summary>
        private const string partitionKey = "abc";

        private const string connectionStringName = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        /// <summary>
        /// Linq query to return all stored participants
        /// </summary>
        /// <returns>a list of participants</returns>
        public List<Participant> GetParticipants()
        {
            var results = new List<Participant>();

            foreach (CloudEntity<ParticipantWrapper> entity in ParticipantsTable.Get())
                results.Add(entity.Value.Participant);
            
            return results;
        }

        /// <summary>
        /// Add a participant in the cloud
        /// </summary>
        public void AddParticipant(Participant newItem)
        {
            ParticipantWrapper pw = new ParticipantWrapper { ID = newItem.Email, Participant = newItem };

            // inserting (or updating record in Table Storage)
            ParticipantsTable.Upsert(new[]
                {
                    new CloudEntity<ParticipantWrapper> {
                        PartitionKey = partitionKey, RowKey = pw.ID, Value = pw }
                });
        }

        /// <summary>
        /// Get a participant from the cloud
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Participant GetParticipant(string email)
        {
            Maybe<CloudEntity<ParticipantWrapper>> maybe = ParticipantsTable.Get(partitionKey, email);
            CloudEntity<ParticipantWrapper> entity = maybe.Value;
            ParticipantWrapper pw = entity.Value;
            return pw.Participant;
        }

        /// <summary>
        /// Removes an object from the tableservice
        /// </summary>
        /// <param name="item"></param>
        public void RemoveParticipant(string email)
        {
            ParticipantsTable.Delete(partitionKey, email);
        }

        private CloudTable<ParticipantWrapper> ParticipantsTable
        {
            get
            {
                return new CloudTable<ParticipantWrapper>(CloudStorage.ForAzureAccount(StorageAccount).BuildTableStorage(), participantTableName);
            }
        }

        private CloudStorageAccount StorageAccount
        {
            get
            {
                return CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(connectionStringName));
            }
        }
    }
}
