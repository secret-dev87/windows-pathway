using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Term;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvTermStatRepository
    {
        long GetTermToTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, long> GetTermToTcp(DateTime fromTimestamp, DateTime toTimestamp);
        List<TermView> GetTermTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<TermView> GetTermAvgResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<TermView> GetTermMaxResponseTime(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<TermUnusedView> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerUnusedServerClassView> GetTcpUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}
