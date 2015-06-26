
using Marta.Common;
using Marta.Runtime.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.Runtime.Host
{
    public class Trip : Actor, ITrip
    {
        public Task<TripInfo> GetInfo()
        {
            return Task.FromResult(StaticData.GetTripById((int)this.GetActorId<ITrip>().GetLongId()));
        }

        public Task<Tuple<StopTimeInfo, StopTimeInfo>> GetStopTimes(BusSnapshotInfo status)
        {
            StopTimeInfo last = null;
            StopTimeInfo next = null;

            GetStopTimes(status, out last, out next);

            return Task.FromResult(Tuple.Create(last, next));
        }

        private void GetStopTimes(BusSnapshotInfo status, out StopTimeInfo last, out StopTimeInfo next)
        {
            last = next = null;

            var times = StaticData.GetStopTimesByTripId(status.TripId);

            var stoptimes = new LinkedList<StopTimeInfo>(times.OrderBy(st => st.Sequence));

            if (stoptimes == null || stoptimes.Count == 0)
            {
                return;
            }

            var busTime = status.AdjustedTimestamp;

            LinkedListNode<StopTimeInfo> node = stoptimes.First;

            while(node != null)
            {
                if(busTime < node.Value.Arrival)
                {
                    next = node.Value;
                    break;
                }

                last = node.Value;
                node = node.Next;
            }
        }
    }
}
