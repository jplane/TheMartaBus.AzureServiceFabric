
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Marta.Common
{
    [DataContract]
    public class BusInfo
    {
        [DataMember]
        public int TripId { get; set; }
        [DataMember]
        public int VehicleId { get; set; }
        [DataMember]
        public string RouteShortName { get; set; }
    }

    [DataContract]
    public class BusSnapshotInfo : BusInfo
    {
        [DataMember]
        public int NextStopId { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public DateTimeOffset Timestamp { get; set; }
        [DataMember]
        public Direction DirectionOfTravel { get; set; }
        [DataMember]
        public Timeliness Timeliness { get; set; }
        [DataMember]
        public int TimelinessOffset { get; set; }

        public TimeSpan AdjustedTimestamp
        {
            get { return this.Timestamp.ToLocalTime().TimeOfDay.Add(TimeSpan.FromMinutes(this.TimelinessOffset)); }
        }
    }

    public enum Timeliness
    {
        OnTime = 1,
        Early,
        Late
    }

    public enum Direction
    {
        North = 1,
        South,
        East,
        West
    }
}
