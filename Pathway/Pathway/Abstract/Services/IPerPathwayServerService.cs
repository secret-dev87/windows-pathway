using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Abstract {
    public interface IPerPathwayServerService {
        List<ServerCPUBusyView> GetServerCPUBusyFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int ipu);
        List<ServerCPUBusyView> GetServerProcessCountFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        List<ServerQueueTcpView> GetServerQueueTCPFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, List<ServerQueueTcpView>> GetServerQueueTCPIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        List<ServerQueueLinkmonView> GetServerQueueLinkmonFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        Dictionary<string, List<ServerCPUBusyView>> GetServerTransactionsIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);

        List<ServerCPUBusyView> GetServerTransactionsFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int ipu);

        List<ServerUnusedServerClassView> GetServerUnusedClassesFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        Dictionary<string, List<ServerUnusedServerClassView>> GetServerUnusedClassesIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);

        Dictionary<string, List<ServerUnusedServerProcessesView>> GetServerUnusedProcessesIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);

        List<ServerUnusedServerProcessesView> GetServerUnusedProcessesFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);

        Dictionary<string, List<ServerQueueLinkmonView>> GetServerQueueLinkmonIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);

        Dictionary<string, List<string>> GetServerPendingClassIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);

        Dictionary<string, List<ServerUnusedServerProcessesView>> GetServerPendingProcessIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);

        List<ServerCPUBusyView> GetServerTransactionsPerIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
    }
}