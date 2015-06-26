
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceFabric.Actors;
using Marta.Runtime.Interfaces;
using System.Threading.Tasks;
using Marta.Common;
using Microsoft.Azure;

namespace Marta.Runtime.Host
{
    public class Route : Actor, IRoute
    {
        public Task<RouteInfo> GetInfo()
        {
            return Task.FromResult(StaticData.GetRouteById((int)this.GetActorId<IRoute>().GetLongId()));
        }

        public Task<IEnumerable<IStop>> GetStops()
        {
            var stops = StaticData.GetStopsByRouteId((int)this.GetActorId<IRoute>().GetLongId())
                                  .Select(si => ActorProxy.Create<IStop>(new ActorId(si.Id)))
                                  .ToArray()
                                  .AsEnumerable();

            return Task.FromResult(stops);
        }
    }
}
