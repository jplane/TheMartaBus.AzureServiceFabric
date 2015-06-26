
using Microsoft.ServiceFabric.Actors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marta.Runtime.Interfaces
{
    public interface IRouteCatalog : IActor
    {
        Task<IEnumerable<IRoute>> GetRoutes();
    }
}
