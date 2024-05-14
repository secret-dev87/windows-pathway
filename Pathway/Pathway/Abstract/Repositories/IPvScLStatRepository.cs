using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvScLStatRepository
    {
        long GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerCPUBusyLinkmonTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, LinkMonToServer> GetLinkmonToServer(DateTime fromTimestamp, DateTime toTimestamp);
        List<ServerQueueLinkmonView> GetServerQueueLinkmon(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerUnusedServerClassView> GetServerUnusedServerClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerQueueTcpSubView> GetQueueLinkmonSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}
