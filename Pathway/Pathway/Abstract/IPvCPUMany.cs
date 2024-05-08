using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure.AllPathway;

namespace Pathway.Core.Abstract {
    internal interface IPvCPUMany {
        double GetCPUElapse(DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, double> GetCPUElapsePerCPU(DateTime fromTimestamp, DateTime toTimestamp);

        CPUSummaryView GetCPUElapseAndBusyTime(DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyTimePerCPU(DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyPercentPerCPU(DateTime fromTimestamp, DateTime toTimestamp, int ipus);

        List<string> GetCPUCount(DateTime fromTimestamp, DateTime toTimestamp);

        
    }
}