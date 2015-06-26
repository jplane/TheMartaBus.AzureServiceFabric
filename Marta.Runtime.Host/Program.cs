using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Actors;

namespace Marta.Runtime.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    fabricRuntime.RegisterActor(typeof(Bus));
                    fabricRuntime.RegisterActor(typeof(Trip));
                    fabricRuntime.RegisterActor(typeof(Route));
                    fabricRuntime.RegisterActor(typeof(RouteCatalog));
                    fabricRuntime.RegisterActor(typeof(Stop));

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e);
                throw;
            }
        }
    }
}
