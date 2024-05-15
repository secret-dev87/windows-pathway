using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;

namespace Pathway.Core.Abstract.Services
{
    public interface IPvAlertService {
        void InsertEmptyDataFor(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);
        List<Alert> GetHourlyAlertFor(string alertName, DateTime fromTimestamp, DateTime toTimestamp);
        void UpdateAlertFor(string alertName, List<Alert> alert);
        Dictionary<string, Dictionary<DateTime, AlertView>> GetCollectionAlertForAllIntervals(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp);
        Dictionary<string, AlertView> GetCollectionAlertFor(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");
        Dictionary<string, AlertView> GetLastIntervalAlertFor(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp);
        List<Alert> GetCollectionAlertFor(string pathwayName, string alertName, DateTime fromTimestamp, DateTime toTimestamp);
        Dictionary<DateTime, AlertView> GetIntervalAlertFor(List<string> alertList, DateTime fromTimestamp, DateTime toTimestamp, string list, Enums.IntervalTypes intervalTypes);
        Dictionary<string, List<UnusedServerProcesses>> GetUnusedServerProcessesesFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        Dictionary<string, List<ServerMaxLinks>> GetServerMaxLinksFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        Dictionary<string, List<CheckDirectoryON>> GetCheckDirectoryONFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        Dictionary<string, List<HighDynamicServers>> GetHighDynamicServersFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        Dictionary<string, List<ServerErrorListView>> GetServerErrorListIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        Dictionary<string, List<ServerQueueTcpSubView>> GetQueueTCPSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        Dictionary<string, List<ServerQueueTcpSubView>> GetQueueLinkmonSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
        Dictionary<string, List<ServerErrorView>> GetServerErrorListSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, List<DateTime> datesWithData = null);
    }
}