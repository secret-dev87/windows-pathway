using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Abstract {
    internal interface IPvScTStat {
        long GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        long GetServerToTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerCPUBusyTcpTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerCPUBusyAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, TcpToServer> GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp);
        List<ServerQueueTcpView> GetServerQueueTCP(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        List<string> GetUnusedServerList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        
    }
}