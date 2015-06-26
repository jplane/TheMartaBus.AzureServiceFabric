
using Marta.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace Marta.DataFeed
{
    public class HttpSource : ISnapshotSource
    {
        private const string MartaUrl = "http://developer.itsmarta.com/BRDRestService/BRDRestService.svc/GetAllBus";

        public HttpSource()
        {
        }

        public IEnumerable<BusSnapshotInfo> GetSnapshots()
        {
            var http = new HttpClient();

            var raw = http.GetStringAsync(MartaUrl).Result;

            var jsonStops = JArray.Parse(raw);

            return jsonStops.Select(json =>
            {
                return new BusSnapshotInfo
                {
                    RouteShortName = (string)json["ROUTE"],
                    TripId = (int)json["TRIPID"],
                    NextStopId = (int)json["STOPID"],
                    VehicleId = (int)json["VEHICLE"],
                    DirectionOfTravel = GetDirectionOfTravel((string)json["DIRECTION"]),
                    Latitude = (double)json["LATITUDE"],
                    Longitude = (double)json["LONGITUDE"],
                    Timestamp = DateTimeOffset.Parse((string)json["MSGTIME"]),
                    Timeliness = GetTimeliness(int.Parse((string)json["ADHERENCE"])),
                    TimelinessOffset = Math.Abs(int.Parse((string)json["ADHERENCE"]))
                };
            });
        }

        private static Timeliness GetTimeliness(int adherence)
        {
            return adherence == 0 ? Timeliness.OnTime : adherence > 0 ? Timeliness.Early : Timeliness.Late;
        }

        private static Direction GetDirectionOfTravel(string direction)
        {
            switch (direction.ToLowerInvariant())
            {
                case "northbound":
                    return Direction.North;
                case "southbound":
                    return Direction.South;
                case "eastbound":
                    return Direction.East;
                case "westbound":
                    return Direction.West;
                default:
                    return (Direction)0;
            }
        }
    }
}
