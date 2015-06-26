
using Marta.Common;
using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface IBus : IActor
    {
        Task<BusInfo> GetInfo();

        Task UpdateStatus(BusSnapshotInfo snapshot);
    }
}
