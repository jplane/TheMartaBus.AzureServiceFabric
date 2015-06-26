
using Marta.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marta.DataFeed
{
    public interface ISnapshotSource
    {
        IEnumerable<BusSnapshotInfo> GetSnapshots();
    }
}
