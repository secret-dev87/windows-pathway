using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;

namespace Pathway.Core.Abstract {
    internal interface IPvPwyMany {
        long GetPathmonProcessBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetPathmonProcessBusyPerCPU(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetPathmonProcessBusyPerPathway(DateTime fromTimestamp, DateTime toTimestamp);
        Dictionary<string, List<CPUView>> GetPathmonProcessBusyPerCPUPerPathway(DateTime fromTimestamp, DateTime toTimestamp);

        
        string GetPathwayName(DateTime fromTimestamp, DateTime toTimestamp);
    }
}