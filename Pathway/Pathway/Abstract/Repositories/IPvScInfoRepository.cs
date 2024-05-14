using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvScInfoRepository
    {
        List<CPUView> GetServerCPUBusy(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0);

        List<CPUView> GetNumStaticMaxServers(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0);

        List<CPUView> GetServerProcessCount(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0);

        List<CPUView> GetServerAverageResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int topCount = 0);

        List<ServerCPUBusyView> GetServerTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        List<ServerCPUBusyView> GetServerTransactionsPerInterval(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}
