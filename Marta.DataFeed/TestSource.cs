
using Marta.Common;
using System;
using System.Collections.Generic;

namespace Marta.DataFeed
{
    public class TestSource : ISnapshotSource
    {
        public TestSource()
        {
        }

        public IEnumerable<BusSnapshotInfo> GetSnapshots()
        {
            yield return new BusSnapshotInfo
            {
                RouteShortName = "55",
                TripId = 33,
                NextStopId = 455,
                VehicleId = 12,
                DirectionOfTravel = Direction.East,
                Latitude = -33.00234,
                Longitude = 1.3344571,
                Timestamp = DateTimeOffset.UtcNow,
                Timeliness = Timeliness.OnTime,
                TimelinessOffset = 0
            };

            yield return new BusSnapshotInfo
            {
                RouteShortName = "55",
                TripId = 33,
                NextStopId = 667,
                VehicleId = 88,
                DirectionOfTravel = Direction.East,
                Latitude = -33.00234,
                Longitude = 1.3344571,
                Timestamp = DateTimeOffset.UtcNow,
                Timeliness = Timeliness.OnTime,
                TimelinessOffset = 0
            };
        }
    }
}
