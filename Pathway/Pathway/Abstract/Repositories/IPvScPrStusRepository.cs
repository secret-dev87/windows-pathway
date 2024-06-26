﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvScPrStusRepository
    {
        long GetServerBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerCPUBusyProcessCount(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp);
        Dictionary<string, List<CPUView>> GetServerBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp);
        List<ServerUnusedServerProcessesView> GetServerUnusedServerProcessPerPathway(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<UnusedServerProcesses> GetUnusedServerProcesseses(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerMaxLinks> GetServerMaxLinks(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CheckDirectoryON> GetCheckDirectoryOnDetail(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<HighDynamicServers> GetHighDynamicServersDetail(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerUnusedServerProcessesView> GetServerPendingProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, double> GetCPUBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUBusyPerInterval> GetCPUBusyPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUBusyPerInterval> GetCPUBusyPercentPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}
