
using Marta.Runtime.Interfaces;
using Marta.Common;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceFabric.Actors;
using System.Runtime.Serialization;

namespace Marta.Runtime.Host
{
    public class Bus : Actor<BusState>, IBus
    {
        public Task<BusInfo> GetInfo()
        {
            return Task.FromResult((BusInfo)this.State.LatestSnapshot);
        }

        public async Task UpdateStatus(BusSnapshotInfo snapshot)
        {
            this.State.LatestSnapshot = snapshot;

            await UpdateMap(snapshot);
            await UpdateStops(snapshot);
        }

        private async Task UpdateStops(BusSnapshotInfo status)
        {
            var trip = ActorProxy.Create<ITrip>(new ActorId(status.TripId));

            var stoptimes = await trip.GetStopTimes(status);

            if (this.State.LastStopId != null)
            {
                if (stoptimes.Item1 == null || stoptimes.Item1.StopId != this.State.LastStopId)
                {
                    var lastStop = ActorProxy.Create<IStop>(new ActorId(this.State.LastStopId.Value));
                    await lastStop.NoLongerDeparted(this);
                }
            }

            this.State.LastStopId = stoptimes.Item1 == null ? null : (long?)stoptimes.Item1.StopId;

            if (this.State.LastStopId != null)
            {
                var lastStop = ActorProxy.Create<IStop>(new ActorId(this.State.LastStopId.Value));
                await lastStop.HasDeparted(this, status.AdjustedTimestamp.Subtract(stoptimes.Item1.Departure));
            }

            if (this.State.NextStopId != null)
            {
                if (stoptimes.Item2 == null || stoptimes.Item2.StopId != this.State.NextStopId)
                {
                    var nextStop = ActorProxy.Create<IStop>(new ActorId(this.State.NextStopId.Value));
                    await nextStop.IsNoLongerApproaching(this);
                }
            }

            this.State.NextStopId = stoptimes.Item2 == null ? null : (long?)stoptimes.Item2.StopId;

            if (this.State.NextStopId != null)
            {
                var nextStop = ActorProxy.Create<IStop>(new ActorId(this.State.NextStopId.Value));
                await nextStop.IsApproaching(this, stoptimes.Item2.Arrival.Subtract(status.AdjustedTimestamp));
            }
        }

        private async Task UpdateMap(BusSnapshotInfo status)
        {
            var signalrUri = CloudConfigurationManager.GetSetting("signalrUri");

            var conn = new HubConnection(signalrUri);

            var hub = conn.CreateHubProxy("MapHub");

            await conn.Start();

            await hub.Invoke("UpdateBus", status);
        }
    }

    [DataContract]
    public class BusState
    {
        [DataMember]
        public long? LastStopId { get; set; }
        [DataMember]
        public long? NextStopId { get; set; }
        [DataMember]
        public BusSnapshotInfo LatestSnapshot { get; set; }
    }
}
