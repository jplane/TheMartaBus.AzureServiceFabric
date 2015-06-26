
using Marta.Common;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Text;

namespace Marta.DataFeed
{
    public class EventHubSink : ISnapshotSink
    {
        private readonly string _ehConnectionString = null;
        private readonly string _ehName = null;

        public EventHubSink()
        {
            _ehConnectionString = CloudConfigurationManager.GetSetting("ehConnectionString");
            _ehName = CloudConfigurationManager.GetSetting("ehName");
        }

        public void HandleSnapshot(BusSnapshotInfo snapshot)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(_ehConnectionString, _ehName);

            var snapJson = JsonConvert.SerializeObject(snapshot);

            var data = new EventData(Encoding.UTF8.GetBytes(snapJson));

            eventHubClient.Send(data);

            eventHubClient.Close();
        }
    }
}
