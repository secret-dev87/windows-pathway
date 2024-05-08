using System;

namespace Pathway.Core.Abstract {
    public interface ICPUBusyService {
        void InsertCPUBusyFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp, double cpuBusy);
    }
}