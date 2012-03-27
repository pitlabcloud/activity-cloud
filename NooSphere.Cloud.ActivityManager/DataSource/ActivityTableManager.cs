using System.Collections.Generic;
using Lokad.Cloud.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using NooSphere.Cloud.ActivityManager.DataSource;
using NooSphere.Core.ActivityModel;

namespace NooSphere.Cloud.ActivityManager
{
    public class ActivityTableManager
    {
        /// <summary>
        /// Name of the activity table
        /// </summary>
        private const string activityTableName = "activities";

        /// <summary>
        /// Name of the partition key
        /// </summary>
        private const string partitionKey = "abc";

        private const string connectionStringName = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        /// <summary>
        /// Linq query to return all stored activities
        /// </summary>
        /// <returns>a list of activities</returns>
        public List<Activity> GetActivities()
        {
            var results = new List<Activity>();

            foreach (CloudEntity<ActivityWrapper> entity in ActivitiesTable.Get())
                results.Add(entity.Value.Activity);
            
            return results;
        }

        /// <summary>
        /// Add an activity in the cloud
        /// </summary>
        public void AddActivity(Activity newItem)
        {
            ActivityWrapper activity = new ActivityWrapper { ID = newItem.Identity.ID.ToString(), Activity = newItem };

            // inserting (or updating record in Table Storage)
            ActivitiesTable.Upsert(new[]
                {
                    new CloudEntity<ActivityWrapper> {
                        PartitionKey = partitionKey, RowKey = activity.ID, Value = activity }
                });
        }

        /// <summary>
        /// Get an activity from the cloud
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Activity GetActivity(string id)
        {
            Maybe<CloudEntity<ActivityWrapper>> maybe = ActivitiesTable.Get(partitionKey, id);
            CloudEntity<ActivityWrapper> entity = maybe.Value;
            ActivityWrapper act = entity.Value;
            return act.Activity;
        }

        /// <summary>
        /// Removes an object from the tableservice
        /// </summary>
        /// <param name="item"></param>
        public void RemoveActivity(string id)
        {
            ActivitiesTable.Delete(partitionKey, id);
        }

        private CloudTable<ActivityWrapper> ActivitiesTable
        {
            get
            {
                return new CloudTable<ActivityWrapper>(CloudStorage.ForAzureAccount(StorageAccount).BuildTableStorage(), activityTableName);
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
