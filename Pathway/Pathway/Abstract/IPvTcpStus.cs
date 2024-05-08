using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Tcp;

namespace Pathway.Core.Abstract {
    internal interface IPvTcpStus {
        long GetTcpBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetTcpBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetTcpBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp);
        Dictionary<string, List<CPUView>> GetTcpBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp);

        List<TcpQueuedTransactionView> GetTcpQueuedTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        
    }
}