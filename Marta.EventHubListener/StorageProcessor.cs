
using Marta.Common;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marta.EventHubListener
{
    internal class StorageProcessor : IEventProcessor
    {
        private CloudStorageAccount _account = null;

        public StorageProcessor()
        {
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine(string.Format("StorageProcessor opening.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));

            _account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("azureTableConnection"));

            return Task.FromResult<object>(null);
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine(string.Format("StorageProcessor closing. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason.ToString()));

            _account = null;

            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var batch = new TableBatchOperation();

            foreach(var msg in messages)
            {
                var snap = JsonConvert.DeserializeObject<BusSnapshotInfo>(Encoding.UTF8.GetString(msg.GetBytes()));

                var entity = new DynamicTableEntity(snap.RouteShortName, snap.VehicleId.ToString());

                entity.Properties.Add("RouteShortName", EntityProperty.GeneratePropertyForString(snap.RouteShortName));
                entity.Properties.Add("VehicleId", EntityProperty.GeneratePropertyForInt(snap.VehicleId));
                entity.Properties.Add("TripId", EntityProperty.GeneratePropertyForInt(snap.TripId));
                entity.Properties.Add("Latitude", EntityProperty.GeneratePropertyForDouble(snap.Latitude));
                entity.Properties.Add("Longitude", EntityProperty.GeneratePropertyForDouble(snap.Longitude));
                entity.Properties.Add("DirectionOfTravel", EntityProperty.GeneratePropertyForString(snap.DirectionOfTravel.ToString()));
                entity.Properties.Add("NextStopId", EntityProperty.GeneratePropertyForInt(snap.NextStopId));
                entity.Properties.Add("Timeliness", EntityProperty.GeneratePropertyForString(snap.Timeliness.ToString()));
                entity.Properties.Add("TimelinessOffset", EntityProperty.GeneratePropertyForInt(snap.TimelinessOffset));
                entity.Properties.Add("Timestamp", EntityProperty.GeneratePropertyForDateTimeOffset(snap.Timestamp));

                batch.Add(TableOperation.InsertOrReplace(entity));
            }

            var tableClient = _account.CreateCloudTableClient();

            var table = tableClient.GetTableReference("snapshots");

            await table.CreateIfNotExistsAsync();

            await table.ExecuteBatchAsync(batch);

            await context.CheckpointAsync();
        }
    }
}
