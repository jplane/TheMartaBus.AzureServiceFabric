
using Marta.Common;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface IStop : IActor
    {
        Task<StopInfo> GetInfo();

        Task IsApproaching(IBus bus, TimeSpan delta);
        Task IsNoLongerApproaching(IBus bus);
        Task HasDeparted(IBus bus, TimeSpan delta);
        Task NoLongerDeparted(IBus bus);

        Task<IEnumerable<IBus>> GetApproachingBuses();
        Task<IEnumerable<IBus>> GetDepartedBuses();
    }
}
