using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Abstract.Repositories;
using Pathway.Core.Abstract.Services;
using Pathway.Core.Repositories;

namespace Pathway.Core.Services
{
    public class PvCPUBusyService : IPvCPUBusyService
    {
        static readonly IPvCPUBusiesRepository _pvCPUBusiesRepository = new PvCPUBusiesRepository();
        public void InsertCPUBusyFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp, double cpuBusy)
        {
            _pvCPUBusiesRepository.InsertCPUBusy(pathwayName, fromTimestamp, toTimestamp, cpuBusy);
        }
    }
}
