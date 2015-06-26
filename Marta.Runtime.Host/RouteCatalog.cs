
using Marta.Common;
using Marta.Runtime.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace Marta.Runtime.Host
{
    public class RouteCatalog : Actor, IRouteCatalog
    {
        public Task<IEnumerable<IRoute>> GetRoutes()
        {
            return Task.FromResult(StaticData.Routes.Select(ri => ActorProxy.Create<IRoute>(new ActorId(ri.Id)))
                                                    .ToArray()
                                                    .AsEnumerable());
        }
    }
}
