
using Marta.Common;
using Marta.Runtime.Interfaces;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.Azure;

namespace Marta.EventHubListener
{
    internal class EntityProcessor : IEventProcessor
    {
        public EntityProcessor()
        {
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine(string.Format("EntityProcessor opening.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));

            return Task.FromResult<object>(null);
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine(string.Format("EntityProcessor closing. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason.ToString()));

            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var appName = CloudConfigurationManager.GetSetting("sfAppName");
            var svcName = CloudConfigurationManager.GetSetting("sfSvcNameTemplate");

            foreach (var msg in messages)
            {
                var status = JsonConvert.DeserializeObject<BusSnapshotInfo>(Encoding.UTF8.GetString(msg.GetBytes()));

                var bus = ActorProxy.Create<IBus>(new ActorId(status.VehicleId), appName, string.Format(svcName, "Bus"));

                try
                {
                    await bus.UpdateStatus(status);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EntityProcessor failure: " + ex.Message);
                    throw;
                }
            }

            await context.CheckpointAsync();
        }
    }
}
