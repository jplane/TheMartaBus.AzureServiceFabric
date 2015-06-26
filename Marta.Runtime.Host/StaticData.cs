
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

using MC = Marta.Common;

namespace Marta.Runtime.Host
{
    internal static class StaticData
    {
        public static MC.RouteInfo GetRouteById(int id)
        {
            MC.RouteInfo route = null;

            _routes.Value.TryGetValue(id, out route);

            return route;
        }

        public static IEnumerable<MC.RouteInfo> Routes
        {
            get { return _routes.Value.Values; }
        }

        private static Lazy<Dictionary<int, MC.RouteInfo>> _routes = new Lazy<Dictionary<int, MC.RouteInfo>>(() =>
        {
            return GetStaticData<MC.RouteInfo>(StaticDataType.Routes, line =>
            {
                var elements = line.Split(',');

                return new MC.RouteInfo
                {
                    Id = int.Parse(elements[0]),
                    ShortName = elements[1],
                    Name = elements[2]
                };
            })
            .ToDictionary(r => r.Id);
        });

        public static MC.TripInfo GetTripById(int id)
        {
            MC.TripInfo trip = null;

            _trips.Value.TryGetValue(id, out trip);

            return trip;
        }

        public static IEnumerable<MC.TripInfo> Trips
        {
            get { return _trips.Value.Values; }
        }

        private static Lazy<Dictionary<int, MC.TripInfo>> _trips = new Lazy<Dictionary<int, MC.TripInfo>>(() =>
        {
            return GetStaticData<MC.TripInfo>(StaticDataType.Trips, line =>
            {
                var elements = line.Split(',');

                return new MC.TripInfo
                {
                    RouteId = int.Parse(elements[0]),
                    Id = int.Parse(elements[2]),
                    ShapeId = int.Parse(elements[6]),
                    Headsign = elements[3]
                };
            })
            .ToDictionary(t => t.Id);
        });

        public static MC.StopInfo GetStopById(int id)
        {
            MC.StopInfo stop = null;

            _stops.Value.TryGetValue(id, out stop);

            return stop;
        }

        public static IEnumerable<MC.StopInfo> Stops
        {
            get { return _stops.Value.Values; }
        }

        private static Lazy<Dictionary<int, MC.StopInfo>> _stops = new Lazy<Dictionary<int, MC.StopInfo>>(() =>
        {
            return GetStaticData<MC.StopInfo>(StaticDataType.Stops, line =>
            {
                var elements = line.Split(',');

                return new MC.StopInfo
                {
                    Id = int.Parse(elements[0]),
                    Latitude = double.Parse(elements[3]),
                    Longitude = double.Parse(elements[4]),
                    Name = elements[2]
                };
            })
            .ToDictionary(s => s.Id);
        });

        public static IEnumerable<MC.StopTimeInfo> GetStopTimesByStopId(int stopId)
        {
            MC.StopTimeInfo[] times = null;

            _stoptimes.Value.Item1.TryGetValue(stopId, out times);

            return times ?? Enumerable.Empty<MC.StopTimeInfo>();
        }

        public static IEnumerable<MC.StopTimeInfo> GetStopTimesByTripId(int tripId)
        {
            MC.StopTimeInfo[] times = null;

            _stoptimes.Value.Item2.TryGetValue(tripId, out times);

            return times ?? Enumerable.Empty<MC.StopTimeInfo>();
        }

        public static IEnumerable<MC.StopInfo> GetStopsByRouteId(int routeId)
        {
            var tripsForRoute = Trips.Where(t => t.RouteId == routeId)
                                     .Select(t => t.Id)
                                     .ToArray();

            return _stoptimes.Value.Item1.SelectMany(pair => pair.Value)
                                         .Join(tripsForRoute, st => st.TripId, tripId => tripId, (st, tripId) => st.StopId)
                                         .Distinct()
                                         .Select(stopId => GetStopById(stopId));
        }

        private static Lazy<Tuple<Dictionary<int, MC.StopTimeInfo[]>, Dictionary<int, MC.StopTimeInfo[]>>> _stoptimes =
            new Lazy<Tuple<Dictionary<int, MC.StopTimeInfo[]>, Dictionary<int, MC.StopTimeInfo[]>>>(() =>
        {
            var stoptimes = GetStaticData<MC.StopTimeInfo>(StaticDataType.StopTimes, line =>
            {
                var elements = line.Split(',');

                return new MC.StopTimeInfo
                {
                    TripId = int.Parse(elements[0]),
                    Arrival = new TimeSpan(int.Parse(elements[1].Split(':')[0]), int.Parse(elements[1].Split(':')[1]), int.Parse(elements[1].Split(':')[2])),
                    Departure = new TimeSpan(int.Parse(elements[2].Split(':')[0]), int.Parse(elements[2].Split(':')[1]), int.Parse(elements[2].Split(':')[2])),
                    StopId = int.Parse(elements[3]),
                    Sequence = int.Parse(elements[4])
                };
            })
            .ToArray();

            return Tuple.Create(
                stoptimes.GroupBy(st => st.StopId).ToDictionary(g => g.Key, g => g.ToArray()),
                stoptimes.GroupBy(st => st.TripId).ToDictionary(g => g.Key, g => g.ToArray()));
        });

        private static IEnumerable<T> GetStaticData<T>(StaticDataType type, Func<string, T> factory)
        {
            using (var strm = LoadCsvStream(type))
            using (var reader = new StreamReader(strm))
            {
                reader.ReadLine();  // skip header

                var line = reader.ReadLine();
                
                while(! string.IsNullOrWhiteSpace(line))
                {
                    yield return factory(line);

                    line = reader.ReadLine();
                }
            }
        }

        private static Stream LoadCsvStream(StaticDataType type)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            var resourceName = string.Format("Marta.Runtime.Host.static_data.{0}.txt", type.ToString().ToLower());

            return assembly.GetManifestResourceStream(resourceName);
        }
    }

    internal enum StaticDataType
    {
        Trips,
        Routes,
        Stops,
        StopTimes,
        Shapes
    }
}
