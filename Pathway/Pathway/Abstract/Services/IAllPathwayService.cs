using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure.AllPathway;

namespace Pathway.Core.Abstract.Services {
    public interface IAllPathwayService {
        Dictionary<string, List<CPUDetailView>> GetCPUBusyDetailFor(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial, int ipu);
		Dictionary<string, PathwayHourlyView> GetPathwayHourly(DateTime fromTimestamp, DateTime toTimestamp);
		CPUSummaryView GetCPUSummaryFor(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial);
        TransactionServerView GetTransactionServer(DateTime fromTimestamp, DateTime toTimestamp);
        TransactionTcpView GetTransactionTcp(DateTime fromTimestamp, DateTime toTimestamp);
        Dictionary<string, List<CPUDetailView>> GetCPUBusyDetailPerIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, string systemSerial);
        Dictionary<string, List<CPUDetailView>> GetCPUBusyDetailPercentPerIntervalFor(DateTime fromTimestamp, DateTime toTimestamp, int ipus);
    }
}