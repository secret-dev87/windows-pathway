using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure.AllPathway;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvCPUManyRepository
    {
        double GetCPUElapse(DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, double> GetCPUElapsePerCPU(DateTime fromTimestamp, DateTime toTimestamp);

        CPUSummaryView GetCPUElapseAndBusyTime(DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyTimePerCPU(DateTime fromTimestamp, DateTime toTimestamp);

        Dictionary<string, CPUDetailElapseAndBusyTimeView> GetCPUElapseAndBusyPercentPerCPU(DateTime fromTimestamp, DateTime toTimestamp, int ipus);

        List<string> GetCPUCount(DateTime fromTimestamp, DateTime toTimestamp);
    }
}
