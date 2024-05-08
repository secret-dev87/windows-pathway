using System;
using System.Collections.Generic;
using Pathway.Core.Infrastructure;
using Pathway.Core.Infrastructure.PerPathway.Server;
using Pathway.Core.Infrastructure.PerPathway.Tcp;
using Pathway.Core.Infrastructure.PerPathway.Term;

namespace Pathway.Core.Abstract {
    public interface IPerPathwayTcpService {
        List<TcpTransactionView> GetTcpTransactionFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName);
        Dictionary<string, List<TcpQueuedTransactionView>> GetTcpQueuedTransactionFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);

        Dictionary<string, List<ServerQueueTcpSubView>> GetQueueTCPSubFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);

        Dictionary<string, List<ServerUnusedServerClassView>> GetTcpUnusedFor(DateTime fromTimestamp, DateTime toTimestamp, string pathwayName, Enums.IntervalTypes intervalTypes);
    }
}