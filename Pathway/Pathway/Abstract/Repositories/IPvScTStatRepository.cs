using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvScTStatRepository
    {
        long GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        long GetServerToTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerCPUBusyTcpTrans(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<CPUView> GetServerCPUBusyAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, TcpToServer> GetTcpToServer(DateTime fromTimestamp, DateTime toTimestamp);
        List<ServerQueueTcpView> GetServerQueueTCP(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<string> GetUnusedServerList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerQueueTcpSubView> GetQueueTCPSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}
