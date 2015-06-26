
using Marta.Common;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Marta.DataFeed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var diContainer = new UnityContainer();

            diContainer.LoadConfiguration();

            var source = diContainer.Resolve<ISnapshotSource>();

            while (true)
            {
                Console.WriteLine("Retrieving snapshots...");

                var snapshots = source.GetSnapshots();

                Parallel.ForEach(snapshots, snapshot =>
                {
                    var handler = diContainer.Resolve<ISnapshotSink>();

                    Console.WriteLine("Writing snapshot...");

                    handler.HandleSnapshot(snapshot);
                });

                Console.WriteLine("Sleeping...");

                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }
    }
}
