
using Marta.Common;
using Microsoft.ServiceFabric.Actors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface IRoute : IActor
    {
        Task<RouteInfo> GetInfo();

        Task<IEnumerable<IStop>> GetStops();
    }
}
