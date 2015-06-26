
using System;

namespace Marta.Common
{
    public class StopTimeInfo
    {
        public int StopId { get; set; }
        public int TripId { get; set; }
        public int Sequence { get; set; }
        public TimeSpan Arrival { get; set; }
        public TimeSpan Departure { get; set; }
    }
}