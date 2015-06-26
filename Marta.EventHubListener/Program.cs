
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using System;

namespace Marta.EventHubListener
{
    class Program
    {
        static void Main(string[] args)
        {
            var ehConnectionString = CloudConfigurationManager.GetSetting("ehConnectionString");
            var ehName = CloudConfigurationManager.GetSetting("ehName");
            var storageConnectionString = CloudConfigurationManager.GetSetting("azureTableConnection");

            //var storageProcHost = new EventProcessorHost(Guid.NewGuid().ToString(), ehName, "storage", ehConnectionString, storageConnectionString);
            //storageProcHost.RegisterEventProcessorAsync<StorageProcessor>().Wait();

            var entityProcHost = new EventProcessorHost(Guid.NewGuid().ToString(), ehName, "servicefabric", ehConnectionString, storageConnectionString);
            entityProcHost.RegisterEventProcessorAsync<EntityProcessor>().Wait();

            Console.ReadLine();
        }
    }
}
