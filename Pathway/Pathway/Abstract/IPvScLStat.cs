using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Abstract {
    public interface IPvScLStat {
        long GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerCPUBusyLinkmonTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, LinkMonToServer> GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp);

        List<ServerQueueLinkmonView> GetServerQueueLinkmon(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        List<ServerUnusedServerClassView> GetServerUnusedServerClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        List<ServerQueueTcpSubView> GetQueueLinkmonSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        
    }
}