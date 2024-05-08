using System;
using System.Collections.Generic;

namespace Pathway.Core.Abstract {
    internal interface IPvAlerts {
        void InsertEmptyData(string pathwayName, DateTime fromTimestamp, DateTime toTimestamp);

        #region Hourly Data
        List<Infrastructure.Alert> GetTermHiMaxRtHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTermHiAvgRtHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTermUnusedHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTermErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTcpQueuedTransactionsHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTcpLowTermPoolHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTcpLowServerPoolHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTcpUnusedHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetTcpErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetServerHiMaxRtHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetServerHiAvgRtHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetServerQueueTcpHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetServerQueueLinkmonHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetServerUnusedClassHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetServerUnusedProcessHourly(DateTime fromTimestamp, DateTime toTimestamp);

        List<Infrastructure.Alert> GetServerErrorListHourly(DateTime fromTimestamp, DateTime toTimestamp);
        #endregion

        #region Collection Data
        List<Infrastructure.Alert> GetTermHiMaxRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTermHiAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTermUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTermErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTcpQueuedTransactions(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTcpLowTermPool(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTcpLowServerPool(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTcpUnused(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetTcpErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerHiMaxRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerHiAvgRt(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerPendingClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerPendingProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerQueueTcp(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerQueueLinkmon(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerUnusedClass(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerUnusedProcess(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetServerMaxLinks(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetCheckDirectoryOn(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");

        List<Infrastructure.Alert> GetHighDynamicServers(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");
            
        List<Infrastructure.Alert> GetServerErrorList(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName = "");
        #endregion

        void UpdateAlert(string alertName, string pathwayName, long count, DateTime fromTimestamp, DateTime toTimestamp);

    }
}