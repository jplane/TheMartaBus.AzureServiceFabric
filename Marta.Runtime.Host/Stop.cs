
using Marta.Common;
using Marta.Runtime.Interfaces;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Marta.Runtime.Host
{
    public class Stop : Actor<StopState>, IStop
    {
        public Task<StopInfo> GetInfo()
        {
            return Task.FromResult(StaticData.GetStopById((int)this.GetActorId<IStop>().GetLongId()));
        }

        private async Task<IHubProxy> GetHub()
        {
            var signalrUri = CloudConfigurationManager.GetSetting("signalrUri");

            var conn = new HubConnection(signalrUri);

            var hub = conn.CreateHubProxy("MapHub");

            await conn.Start();

            return hub;
        }

        public async Task IsApproaching(IBus bus, TimeSpan delta)
        {
            var busId = bus.GetActorId().GetLongId();

            this.State.Approaching[busId] = delta;

            var hub = await GetHub();

            await hub.Invoke("BusApproachingStop", await this.GetInfo(), busId, delta);
        }

        public async Task IsNoLongerApproaching(IBus bus)
        {
            var busId = bus.GetActorId().GetLongId();

            this.State.Approaching.Remove(busId);

            var hub = await GetHub();

            await hub.Invoke("BusNoLongerApproachingStop", await this.GetInfo(), busId);
        }

        public async Task HasDeparted(IBus bus, TimeSpan delta)
        {
            var busId = bus.GetActorId().GetLongId();

            this.State.Departing[busId] = delta;

            var hub = await GetHub();

            await hub.Invoke("BusHasDepartedStop", await this.GetInfo(), busId, delta);
        }

        public async Task NoLongerDeparted(IBus bus)
        {
            var busId = bus.GetActorId().GetLongId();

            this.State.Departing.Remove(busId);

            var hub = await GetHub();

            await hub.Invoke("BusNoLongerDepartedStop", await this.GetInfo(), busId);
        }

        public Task<IEnumerable<IBus>> GetApproachingBuses()
        {
            return Task.FromResult(this.State.Approaching.Keys.Select(id => ActorProxy.Create<IBus>(new ActorId(id))));
        }

        public Task<IEnumerable<IBus>> GetDepartedBuses()
        {
            return Task.FromResult(this.State.Departing.Keys.Select(id => ActorProxy.Create<IBus>(new ActorId(id))));
        }
    }

    [DataContract]
    public class StopState
    {
        public StopState()
        {
            Approaching = new Dictionary<long, TimeSpan>();
            Departing = new Dictionary<long, TimeSpan>();
        }

        [DataMember]
        public Dictionary<long, TimeSpan> Approaching { get; set; }
        [DataMember]
        public Dictionary<long, TimeSpan> Departing { get; set; }
    }
}
