using Pathway.Core.Infrastructure.PerPathway.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathway.Core.Abstract.Repositories
{
    internal interface IPvScStusRepository
    {
        List<ServerErrorListView> GetServerErrorLists(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerErrorView> GetServerErrorListSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<string> GetServerPendingClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        int GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, string serverClassName);
        Dictionary<String, int> GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}
