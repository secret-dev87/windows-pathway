using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Abstract {
    public interface IPvScStus {
        List<ServerErrorListView> GetServerErrorLists(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        List<ServerErrorView> GetServerErrorListSub(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        

        List<string> GetServerPendingClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        int GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, string serverClassName);

        Dictionary<String, int> GetFreezeState(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}