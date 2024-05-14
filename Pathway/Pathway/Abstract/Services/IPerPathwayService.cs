using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway;

namespace Pathway.Core.Abstract {
    public interface IPerPathwayService {
        Dictionary<string, CPUDetailView> GetCPUBusyDetailFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, int ipu, string systemSerial);
        CPUSummaryView GetCPUSummaryFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, string systemSerial);
        Dictionary<string, CPUSummaryView> GetIntervalCPUSummaryFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes, string systemSerial);
        TransactionServerView GetTransactionServerFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, TransactionServerView> GetIntervalTransactionServerFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);
        TransactionTcpView GetTransactionTCPFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, TransactionTcpView> GetIntervalTransactionTCPFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);
    }
}