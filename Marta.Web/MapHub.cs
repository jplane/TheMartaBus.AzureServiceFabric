
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marta.Common;
using Marta.Runtime.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.Azure;
using Microsoft.ServiceFabric.Actors;

namespace Marta.Web
{
    public class MapHub : Hub
    {
        public Task RegisterMapView()
        {
            return Groups.Add(Context.ConnectionId, "mapViews");
        }
        
        public async Task<IEnumerable<RouteInfo>> GetRoutes()
        {
            var appName = CloudConfigurationManager.GetSetting("sfAppName");
            var svcName = CloudConfigurationManager.GetSetting("sfSvcNameTemplate");

            var catalog = ActorProxy.Create<IRouteCatalog>(ActorId.NewId(), appName, string.Format(svcName, "RouteCatalog"));

            var infos = new List<RouteInfo>();

            foreach(var route in await catalog.GetRoutes())
            {
                infos.Add(await route.GetInfo());
            }

            return infos;
        }

        public async Task<IEnumerable<StopInfo>> GetStopsForRoute(int routeId)
        {
            var appName = CloudConfigurationManager.GetSetting("sfAppName");
            var svcName = CloudConfigurationManager.GetSetting("sfSvcNameTemplate");

            var route = ActorProxy.Create<IRoute>(new ActorId(routeId), appName, string.Format(svcName, "Route"));

            var infos = new List<StopInfo>();

            foreach(var stop in await route.GetStops())
            {
                infos.Add(await stop.GetInfo());
            }

            return infos;
        }

        public Task BusApproachingStop(StopInfo stop, int vehicleId, TimeSpan delta)
        {
            return Task.FromResult(0);
        }

        public Task BusNoLongerApproachingStop(StopInfo stop, int vehicleId)
        {
            return Task.FromResult(0);
        }

        public Task BusHasDepartedStop(StopInfo stop, int vehicleId, TimeSpan delta)
        {
            return Task.FromResult(0);
        }

        public Task BusNoLongerDepartedStop(StopInfo stop, int vehicleId)
        {
            return Task.FromResult(0);
        }

        public async Task UpdateBus(BusSnapshotInfo snapshot)
        {
            var appName = CloudConfigurationManager.GetSetting("sfAppName");
            var svcName = CloudConfigurationManager.GetSetting("sfSvcNameTemplate");

            var trip = ActorProxy.Create<ITrip>(new ActorId(snapshot.TripId), appName, string.Format(svcName, "Trip"));

            var tripInfo = await trip.GetInfo();

            var headsign = tripInfo == null ? string.Format("[{0}]", snapshot.TripId) : tripInfo.Headsign;

            await Clients.Group("mapViews").updateBus(snapshot, headsign);
        }
    }
}
