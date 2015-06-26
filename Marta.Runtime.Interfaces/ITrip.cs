
using Marta.Common;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface ITrip : IActor
    {
        Task<TripInfo> GetInfo();

        Task<Tuple<StopTimeInfo, StopTimeInfo>> GetStopTimes(BusSnapshotInfo status);
    }
}
