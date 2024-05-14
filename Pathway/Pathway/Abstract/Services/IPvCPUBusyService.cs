using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Services
{
    public interface IPvCPUBusyService
    {
        void InsertCPUBusyFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp, double cpuBusy);
    }
}
