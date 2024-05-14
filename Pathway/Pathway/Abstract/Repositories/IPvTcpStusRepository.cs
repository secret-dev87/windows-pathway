using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Tcp;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvTcpStusRepository
    {
        long GetTcpBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetTcpBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetTcpBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp);
        Dictionary<string, List<CPUView>> GetTcpBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp);
        List<TcpQueuedTransactionView> GetTcpQueuedTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}
