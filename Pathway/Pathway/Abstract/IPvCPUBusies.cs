using System;

namespace Pathway.Core.Abstract {
    internal interface IPvCPUBusies {
        void InsertCPUBusy(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp, double cpuBusy);

        
    }
}